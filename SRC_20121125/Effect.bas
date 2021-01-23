Attribute VB_Name = "Effect"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'特殊効果の自動選択＆再生処理


'構えている武器の種類
Private WeaponInHand As String

'攻撃手段の種類
Private CurrentWeaponType As String


'戦闘アニメ再生用サブルーチン
Public Sub ShowAnimation(aname As String)
Dim buf As String, ret As Double, i As Integer
Dim expr As String

    If Not BattleAnimation Then
        Exit Sub
    End If
    
    '右クリック中は特殊効果をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    'サブルーチン呼び出しのための式を作成
    expr = LIndex(aname, 1)
    If InStr(expr, "戦闘アニメ_") <> 1 Then
        expr = "戦闘アニメ_" & LIndex(aname, 1)
    End If
    If FindNormalLabel(expr) = 0 Then
        ErrorMessage "サブルーチン「" & expr & "」が見つかりません"
        Exit Sub
    End If
    expr = "Call(`" & expr & "`"
    For i = 2 To LLength(aname)
        expr = expr & ",`" & LIndex(aname, i) & "`"
    Next
    expr = expr & ")"
    
    '画像描画が行われたかどうかの判定のためにフラグを初期化
    IsPictureDrawn = False
    
    'メッセージウィンドウの状態を記録
    SaveMessageFormStatus
    
    '戦闘アニメ再生
    SaveBasePoint
    CallFunction expr, StringType, buf, ret
    RestoreBasePoint
    
    'メッセージウィンドウの状態が変化している場合は復元
    KeepMessageFormStatus
    
    '画像を消去しておく
    If IsPictureDrawn And LCase$(buf) <> "keep" Then
        ClearPicture
        MainForm.picMain(0).Refresh
    End If
    
    Exit Sub
    
ErrorHandler:
    
    '戦闘アニメ実行中に発生したエラーの処理
    If Len(EventErrorMessage) > 0 Then
        DisplayEventErrorMessage CurrentLineNum, EventErrorMessage
        EventErrorMessage = ""
    Else
        DisplayEventErrorMessage CurrentLineNum, ""
    End If
End Sub


'武器準備時の特殊効果
Public Sub PrepareWeaponEffect(u As Unit, ByVal w As Integer)
    '右クリック中は特殊効果をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    If BattleAnimation Then
        PrepareWeaponAnimation u, w
    Else
        PrepareWeaponSound u, w
    End If
End Sub

'武器準備時のアニメーション
Public Sub PrepareWeaponAnimation(u As Unit, ByVal w As Integer)
Dim wname As String, wclass As String, wtype As String
Dim double_weapon As Boolean
Dim aname As String, sname As String, cname As String
Dim with_face_up As Boolean
Dim i As Integer

    '戦闘アニメ非自動選択
    If IsOptionDefined("戦闘アニメ非自動選択") Then
        Exit Sub
    End If
    
    With u
        'まず準備アニメ表示の際にフェイスアップを表示するか決定する
        If .CountWeapon >= 4 _
            And w >= .CountWeapon - 1 _
            And .Weapon(w).Power >= 1800 _
            And ((.Weapon(w).Bullet > 0 And .Weapon(w).Bullet <= 4) _
                Or .Weapon(w).ENConsumption >= 35) _
        Then
            '４つ以上の武器を持つユニットがそのユニットの最高威力
            'もしくは２番目に強力な武器を使用し、
            'その武器の攻撃力1800以上でかつ武器使用可能回数が限定されていれば
            '必殺技と見なしてフェイスアップ表示
'            with_face_up = True
        End If
        
        '空中移動専用形態は武器を手で構えない
        If .Data.Transportation = "空" Then
            WeaponInHand = ""
            GoTo SkipWeaponAnimation
        End If
        
        '等身大基準の場合、非人間ユニットはメカであることが多いので内蔵武器を優先する
        If IsOptionDefined("等身大基準") And Not .IsHero() Then
            WeaponInHand = ""
            GoTo SkipWeaponAnimation
        End If
        
        wname = .WeaponNickname(w)
        wclass = .Weapon(w).Class
    End With
    
    '武器準備のアニメーションを非表示にするオプションを選択している？
' MOD START MARGE
'    If Not WeaponAnimation Or IsOptionDefined("武器準備アニメ非表示") Then
    If (Not WeaponAnimation And Not ExtendedAnimation) _
        Or IsOptionDefined("武器準備アニメ非表示") _
    Then
' MOD END MARGE
        WeaponInHand = ""
        GoTo SkipWeaponAnimation
    End If
    
    '二刀流？
    If InStr(wname, "ダブル") > 0 Or InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "双") > 0 Or InStr(wname, "二刀") > 0 _
    Then
        double_weapon = True
    End If
    
    '「ブーン」という効果音を鳴らす？
    If InStr(wname, "高周波") > 0 Or InStr(wname, "電磁") > 0 Then
        sname = "BeamSaber.wav"
    End If
    
    'これから武器の種類を判定
    
    If InStrNotNest(wclass, "武") = 0 _
        And InStrNotNest(wclass, "突") = 0 _
        And InStrNotNest(wclass, "接") = 0 _
        And InStrNotNest(wclass, "実") = 0 _
    Then
        GoTo SkipInfightWeapon
    End If
    
    '武器名から武器の種類を判定
    wtype = CheckWeaponType(wname, wclass)
    If wtype = "手裏剣" Then
        '手裏剣は構えずにいきなり投げたほうがかっこいいと思うので
        Exit Sub
    End If
    If wtype <> "" Then
        GoTo FoundWeaponType
    End If
    
    '詳細が分からなかった武器
    If InStrNotNest(wclass, "武") > 0 Then
        '装備しているアイテムから武器を検索
        For i = 1 To u.CountItem
            With u.Item(i)
                If .Activated _
                    And (.Part = "両手" _
                        Or .Part = "片手" _
                        Or .Part = "武器") _
                Then
                    wtype = CheckWeaponType(.Nickname, "")
                    If wtype <> "" Then
                        GoTo FoundWeaponType
                    End If
                    wtype = CheckWeaponType(.Class0, "")
                    If wtype <> "" Then
                        GoTo FoundWeaponType
                    End If
                    Exit For
                End If
            End With
        Next
        GoTo SkipShootingWeapon
    End If
    
    If InStrNotNest(wclass, "突") > 0 _
        Or InStrNotNest(wclass, "接") > 0 _
    Then
        GoTo SkipShootingWeapon
    End If
    
SkipInfightWeapon:
    
    'まずはビーム攻撃かどうか判定
    If Not IsBeamWeapon(wname, wclass, cname) Then
        GoTo SkipBeamWeapon
    End If
    
    '手持ち？
    If InStr(wname, "ライフル") > 0 Or InStr(wname, "バズーカ") > 0 _
        Or Right$(wname, 2) = "ガン" _
        Or (Right$(wname, 1) = "銃" And Right$(wname, 2) <> "機銃") _
    Then
        If InStrNotNest(wclass, "Ｍ") > 0 Then
            wtype = "ＭＡＰバスタービームライフル"
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 _
            Or InStr(wname, "大") > 0 _
            Or Left$(wname, 2) = "ギガ" _
        Then
            wtype = "バスタービームライフル"
        ElseIf InStr(wname, "メガ") > 0 _
            Or InStr(wname, "ハイ") > 0 _
            Or InStr(wname, "バズーカ") > 0 _
        Then
            If double_weapon Then
                wtype = "ダブルビームランチャー"
            Else
                wtype = "ビームランチャー"
            End If
            If InStr(wname, "ライフル") > 0 Then
                wtype = "バスタービームライフル"
            End If
        ElseIf CountAttack0(u, w) >= 4 Then
            wtype = "マシンガン"
        ElseIf InStr(wname, "ピストル") > 0 _
            Or InStr(wname, "ミニ") > 0 _
            Or InStr(wname, "小") > 0 _
        Then
            wtype = "レーザーガン"
        Else
            If double_weapon Then
                wtype = "ダブルビームライフル"
            Else
                wtype = "ビームライフル"
            End If
        End If
        GoTo FoundWeaponType
    End If
    
SkipBeamWeapon:
    
    If InStr(wname, "弓") > 0 _
        Or InStr(wname, "ショートボウ") > 0 _
        Or InStr(wname, "ロングボウ") > 0 _
    Then
        wtype = "弓"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "クロスボウ") > 0 _
        Or InStr(wname, "ボウガン") > 0 _
    Then
        wtype = "クロスボウ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "バズーカ") > 0 Then
        wtype = "バズーカ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "サブマシンガン") > 0 Then
        wtype = "サブマシンガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "マシンガン") > 0 Or InStr(wname, "機関銃") > 0 Then
        If InStr(wname, "ヘビー") > 0 Or InStr(wname, "重") > 0 Then
            wtype = "ヘビーマシンガン"
        Else
            wtype = "マシンガン"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ガトリング") > 0 Then
        wtype = "ガトリング"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ピストル") > 0 Or InStr(wname, "拳銃") > 0 Then
        wtype = "ピストル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "リボルバー") > 0 Or InStr(wname, "リボルヴァー") > 0 Then
        wtype = "リボルバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ショットガン") > 0 Or InStr(wname, "ライアットガン") > 0 Then
        wtype = "ショットガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "スーパーガン") > 0 Then
        wtype = "スーパーガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "スーパーキャノン") > 0 Then
        wtype = "スーパーキャノン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ライフル") > 0 _
        Or (Right$(wname, 1) = "銃" And Right$(wname, 2) <> "機銃") _
        Or Right$(wname, 2) = "ガン" _
    Then
        wtype = "ライフル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "対戦車ライフル") > 0 Then
        wtype = "対戦車ライフル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "対物ライフル") > 0 Then
        wtype = "対物ライフル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "消火器") > 0 Then
        wtype = "消火器"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "放水") > 0 Or InStr(wname, "放射器") > 0 Then
        wtype = "放水銃"
        GoTo FoundWeaponType
    End If
    
SkipShootingWeapon:
    
    '対応する武器は見つからなかった
    WeaponInHand = ""
    GoTo SkipWeaponAnimation
    
FoundWeaponType:
    
    '構えている武器を記録
    WeaponInHand = wtype
    
    '表示する準備アニメの種類
    aname = wtype & "準備"
    
    '色
    If InStr(wtype, "ビームサーベル") > 0 _
        Or InStr(wtype, "ビームカッター") > 0 _
        Or wtype = "ビームナイフ" _
        Or wtype = "ライトセイバー" _
    Then
        If InStr(wname, "ビーム") > 0 Then
            aname = aname & " ピンク"
        ElseIf InStr(wname, "プラズマ") > 0 Then
            aname = aname & " グリーン"
        ElseIf InStr(wname, "レーザー") > 0 Then
            aname = aname & " ブルー"
        ElseIf InStr(wname, "ライト") > 0 Then
            aname = aname & " イエロー"
        End If
    End If
    
    '効果音
    If Len(sname) > 0 Then
        aname = aname & " " & sname
    End If
    
    '二刀流
    If double_weapon Then
        aname = aname & " 二刀流"
    End If
    
    '準備アニメ表示
    ShowAnimation aname
    
SkipWeaponAnimation:
    
    '武器の準備アニメをスキップする場合はここから
    
    If with_face_up Then
        'フェイスアップを表示する
        aname = "フェイスアップ準備"
        
        '衝撃を表示？
        If InStrNotNest(wclass, "サ") > 0 Then
            aname = aname & " 衝撃"
        End If
        
        'フェイスアップアニメ表示
        ShowAnimation aname
    End If
End Sub

'武器の名称から武器の種類を判定
Private Function CheckWeaponType(wname As String, wclass As String) As String
    If InStr(wname, "ビーム") > 0 _
        Or InStr(wname, "プラズマ") > 0 _
        Or InStr(wname, "レーザー") > 0 _
        Or InStr(wname, "ブラスター") > 0 _
        Or InStr(wname, "ライト") > 0 _
    Then
        If InStr(wname, "サーベル") > 0 _
            Or InStr(wname, "セイバー") > 0 _
            Or InStr(wname, "ブレード") > 0 _
            Or InStr(wname, "ソード") > 0 _
            Or InStr(wname, "剣") > 0 _
            Or InStr(wname, "刀") > 0 _
        Then
            If InStr(wname, "ハイパー") > 0 Or InStr(wname, "ロング") > 0 _
                Or InStr(wname, "大") > 0 Or InStr(wname, "高") > 0 _
            Then
                CheckWeaponType = "ハイパービームサーベル"
            ElseIf InStr(wname, "セイバー") > 0 Then
                CheckWeaponType = "ライトセイバー"
            Else
                CheckWeaponType = "ビームサーベル"
            End If
            Exit Function
        End If
        
        If InStr(wname, "カッター") > 0 Then
            If InStr(wname, "ハイパー") > 0 Or InStr(wname, "ロング") > 0 _
                Or InStr(wname, "大") > 0 Or InStr(wname, "高") > 0 _
            Then
                CheckWeaponType = "エナジーブレード"
            Else
                CheckWeaponType = "エナジーカッター"
            End If
            Exit Function
        End If
        
        If InStr(wname, "ナイフ") > 0 _
            Or InStr(wname, "ダガー") > 0 _
        Then
            CheckWeaponType = "ビームナイフ"
            Exit Function
        End If
    End If
    
    If InStr(wname, "ナイフ") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "短刀") > 0 Or InStr(wname, "小刀") > 0 _
    Then
        If InStr(wname, "投") > 0 Or InStr(wname, "飛び") > 0 _
            Or Right$(wname, 3) = "スロー" Or Right$(wname, 3) = "スロウ" _
            Or InStrNotNest(wclass, "実") > 0 _
        Then
            CheckWeaponType = "投げナイフ"
        Else
            CheckWeaponType = "ナイフ"
        End If
        Exit Function
    End If
    
    If InStr(wname, "ショートソード") > 0 Or InStr(wname, "短剣") > 0 _
        Or InStr(wname, "スモールソード") > 0 Or InStr(wname, "小剣") > 0 _
    Then
        CheckWeaponType = "ショートソード"
        Exit Function
    End If
    
    If InStr(wname, "グレートソード") > 0 Or InStr(wname, "大剣") > 0 _
        Or InStr(wname, "ハンデッドソード") > 0 Or InStr(wname, "両手剣") > 0 _
    Then
        CheckWeaponType = "大剣"
        Exit Function
    End If
    
    If InStr(wname, "ロングソード") > 0 Or InStr(wname, "長剣") > 0 _
        Or InStr(wname, "バスタードソード") > 0 _
        Or wname = "ソード" _
    Then
        CheckWeaponType = "ソード"
        Exit Function
    End If
    
    If InStr(wname, "手裏剣") > 0 Then
        CheckWeaponType = "手裏剣"
        Exit Function
    End If
        
    If Right$(wname, 1) = "剣" _
        And (Len(wname) <= 3 _
            Or Right$(wname, 2) = "の剣") _
    Then
        If InStr(wname, "ブラック") > 0 Or InStr(wname, "黒") > 0 Then
            CheckWeaponType = "黒剣"
        Else
            CheckWeaponType = "剣"
        End If
        Exit Function
    End If
    
    If InStr(wname, "ソードブレイカー") > 0 Then
        CheckWeaponType = "ソードブレイカー"
        Exit Function
    End If
    
    If InStr(wname, "レイピア") > 0 Then
        CheckWeaponType = "レイピア"
        Exit Function
    End If
    
    If InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 _
        Or InStr(wname, "カットラス") > 0 Or InStr(wname, "三日月刀") > 0 _
    Then
        CheckWeaponType = "シミター"
        Exit Function
    End If
    
    If InStr(wname, "ショーテル") > 0 Then
        CheckWeaponType = "ショーテル"
        Exit Function
    End If
    
    If InStr(wname, "ナギナタ") > 0 Or InStr(wname, "薙刀") > 0 _
        Or InStr(wname, "グレイブ") > 0 _
    Then
        CheckWeaponType = "ナギナタ"
        Exit Function
    End If
    
    If InStr(wname, "竹刀") > 0 Then
        CheckWeaponType = "竹刀"
        Exit Function
    End If
    
    If InStr(wname, "脇差") > 0 Or InStr(wname, "小太刀") > 0 Then
        CheckWeaponType = "脇差"
        Exit Function
    End If
    
    If wname = "刀" Or wname = "日本刀" _
        Or InStr(wname, "太刀") > 0 _
    Then
        CheckWeaponType = "日本刀"
        Exit Function
    End If
    
    If InStr(wname, "忍者刀") > 0 Then
        CheckWeaponType = "忍者刀"
        Exit Function
    End If
    
    If InStr(wname, "十手") > 0 Then
        CheckWeaponType = "十手"
        Exit Function
    End If
    
    If InStr(wname, "青龍刀") > 0 Then
        CheckWeaponType = "青龍刀"
        Exit Function
    End If
    
    If InStr(wname, "トマホーク") > 0 Then
        CheckWeaponType = "トマホーク"
        Exit Function
    End If
    
    If InStr(wname, "アックス") > 0 Or InStr(wname, "斧") > 0 Then
        If InStr(wname, "グレート") > 0 Or InStr(wname, "両") > 0 _
            Or InStr(wname, "バトル") > 0 _
        Then
            CheckWeaponType = "両刃斧"
        Else
            CheckWeaponType = "片刃斧"
        End If
        Exit Function
    End If
    
    If InStr(wname, "サイズ") > 0 Or InStr(wname, "大鎌") > 0 Then
        CheckWeaponType = "大鎌"
        Exit Function
    End If
    
    If InStr(wname, "鎌") > 0 Then
        CheckWeaponType = "鎌"
        Exit Function
    End If
    
    If InStr(wname, "スタッフ") > 0 Or InStr(wname, "杖") > 0 Then
        CheckWeaponType = "杖"
        Exit Function
    End If
    
    If InStr(wname, "棍棒") > 0 Then
        CheckWeaponType = "棍棒"
        Exit Function
    End If
    
    If InStr(wname, "警棒") > 0 Then
        CheckWeaponType = "警棒"
        Exit Function
    End If
    
    If wname = "棒" Then
        CheckWeaponType = "棒"
        Exit Function
    End If
    
    If InStr(wname, "鉄パイプ") > 0 Then
        CheckWeaponType = "鉄パイプ"
        Exit Function
    End If
    
    If InStr(wname, "スタンロッド") > 0 Then
        CheckWeaponType = "スタンロッド"
        Exit Function
    End If
    
    If InStr(wname, "スパナ") > 0 Then
        CheckWeaponType = "スパナ"
        Exit Function
    End If
    
    If InStr(wname, "メイス") > 0 Then
        CheckWeaponType = "メイス"
        Exit Function
    End If
    
    
    If InStr(wname, "パンチ") > 0 Or InStr(wname, "ナックル") > 0 Then
        'ハンマーパンチ等がハンマーにひっかかると困るため、ここで判定
        If InStrNotNest(wclass, "実") > 0 Then
            CheckWeaponType = "ロケットパンチ"
        End If
        Exit Function
    End If
    
    
    If InStr(wname, "ウォーハンマー") > 0 Then
        CheckWeaponType = "ウォーハンマー"
        Exit Function
    End If
    
    If InStr(wname, "木槌") > 0 Then
        CheckWeaponType = "木槌"
        Exit Function
    End If
    
    If InStr(wname, "ピコピコハンマー") > 0 Then
        CheckWeaponType = "ピコピコハンマー"
        Exit Function
    End If
    
    If InStr(wname, "ハンマー") > 0 Then
        If InStrNotNest(wclass, "実") > 0 Then
            CheckWeaponType = "鎖鉄球"
        Else
            CheckWeaponType = "ハンマー"
        End If
        Exit Function
    End If
    
    If InStr(wname, "槌") > 0 Then
        CheckWeaponType = "ハンマー"
        Exit Function
    End If
    
    If Right$(wname, 3) = "モール" Then
        CheckWeaponType = "モール"
        Exit Function
    End If
    
    If Right$(wname, 2) = "ムチ" Or InStr(wname, "鞭") > 0 _
        Or InStr(wname, "ウィップ") > 0 _
    Then
        CheckWeaponType = "鞭"
        Exit Function
    End If
    
    If wname = "サイ" Then
        CheckWeaponType = "サイ"
        Exit Function
    End If
    
    If InStr(wname, "トンファー") > 0 Then
        CheckWeaponType = "トンファー"
        Exit Function
    End If
    
    If InStr(wname, "鉄の爪") > 0 Then
        CheckWeaponType = "クロー"
        Exit Function
    End If
    
    If InStr(wname, "ハルバード") > 0 Then
        CheckWeaponType = "ハルバード"
        Exit Function
    End If
    
    
    If InStr(wname, "モーニングスター") > 0 Then
        CheckWeaponType = "モーニングスター"
        Exit Function
    End If
    
    If InStr(wname, "フレイル") > 0 Then
        CheckWeaponType = "フレイル"
        Exit Function
    End If
    
    If InStr(wname, "鎖鉄球") > 0 Then
        CheckWeaponType = "鎖鉄球"
        Exit Function
    End If
    
    If InStr(wname, "分銅") > 0 Then
        CheckWeaponType = "分銅"
        Exit Function
    End If
    
    If InStr(wname, "ヌンチャク") > 0 Then
        CheckWeaponType = "ヌンチャク"
        Exit Function
    End If
    
    If InStr(wname, "三節棍") > 0 Then
        CheckWeaponType = "三節棍"
        Exit Function
    End If
    
    If InStr(wname, "チェーン") > 0 Then
        CheckWeaponType = "チェーン"
        Exit Function
    End If
    
    
    If InStr(wname, "ブーメラン") > 0 Then
        CheckWeaponType = "ブーメラン"
        Exit Function
    End If
    
    If InStr(wname, "チャクラム") > 0 Then
        CheckWeaponType = "チャクラム"
        Exit Function
    End If
    
    If InStr(wname, "ソーサー") > 0 Then
        CheckWeaponType = "ソーサー"
        Exit Function
    End If
    
    If InStr(wname, "クナイ") > 0 Then
        CheckWeaponType = "クナイ"
        Exit Function
    End If
    
    If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 Then
        CheckWeaponType = "石"
        Exit Function
    End If
    
    If InStr(wname, "岩") > 0 Then
        CheckWeaponType = "岩"
        Exit Function
    End If
    
    If InStr(wname, "鉄球") > 0 Then
        CheckWeaponType = "鉄球"
        Exit Function
    End If
    
    If InStr(wname, "手榴弾") > 0 Then
        CheckWeaponType = "手榴弾"
        Exit Function
    End If
    
    If InStr(wname, "ポテトスマッシャー") > 0 Then
        CheckWeaponType = "ポテトスマッシャー"
        Exit Function
    End If
    
    If InStr(wname, "ダイナマイト") > 0 Then
        CheckWeaponType = "ダイナマイト"
        Exit Function
    End If
    
    If InStr(wname, "爆弾") > 0 Then
        If InStr(wname, "投げ") > 0 Then
            CheckWeaponType = "爆弾"
            Exit Function
        End If
    End If
    
    If InStr(wname, "火炎瓶") > 0 Then
        CheckWeaponType = "火炎瓶"
        Exit Function
    End If
    
    If InStr(wname, "ネット") > 0 Or InStr(wname, "網") > 0 Then
        CheckWeaponType = "ネット"
        Exit Function
    End If
    
    If InStr(wname, "手錠") > 0 Then
        CheckWeaponType = "ネット"
        Exit Function
    End If
    
    If Right$(wname, 2) = "コマ" Then
        CheckWeaponType = "コマ"
        Exit Function
    End If
    
    If InStr(wname, "札") > 0 Then
        CheckWeaponType = "お札"
        Exit Function
    End If
    
    
    If InStr(wname, "リボン") > 0 Then
        CheckWeaponType = "リボン"
        Exit Function
    End If
    
    If InStr(wname, "フープ") > 0 Then
        CheckWeaponType = "フープ"
        Exit Function
    End If
    
    
    If InStr(wname, "カタログ") > 0 Then
        CheckWeaponType = "カタログ"
        Exit Function
    End If
    
    If InStr(wname, "フライパン") > 0 Then
        CheckWeaponType = "フライパン"
        Exit Function
    End If
    
    If InStr(wname, "トンボ") > 0 Then
        CheckWeaponType = "トンボ"
        Exit Function
    End If
    
    If InStr(wname, "モップ") > 0 Then
        CheckWeaponType = "モップ"
        Exit Function
    End If
    
    If InStr(wname, "唐傘") > 0 Then
        CheckWeaponType = "唐傘"
        Exit Function
    End If
    
    If InStr(wname, "金属バット") > 0 Then
        CheckWeaponType = "金属バット"
        Exit Function
    End If
    
    If InStr(wname, "釘バット") > 0 Then
        CheckWeaponType = "釘バット"
        Exit Function
    End If
    
    If Right$(wname, 3) = "バット" Then
        If InStr(wname, "ヘッドバット") = 0 Then
            CheckWeaponType = "バット"
            Exit Function
        End If
    End If
    
    If InStr(wname, "扇子") > 0 Then
        CheckWeaponType = "扇子"
        Exit Function
    End If
    
    If InStr(wname, "ギター") > 0 Then
        CheckWeaponType = "ギター"
        Exit Function
    End If
    
    If InStr(wname, "ハリセン") > 0 Then
        CheckWeaponType = "ハリセン"
        Exit Function
    End If
    
    If wname = "ゴルフドライバー" Then
        CheckWeaponType = "ゴルフドライバー"
        Exit Function
    End If
    
    
    If InStr(wname, "トライデント") > 0 Or InStr(wname, "三叉槍") > 0 _
        Or InStr(wname, "ジャベリン") > 0 _
    Then
        CheckWeaponType = "トライデント"
        Exit Function
    End If
    
    If InStr(wname, "スピア") > 0 Then
        CheckWeaponType = "スピア"
        Exit Function
    End If
    
    If InStr(wname, "槍") > 0 Then
        CheckWeaponType = "和槍"
        Exit Function
    End If
    
    If InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 Then
        CheckWeaponType = "ランス"
        Exit Function
    End If
    
    If InStr(wname, "パイク") > 0 Then
        CheckWeaponType = "ランス"
        Exit Function
    End If
    
    If InStr(wname, "エストック") > 0 Then
        CheckWeaponType = "エストック"
        Exit Function
    End If
    
    If wname = "ロッド" Then
        CheckWeaponType = "ロッド"
        Exit Function
    End If
    
    If InStr(wname, "ドリル") > 0 Then
        CheckWeaponType = "ドリル"
        Exit Function
    End If
End Function

'武器準備時の効果音
Public Sub PrepareWeaponSound(u As Unit, ByVal w As Integer)
Dim wname As String, wclass As String

    'フラグをクリア
    IsWavePlayed = False
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    If InStrNotNest(wclass, "武") > 0 Or InStrNotNest(wclass, "突") > 0 Then
        If InStr(wname, "ビーム") > 0 _
            Or InStr(wname, "プラズマ") > 0 _
            Or InStr(wname, "レーザー") > 0 _
            Or InStr(wname, "ブラスター") > 0 _
            Or InStr(wname, "高周波") > 0 _
            Or InStr(wname, "電磁") > 0 _
            Or wname = "セイバー" _
            Or wname = "ライトセイバー" _
            Or wname = "ランサー" _
        Then
            PlayWave "BeamSaber.wav"
        End If
    End If
    
    'フラグをクリア
    IsWavePlayed = False
End Sub


'武器使用時の特殊効果
Public Sub AttackEffect(u As Unit, ByVal w As Integer)
    '右クリック中は特殊効果をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    If BattleAnimation Then
        AttackAnimation u, w
    Else
        AttackSound u, w
    End If
End Sub

'武器使用時のアニメーション
Public Sub AttackAnimation(u As Unit, ByVal w As Integer)
Dim wname As String, wclass As String, wtype As String, wtype0 As String
Dim aname As String, bmpname As String, cname As String, cname0 As String
Dim sname As String, sname0 As String
Dim attack_times As Integer
Dim double_weapon As Boolean
Dim double_attack As Boolean
Dim combo_attack As Boolean
Dim is_handy_weapon As Boolean
Dim i As Integer

    '戦闘アニメ非自動選択オプション
    If IsOptionDefined("戦闘アニメ非自動選択") Then
        ShowAnimation "デフォルト攻撃"
        Exit Sub
    End If
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '二刀流？
    If InStr(wname, "ダブル") > 0 Or InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "デュアル") > 0 _
        Or InStr(wname, "双") > 0 Or InStr(wname, "二刀") > 0 _
        Or InStr(wname, "２連") > 0 Or InStr(wname, "二連") > 0 _
        Or InStr(wname, "連装") > 0 _
    Then
        double_weapon = True
    End If
    
    '連続攻撃？
    If InStr(wname, "ダブル") > 0 Or InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "コンビネーション") > 0 _
        Or InStr(wname, "コンボ") > 0 _
        Or InStr(wname, "連") > 0 Or InStrNotNest(wclass, "連") > 0 _
    Then
        double_attack = True
    End If
    
    '乱打？
    If InStr(wname, "乱打") > 0 Or InStr(wname, "乱舞") > 0 _
        Or InStr(wname, "乱れ") > 0 Or InStr(wname, "百烈") > 0 _
        Or (Right$(wname, 4) = "ラッシュ" _
            And InStr(wname, "クラッシュ") = 0 _
            And InStr(wname, "スラッシュ") = 0 _
            And InStr(wname, "スプラッシュ") = 0 _
            And InStr(wname, "フラッシュ") = 0) _
    Then
        combo_attack = True
    End If
    
    'これから武器の種類を判定
    
    'まずは白兵戦用武器の判定
    If InStrNotNest(wclass, "武") = 0 _
        And InStrNotNest(wclass, "突") = 0 _
        And InStrNotNest(wclass, "接") = 0 _
        And InStrNotNest(wclass, "格") = 0 _
    Then
        GoTo SkipInfightWeapon
    End If
    
    '投擲武器を除く
    If InStr(wname, "投") > 0 Or InStr(wname, "飛び") > 0 _
        Or Right$(wname, 3) = "スロー" Or Right$(wname, 3) = "スロウ" _
        Or InStrNotNest(wclass, "実") > 0 _
    Then
        GoTo SkipInfightWeapon
    End If
    
    '移動マップ攻撃
    If InStrNotNest(wclass, "Ｍ移") > 0 Then
        wtype = "ＭＡＰ移動タックル"
        GoTo FoundWeaponType
    End If
    
    '突撃系(武器を構えて突進する)
    
    If InStr(wname, "突撃") > 0 Or InStr(wname, "突進") > 0 _
        Or InStr(wname, "チャージ") > 0 _
    Then
        Select Case WeaponInHand
            Case ""
                '該当せず
            Case Else
                wtype = WeaponInHand & "突撃"
                GoTo FoundWeaponType
        End Select
    End If
    
    '打撃系の攻撃
    
    If InStr(wname, "拳法") > 0 _
        Or Right$(wname, 2) = "アーツ" _
        Or Right$(wname, 5) = "ストライク" _
    Then
        wtype = "連打"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "触手") > 0 Or InStr(wname, "触腕") > 0 Then
        wtype = "白兵連撃"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "パンチ") > 0 Or InStr(wname, "チョップ") > 0 _
        Or InStr(wname, "ナックル") > 0 Or InStr(wname, "ブロー") > 0 _
        Or InStr(wname, "拳") > 0 Or InStr(wname, "掌") > 0 _
        Or InStr(wname, "打") > 0 Or InStr(wname, "勁") > 0 _
        Or InStr(wname, "殴") > 0 _
        Or Right$(wname, 1) = "手" Or Right$(wname, 1) = "腕" _
    Then
        If combo_attack Then
            wtype = "乱打"
        ElseIf double_attack Then
            wtype = "連打"
        ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
            wtype = "アッパー"
        Else
            wtype = "打突"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "格闘") > 0 Or InStr(wname, "怪力") > 0 Then
        wtype = "格闘"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "タックル") > 0 Or InStr(wname, "体当") > 0 _
        Or InStr(wname, "チャージ") > 0 Or InStr(wname, "ぶちかまし") > 0 _
        Or InStr(wname, "かみつき") > 0 _
    Then
        wtype = "タックル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "キック") > 0 Or InStr(wname, "蹴") > 0 _
        Or InStr(wname, "脚") > 0 Or Right$(wname, 1) = "足" _
    Then
        wtype = "キック"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ヘッドバット") > 0 Or InStr(wname, "頭突") > 0 Then
        wtype = "ヘッドバット"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "アッパー") > 0 Then
        wtype = "アッパー"
        GoTo FoundWeaponType
    End If
    
    '振って攻撃する武器
    
    If InStr(wname, "ソード") > 0 Or InStr(wname, "剣") > 0 _
        Or InStr(wname, "ナイフ") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 _
        Or InStr(wname, "カットラス") > 0 Or InStr(wname, "カッター") > 0 _
        Or Right$(wname, 2) = "ムチ" Or InStr(wname, "鞭") > 0 _
        Or InStr(wname, "ウィップ") > 0 _
        Or InStr(wname, "ハンマー") > 0 Or InStr(wname, "ロッド") > 0 _
        Or InStr(wname, "クロー") > 0 Or InStr(wname, "爪") > 0 _
        Or InStr(wname, "ひっかき") > 0 _
        Or InStr(wname, "アーム") > 0 _
        Or Right$(wname, 1) = "尾" _
    Then
        If combo_attack Then
            wtype = "白兵乱撃"
        ElseIf double_attack Then
            wtype = "白兵連撃"
        ElseIf InStr(wname, "回転") > 0 Then
            wtype = "白兵回転"
        ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
            wtype = "振り上げ"
        Else
            wtype = "白兵武器"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "刀") > 0 Or InStr(wname, "斬") > 0 _
        Or InStr(wname, "ブレード") > 0 Or InStr(wname, "刃") > 0 _
        Or InStr(wname, "アックス") > 0 Or InStr(wname, "斧") > 0 _
        Or InStr(wname, "カット") > 0 Or InStr(wname, "カッター") > 0 _
        Or InStr(wname, "スラッシュ") > 0 _
        Or InStr(wname, "居合") > 0 _
    Then
        If combo_attack Then
            wtype = "白兵乱撃"
        ElseIf double_attack Then
            wtype = "ダブル斬撃"
        ElseIf InStr(wname, "回転") > 0 Then
            wtype = "白兵回転"
        ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
            wtype = "振り上げ"
        ElseIf InStr(wname, "ブラック") > 0 Or InStr(wname, "黒") > 0 Then
            wtype = "黒斬撃"
        Else
            wtype = "斬撃"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "サイズ") > 0 Or InStr(wname, "鎌") > 0 _
        Or InStr(wname, "グレイブ") > 0 Or InStr(wname, "ナギナタ") > 0 _
    Then
        wtype = "振り下ろし"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ショーテル") > 0 Then
        wtype = "ダブル斬撃"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "円月殺法") > 0 Then
        wtype = "円月殺法"
        GoTo FoundWeaponType
    End If
    
    '大きく振りまわす武器
    
    If InStr(wname, "鎖鉄球") > 0 Then
        wtype = "鎖鉄球"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "モーニングスター") > 0 Then
        wtype = "モーニングスター"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "フレイル") > 0 Then
        wtype = "フレイル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "分銅") > 0 Then
        wtype = "分銅"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "チェーン") > 0 And InStr(wname, "チェーンソー") = 0 Then
        wtype = "チェーン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ヌンチャク") > 0 Then
        wtype = "ヌンチャク"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "三節棍") > 0 Then
        wtype = "三節棍"
        GoTo FoundWeaponType
    End If
    
    '突き刺す武器
    
    If InStr(wname, "スピア") > 0 Or InStr(wname, "槍") > 0 _
        Or InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 _
        Or InStr(wname, "トライデント") > 0 _
        Or InStr(wname, "ジャベリン") > 0 _
        Or InStr(wname, "レイピア") > 0 _
        Or wname = "ロッド" _
    Then
        If combo_attack Then
            wtype = "乱突"
        ElseIf double_attack Then
            wtype = "連突"
        Else
            wtype = "刺突"
        End If
        GoTo FoundWeaponType
    End If
    
    '特殊な格闘武器
    
    If InStr(wname, "ドリル") > 0 Then
        wtype = "ドリル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "チェーンソー") > 0 Then
        wtype = "チェーンソー"
        GoTo FoundWeaponType
    End If
    
    '詳細が分からなかった武器
    If InStrNotNest(wclass, "武") > 0 Then
        '装備しているアイテムから武器を検索
        For i = 1 To u.CountItem
            With u.Item(i)
                If .Activated _
                    And (.Part = "両手" _
                        Or .Part = "片手" _
                        Or .Part = "武器") _
                Then
                    wtype = CheckWeaponType(.Nickname, "")
                    If wtype = "" Then
                        wtype = CheckWeaponType(.Class0, "")
                    End If
                    Exit For
                End If
            End With
        Next
        Select Case wtype
            Case "スピア", "ランス", "トライデント", "和槍", _
                "エストック"
                If combo_attack Then
                    wtype = "乱突"
                ElseIf double_attack Then
                    wtype = "連突"
                Else
                    wtype = "刺突"
                End If
            Case Else
                If combo_attack Then
                    wtype = "白兵乱撃"
                ElseIf double_attack Then
                    wtype = "白兵連撃"
                ElseIf InStr(wname, "回転") > 0 Then
                    wtype = "白兵回転"
                ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
                    wtype = "振り上げ"
                Else
                    wtype = "白兵武器"
                End If
        End Select
        GoTo FoundWeaponType
    End If
    
    '詳細が分からなかった近接技
    If InStrNotNest(wclass, "突") > 0 _
        And InStrNotNest(wclass, "接") > 0 _
    Then
        wtype = "格闘"
        GoTo FoundWeaponType
    End If
    
SkipInfightWeapon:
    
    If InStrNotNest(wclass, "実") = 0 Then
        GoTo SkipThrowingWeapon
    End If
    
    '投擲武器
    '(真っ直ぐ飛ぶ武器)
    
    If InStr(wname, "槍") > 0 Or InStr(wname, "スピア") > 0 Then
        wtype = "投げ槍"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ナイフ") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "クナイ") > 0 Or InStr(wname, "苦無") > 0 _
    Then
        wtype = "投げナイフ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 Then
        wtype = "石"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "岩") > 0 Then
        wtype = "岩"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "鉄球") > 0 Then
        wtype = "鉄球"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ダイナマイト") > 0 Then
        wtype = "ダイナマイト"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "爆弾") > 0 Then
        If InStr(wname, "投げ") > 0 Then
            wtype = "爆弾"
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "ハンドグレネード") > 0 Then
        wtype = "グレネード投げ"
        GoTo FoundWeaponType
    End If
    
    '(回転しながら飛ぶ武器)
    
    If InStr(wname, "トマホーク") > 0 Then
        wtype = "トマホーク投擲"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "アックス") > 0 Or InStr(wname, "斧") > 0 Then
        If InStr(wname, "グレート") > 0 Or InStr(wname, "両") > 0 _
            Or InStr(wname, "バトル") > 0 _
        Then
            wtype = "両刃斧投擲"
        Else
            wtype = "片刃斧投擲"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "サイズ") > 0 Or InStr(wname, "大鎌") > 0 Then
        wtype = "大鎌投擲"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "鎌") > 0 Then
        wtype = "鎌投擲"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ブーメラン") > 0 Then
        wtype = "ブーメラン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "チャクラム") > 0 Then
        wtype = "チャクラム"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "手裏剣") > 0 Then
        wtype = "手裏剣"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "手榴弾") > 0 Then
        wtype = "手榴弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ポテトマッシャー") > 0 Then
        wtype = "ポテトマッシャー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "火炎瓶") > 0 Then
        wtype = "火炎瓶"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "手錠") > 0 Then
        wtype = "手錠"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "フープ") > 0 Then
        wtype = "フープ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "扇子") > 0 Then
        wtype = "扇子"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "札") > 0 Then
        wtype = "お札"
        GoTo FoundWeaponType
    End If
    
    '弓矢
    
    If InStr(wname, "弓") > 0 _
        Or InStr(wname, "ショートボウ") > 0 _
        Or InStr(wname, "ロングボウ") > 0 _
    Then
        wtype = "弓矢"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "矢") > 0 _
        Or InStr(wname, "アロー") > 0 _
    Then
        If CountAttack0(u, w) > 1 Then
            wtype = "矢連射"
        Else
            wtype = "矢"
        End If
        GoTo FoundWeaponType
    End If
    
    '遠距離系の格闘武器
    
    '振る武器
    
    If Right$(wname, 2) = "ムチ" Or InStr(wname, "鞭") > 0 _
        Or InStr(wname, "ウィップ") > 0 _
    Then
        wtype = "白兵武器"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "触手") > 0 Or InStr(wname, "触腕") > 0 Then
        wtype = "白兵連撃"
        GoTo FoundWeaponType
    End If
    
    '大きく振りまわす武器
    
    If InStr(wname, "鎖鉄球") > 0 Or InStr(wname, "ハンマー") > 0 Then
        wtype = "鎖鉄球"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "分銅") > 0 Then
        wtype = "分銅"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "チェーン") > 0 Then
        wtype = "チェーン"
        GoTo FoundWeaponType
    End If
    
    'その他格闘系
    
    If InStr(wname, "パンチ") > 0 Or InStr(wname, "ナックル") > 0 Then
        wtype = "ロケットパンチ"
        GoTo FoundWeaponType
    End If
    
SkipThrowingWeapon:
    
    'これより通常射撃攻撃
    
    'まずは手持ち武器の判定
    is_handy_weapon = True
    
    '光線系の攻撃かどうかを判定する
    
    If IsBeamWeapon(wname, wclass, cname) Then
        wtype = "ビーム"
        
        '実弾系武器判定をスキップ
        GoTo SkipNormalHandWeapon
    End If
    
    '手に持つ射撃武器
    
    '(大き目の実弾を飛ばすタイプ)
    
    If InStr(wname, "クロスボウ") > 0 _
        Or InStr(wname, "ボウガン") > 0 _
    Then
        wtype = "クロスボウ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "バズーカ") > 0 Then
        wtype = "バズーカ"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "対戦車ライフル") > 0 Then
        wtype = "対戦車ライフル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "対物ライフル") > 0 Then
        wtype = "対物ライフル"
        GoTo FoundWeaponType
    End If
    
    '(小さな弾を単発で撃つタイプの手持ち火器)
    
    If InStr(wname, "ピストル") > 0 Or InStr(wname, "拳銃") > 0 Then
        wtype = "ピストル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "リボルバー") > 0 Or InStr(wname, "リボルヴァー") > 0 Then
        wtype = "リボルバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ライフル") > 0 _
        Or (Right$(wname, 1) = "銃" And Right$(wname, 2) <> "機銃") _
    Then
        wtype = "ライフル"
        GoTo FoundWeaponType
    End If
    
    '(連射するタイプの手持ち火器)
    
    If InStr(wname, "サブマシンガン") > 0 Then
        wtype = "サブマシンガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "マシンガン") > 0 Or InStr(wname, "機関銃") > 0 Then
        If InStr(wname, "ヘビー") > 0 Or InStr(wname, "重") > 0 Then
            wtype = "ヘビーマシンガン"
        Else
            wtype = "マシンガン"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ガトリング") > 0 Then
        wtype = "ガトリング"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ショットガン") > 0 Or InStr(wname, "ライアットガン") > 0 Then
        wtype = "ショットガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "レールガン") > 0 _
        Or InStr(wname, "リニアガン") > 0 _
    Then
        PlayWave "Thunder.wav"
        Sleep 300
        wtype = "キャノン砲"
        GoTo FoundWeaponType
    End If
    
    'よく分からないのでライフル扱い
    If Right$(wname, 2) = "ガン" Then
        wtype = "ライフル"
        GoTo FoundWeaponType
    End If
    
    GoTo SkipHandWeapon
    
SkipNormalHandWeapon:
    
    '(手持ちのビーム攻撃)
    
    If InStr(wname, "ライフル") > 0 Or InStr(wname, "ガン") > 0 _
        Or InStr(wname, "ピストル") > 0 Or InStr(wname, "バズーカ") > 0 _
        Or (Right$(wname, 1) = "銃" And Right$(wname, 2) <> "機銃") _
    Then
        If InStrNotNest(wclass, "Ｍ") > 0 Then
            wtype = "ＭＡＰバスタービームライフル"
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 _
            Or InStr(wname, "大") > 0 _
            Or Left$(wname, 2) = "ギガ" _
        Then
            wtype = "バスタービームライフル"
        ElseIf InStr(wname, "メガ") > 0 _
            Or InStr(wname, "ハイ") > 0 _
            Or InStr(wname, "バズーカ") > 0 _
        Then
            If double_weapon Then
                wtype = "ダブルビームランチャー"
            Else
                wtype = "ビームランチャー"
            End If
            If InStr(wname, "ライフル") > 0 Then
                bmpname = "Weapon\EFFECT_BusterRifle01.bmp"
            End If
        ElseIf CountAttack0(u, w) >= 4 Then
            wtype = "レーザーマシンガン"
            bmpname = "Weapon\EFFECT_Rifle01.bmp"
        ElseIf InStr(wname, "ピストル") > 0 _
            Or InStr(wname, "ミニ") > 0 _
            Or InStr(wname, "小") > 0 _
        Then
            wtype = "レーザーガン"
        Else
            If double_weapon Then
                wtype = "ダブルビームライフル"
            Else
                wtype = "ビームライフル"
            End If
        End If
        
        If wtype = "バスター" Then
            wtype0 = "粒子集中"
        End If
        
        GoTo FoundWeaponType
    End If
    
SkipHandWeapon:
    
    '内蔵型射撃武器
    is_handy_weapon = False
    
    '(大型の実弾火器)
    
    If InStr(wname, "ミサイル") > 0 Or InStr(wname, "ロケット") > 0 Then
        wtype = "ミサイル"
        
        If InStr(wname, "ドリル") > 0 Then
            wtype = "ドリルミサイル"
            GoTo FoundWeaponType
        End If
        
        attack_times = CountAttack0(u, w)
        
        If InStr(wname, "大型") > 0 Or InStr(wname, "ビッグ") > 0 _
            Or InStr(wname, "対艦") > 0 _
        Then
            wtype = "スーパーミサイル"
            attack_times = 1
        ElseIf InStr(wname, "小型") > 0 Then
            wtype = "小型ミサイル"
        ElseIf InStr(wname, "ランチャー") > 0 Or InStr(wname, "ポッド") > 0 _
            Or InStr(wname, "マイクロ") > 0 Or InStr(wname, "スプレー") > 0 _
        Then
            wtype = "小型ミサイル"
            attack_times = 6
        End If
        
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "グレネード") > 0 _
        Or InStr(wname, "ディスチャージャー") > 0 _
    Then
        wtype = "グレネード"
        
        attack_times = CountAttack0(u, w)
        
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "シュツルムファウスト") > 0 Then
        wtype = "実弾発射"
        
        bmpname = "Bullet\EFFECT_BazookaBullet01.bmp"
        attack_times = CountAttack0(u, w)
        
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "爆弾") > 0 Or InStr(wname, "爆撃") > 0 _
        Or InStr(wname, "爆雷") > 0 _
    Then
        If u.Weapon(w).MaxRange = 1 Then
            wtype = "投下爆弾"
        Else
            wtype = "グレネード"
            attack_times = CountAttack0(u, w)
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "光子魚雷") > 0 Then
        wtype = "光子魚雷"
        GoTo FoundWeaponType
    End If
    
    '(怪光線系)
    
    If InStr(wname, "怪光線") > 0 Then
        wtype = "怪光線"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "破壊光線") > 0 Then
        wtype = "破壊光線"
        GoTo FoundWeaponType
    End If
    
    '特殊な物質を出す武器
    
    If InStr(wname, "消火") > 0 Then
        wtype = "消火器"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "放水") > 0 Or InStr(wname, "水流") > 0 Then
        wtype = "放水銃"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "水鉄砲") > 0 Or Right$(wname, 1) = "液" Then
        wtype = "実弾発射"
        sname = "Bow.wav"
        If InStr(wname, "毒") > 0 Or InStr(wname, "毒") > 0 Then
            bmpname = "Bullet\EFFECT_Venom01.bmp"
        Else
            bmpname = "Bullet\EFFECT_WaterShot01.bmp"
        End If
        GoTo FoundWeaponType
    End If
    
    '物理現象系の攻撃(炎や光など)
    
    If InStr(wname, "重力") > 0 Or InStr(wname, "グラビ") > 0 _
         Or InStr(wname, "ブラックホール") > 0 _
         Or InStr(wname, "縮退") > 0 _
    Then
        wtype = "重力弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "落雷") > 0 Or Right$(wname, 2) = "稲妻" Then
        wtype = "落雷"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "雷") > 0 Or InStr(wname, "ライトニング") > 0 _
         Or InStr(wname, "サンダー") > 0 _
    Then
        If InStrNotNest(wclass, "実") = 0 Then
            If u.Weapon(w).MaxRange = 1 Then
                wtype = "破壊光線"
                sname = "Thunder.wav"
            Else
                wtype = "落雷"
            End If
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "電撃") > 0 Or InStr(wname, "電流") > 0 _
        Or InStr(wname, "エレクト") > 0 _
    Then
        wtype = "破壊光線"
        sname = "Thunder.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "エネルギー弾") > 0 Then
        wtype = "球電"
        sname = "Beam.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "泡") > 0 Or InStr(wname, "バブル") > 0 Then
        wtype = "泡"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "音波") > 0 Or InStr(wname, "サウンド") > 0 _
        Or InStr(wname, "ソニック") > 0 _
        Or (InStrNotNest(wclass, "音") > 0 And InStr(wname, "ショック") > 0) _
        Or InStr(wname, "ウェーブ") > 0 _
        Or InStr(wname, "叫び") > 0 _
        Or (InStrNotNest(wclass, "音") > 0 And InStr(wname, "咆哮") > 0) _
    Then
        wtype = "音波"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "歌") > 0 Or InStr(wname, "ソング") > 0 Then
        wtype = "音符"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "針") > 0 Or InStr(wname, "ニードル") > 0 Then
        wtype = "ニードル"
        If CountAttack0(u, w) > 1 Then
            wtype = "ニードル連射"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "津波") > 0 _
        Or InStr(wname, "ダイダル") > 0 _
    Then
        wtype = "津波"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "コメット") > 0 Then
        wtype = "流星"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "メテオ") > 0 Or InStr(wname, "隕石") > 0 Then
        wtype = "隕石"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "竜巻") > 0 Or InStr(wname, "渦巻") > 0 _
        Or InStr(wname, "トルネード") > 0 Or InStr(wname, "サイクロン") > 0 _
    Then
        wtype = "竜巻"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "つらら") > 0 Then
        wtype = "氷弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "つぶて") > 0 Then
        wtype = "岩弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "吹雪") > 0 Or InStr(wname, "ブリザード") > 0 _
        Or InStr(wname, "アイスストーム") > 0 _
    Then
        wtype = "吹雪"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ストーム") > 0 Or InStr(wname, "ハリケーン") > 0 _
        Or InStr(wname, "タイフーン") > 0 _
        Or InStr(wname, "台風") > 0 Or InStr(wname, "嵐") > 0 _
    Then
        wtype = "強風"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ウィンド") > 0 Or InStr(wname, "ウインド") > 0 _
        Or InStr(wname, "風") > 0 _
    Then
        wtype = "風"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "煙") > 0 Or InStr(wname, "スモーク") > 0 _
        Or Right$(wname, 2) = "ガス" Or Right$(wname, 1) = "霧" _
        Or InStr(wname, "胞子") > 0 _
    Then
        wtype = "煙"
        If InStr(wname, "毒") > 0 Or InStrNotNest(wclass, "毒") > 0 Then
            cname = "緑"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "火炎弾") > 0 Then
        wtype = "火炎弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "火炎放射") > 0 _
        Or Right$(wname, 2) = "火炎" _
    Then
        wtype = "火炎放射"
        sname = "AntiShipMissile.wav"
        GoTo FoundWeaponType
    End If
    
    If Right$(wname, 5) = "ファイアー" _
        Or Right$(wname, 5) = "ファイヤー" _
        Or Right$(wname, 4) = "ファイア" _
        Or Right$(wname, 4) = "ファイヤ" _
    Then
        If InStrNotNest(wclass, "実") = 0 And Left$(wname, 2) <> "フル" Then
            If InStrNotNest(wclass, "術") > 0 Then
                wtype = "炎投射"
            Else
                wtype = "火炎放射"
                sname = "AntiShipMissile.wav"
            End If
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "息") > 0 Or Right$(wname, 3) = "ブレス" Then
        If InStrNotNest(wclass, "実") = 0 Then
            wtype = "火炎放射"
            sname = "Breath.wav"
            
            Select Case SpellColor(wname, wclass)
                Case "赤", "青", "黄", "緑", "白", "黒"
                    cname = SpellColor(wname, wclass)
            End Select
            
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "エネルギー波") > 0 Then
        wtype = "波動放射"
        sname = "Beam.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "衝撃") > 0 Then
        wtype = "波動放射"
        cname = "白"
        sname = "Bazooka.wav"
        GoTo FoundWeaponType
    End If
    
    '霊的、魔法的な攻撃
    
    If InStr(wname, "気弾") > 0 Then
        wtype = "気弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ソニックブレード") > 0 Then
        wtype = "気斬"
        GoTo FoundWeaponType
    End If
    
    If u.IsSpellWeapon(w) Or InStrNotNest(wclass, "魔") > 0 Then
'        wtype = "魔法放射"
'        cname = SpellColor(wname, wclass)
        wtype = "デフォルト"
        sname = "Whiz.wav"
        GoTo FoundWeaponType
    End If
    
    '(ビーム攻撃)
    
    If wtype = "ビーム" Then
        If InStrNotNest(wclass, "Ｍ") > 0 Then
            wtype = "ＭＡＰビーム"
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 _
            Or InStr(wname, "大") > 0 _
            Or Left$(wname, 2) = "ギガ" _
        Then
            wtype = "大ビーム"
        ElseIf InStr(wname, "メガ") > 0 _
            Or InStr(wname, "ハイ") > 0 _
        Then
            wtype = "中ビーム"
        ElseIf CountAttack0(u, w) >= 4 _
            Or InStr(wname, "対空") > 0 _
        Then
            wtype = "ニードルレーザー連射"
        ElseIf InStr(wname, "ミニ") > 0 _
            Or InStr(wname, "小") > 0 _
        Then
            wtype = "ニードルレーザー"
        ElseIf InStr(wname, "ランチャー") > 0 _
            Or InStr(wname, "キャノン") > 0 _
            Or InStr(wname, "カノン") > 0 _
            Or InStr(wname, "砲") > 0 _
        Then
            wtype = "中ビーム"
        Else
            wtype = "小ビーム"
        End If
        
        If wtype = "大ビーム" Then
            wtype0 = "粒子集中"
        End If
        
        Select Case wtype
            Case "小ビーム", "中ビーム"
                If double_weapon Then
                    wtype = "２連" & wtype
                End If
        End Select
        
        If InStr(wname, "拡散") > 0 _
            Or InStr(wname, "放射") > 0 _
            Or InStr(wname, "ホーミング") > 0 _
            Or InStr(wname, "誘導") > 0 _
        Then
            wtype = "拡散ビーム"
        End If
        
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "光線") > 0 Then
        wtype = "怪光線"
        GoTo FoundWeaponType
    End If
    
    '(小型で連射する火器)
    
    If InStr(wname, "バルカン") > 0 Then
        wtype = "バルカン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "機銃") > 0 Or InStr(wname, "機関砲") > 0 Then
        wtype = "機関砲"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "チェーンガン") > 0 _
        Or InStr(wname, "ガンランチャー") > 0 _
    Then
        wtype = "内蔵ガトリング"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "マシンキャノン") > 0 _
        Or InStr(wname, "オートキャノン") > 0 _
        Or InStr(wname, "速射砲") > 0 _
    Then
        wtype = "重機関砲"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ベアリング") > 0 Or InStr(wname, "クレイモア") > 0 Then
        wtype = "ベアリング"
        GoTo FoundWeaponType
    End If
    
    '(オールレンジ攻撃)
    
    If InStr(wname, "有線") > 0 Then
        wtype = "２ＷＡＹ射出"
        GoTo FoundWeaponType
    End If
    
    '汎用的な「砲」の指定は最後に判定
    If InStr(wname, "砲") > 0 _
        Or InStr(wname, "キャノン") > 0 Or InStr(wname, "カノン") > 0 _
        Or InStr(wname, "弾") > 0 _
    Then
        If InStr(wname, "リニア") > 0 _
            Or InStr(wname, "レール") > 0 _
            Or InStr(wname, "電磁") > 0 _
        Then
            PlayWave "Thunder.wav"
            Sleep 300
        End If
        
        wtype = "キャノン砲"
        
        attack_times = CountAttack0(u, w)
        
        GoTo FoundWeaponType
    End If
    
SkipShootingWeapon:
    
    '対応する武器は見つからなかった
    wtype = "デフォルト"
    
FoundWeaponType:
    
    '空中移動専用形態は武器を手で構えない。
    'また等身大基準の場合、非人間ユニットはメカであることが多いのでこちらも
    '内蔵武器を優先する。
    If is_handy_weapon _
        And (u.Data.Transportation = "空" _
            Or (IsOptionDefined("等身大基準") And Not u.IsHero())) _
    Then
        Select Case wtype
            Case "ＭＡＰバスタービームライフル"
                wtype = "ＭＡＰビーム"
            Case "バスタービームライフル"
                wtype = "大ビーム"
            Case "ダブルビームランチャー"
                wtype = "２連中ビーム"
            Case "ビームランチャー"
                wtype = "中ビーム"
            Case "ダブルビームライフル"
                wtype = "２連小ビーム"
            Case "ビームライフル"
                wtype = "小ビーム"
            Case "レーザーマシンガン"
                wtype = "ニードルレーザー連射"
            Case "レーザーガン"
                wtype = "ニードルレーザー"
            Case "サブマシンガン", "マシンガン"
                wtype = "機関砲"
            Case "ヘビーマシンガン"
                wtype = "重機関砲"
            Case "ガトリング"
                wtype = "内蔵ガトリング"
            Case "ショットガン"
                wtype = "ベアリング"
            Case Else
                '手持ち武器の画像を空にする
                bmpname = "-.bmp"
        End Select
    End If
    
    'マップ攻撃？
    If InStrNotNest(wclass, "Ｍ") > 0 Then
        'マップ攻撃対応アニメに置き換え
        Select Case wtype
            Case "矢", "小型ミサイル", "ミサイル", "スーパーミサイル", _
                "グレネード", "キャノン砲", "大キャノン砲", "ＩＣＢＭ", _
                "シュートカッター", "球電", "氷弾", "火炎弾", "岩弾", _
                "発光", "落雷", "放電", "氷柱", _
                "つらら", "凍結", "吹雪", "風", _
                "強風", "竜巻", "津波", "泡", _
                "音符", "オールレンジ", "煙", _
                "気弾", "連気弾", "気斬", "波動放射"
                wtype = "ＭＡＰ" & wtype
            Case "炎", "炎投射", "火炎放射"
                wtype = "ＭＡＰ炎"
            Case "ニードル", "ニードル連射"
                wtype = "ＭＡＰニードル"
            Case "投下爆弾"
                wtype = "ＭＡＰ爆発"
            Case "重力弾"
                wtype = "ＭＡＰブラックホール"
            Case Else
                If InStr(wname, "フラッシュ") > 0 Or InStr(wname, "閃光") > 0 Then
                    wtype = "ＭＡＰフラッシュ"
                ElseIf InStr(wname, "ダーク") > 0 Or InStr(wname, "闇") > 0 Then
                    wtype = "ＭＡＰダークネス"
                ElseIf InStr(wname, "地震") > 0 Or InStr(wname, "クウェイク") > 0 _
                    Or InStr(wname, "クエイク") > 0 _
                Then
                    wtype = "ＭＡＰ地震"
                    sname = " Explode(Far).wav"
                ElseIf InStr(wname, "核") > 0 Or InStr(wname, "アトミック") > 0 Then
                    wtype = "ＭＡＰ核爆発"
                End If
        End Select
    End If
    
    
    '使用した攻撃手段を記録
    CurrentWeaponType = wtype
    
    '描画色を最終決定
    If InStr(wname, "レッド") > 0 Or InStr(wname, "赤") > 0 Then
        cname = "赤"
    ElseIf InStr(wname, "ブルー") > 0 Or InStr(wname, "青") > 0 Then
        cname = "青"
    ElseIf InStr(wname, "イエロー") > 0 Or InStr(wname, "黄") > 0 Then
        cname = "黄"
    ElseIf InStr(wname, "グリーン") > 0 Or InStr(wname, "緑") > 0 Then
        cname = "緑"
    ElseIf InStr(wname, "ピンク") > 0 Or InStr(wname, "桃") > 0 Then
        cname = "桃"
    ElseIf InStr(wname, "ブラウン") > 0 Or InStr(wname, "橙") > 0 Then
        cname = "橙"
    ElseIf InStr(wname, "ブラック") > 0 Or InStr(wname, "黒") > 0 _
        Or InStr(wname, "ダーク") > 0 Or InStr(wname, "闇") > 0 _
    Then
        cname = "黒"
    ElseIf InStr(wname, "ホワイト") > 0 Or InStr(wname, "白") > 0 _
        Or InStr(wname, "ホーリー") > 0 Or InStr(wname, "聖") > 0 _
    Then
        cname = "白"
    End If
    
    '２種類のアニメを組み合わせる場合
    If Len(wtype0) > 0 Then
        '表示する準備アニメの種類
        aname = wtype0 & "準備"
        
        '色
        If Len(cname0) > 0 Then
            aname = aname & " " & cname0
        ElseIf Len(cname) > 0 Then
            aname = aname & " " & cname
        End If
        
        '効果音
        If Len(sname0) > 0 Then
            aname = aname & " " & sname0
        End If
        
        '戦闘アニメ表示
        ShowAnimation aname
    End If
    
    '表示する攻撃アニメの種類
    aname = wtype & "攻撃"
    
    '発射回数
    If attack_times > 0 Then
        aname = aname & " " & Format$(attack_times)
    End If
    
    '画像
    If Len(bmpname) > 0 Then
        aname = aname & " " & bmpname
    End If
    
    '色
    If Len(cname) > 0 Then
        aname = aname & " " & cname
    End If
    
    '効果音
    If Len(sname) > 0 Then
        aname = aname & " " & sname
    End If
    
    '攻撃アニメ表示
    ShowAnimation aname
End Sub

'武器使用時の効果音
Public Sub AttackSound(u As Unit, ByVal w As Integer)
Dim wname As String, wclass As String
Dim sname As String, num As Integer
Dim i As Integer
        
    'フラグをクリア
    IsWavePlayed = False
    
    '右クリック中は効果音をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '効果音が必要ないもの
    If u.IsWeaponClassifiedAs(w, "武") _
        Or u.IsWeaponClassifiedAs(w, "突") _
        Or u.IsWeaponClassifiedAs(w, "接") _
    Then
        Exit Sub
    End If
    If InStr(wname, "ビームサーベル") > 0 Then
        Exit Sub
    End If
    If InStrNotNest(wclass, "武") > 0 Then
        If InStr(wname, "銃剣") > 0 Then
            Exit Sub
        End If
    End If
    
    '効果音の再生回数
    num = CountAttack(u, w)
    
    '武器名に応じて効果音を選択
    If InStr(wname, "主砲") > 0 Or InStr(wname, "副砲") > 0 Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Cannon.wav"
        End If
    ElseIf InStr(wname, "対空砲") > 0 Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
            num = 4
        Else
            sname = "MachineCannon.wav"
        End If
    ElseIf InStr(wname, "レーザー") > 0 Or InStr(wname, "光線") > 0 _
        Or InStr(wname, "凝集光") > 0 _
        Or InStr(wname, "熱線") > 0 Or InStr(wname, "冷線") > 0 _
        Or InStr(wname, "衝撃波") > 0 Or InStr(wname, "電磁波") > 0 _
        Or InStr(wname, "電波") > 0 Or InStr(wname, "音波") > 0 _
        Or InStr(wname, "磁力") > 0 _
        Or InStr(wname, "ブラックホール") > 0 Or InStr(wname, "縮退") > 0 _
        Or InStr(wname, "ウェーブ") > 0 Or InStr(wname, "波動") > 0 _
        Or InStr(wname, "ソニック") > 0 Or InStr(wname, "スパーク") > 0 _
        Or InStr(wname, "エネルギー") > 0 _
    Then
        sname = "LaserGun.wav"
    ElseIf InStr(wname, "粒子") > 0 Or InStr(wname, "陽電子") > 0 _
        Or InStr(wname, "陽子") > 0 _
        Or InStr(wname, "ブラスター") > 0 Or InStr(wname, "ブラスト") > 0 _
        Or InStr(wname, "フェイザー") > 0 Or InStr(wname, "ディスラプター") > 0 _
        Or InStr(wname, "スマッシャー") > 0 Or InStr(wname, "スラッシャー") > 0 _
        Or InStr(wname, "フラッシャー") > 0 Or InStr(wname, "ディバイダー") > 0 _
        Or InStr(wname, "ドライバー") > 0 Or InStr(wname, "シュトラール") > 0 _
        Or InStr(wname, "ニュートロン") > 0 Or InStr(wname, "プラズマ") > 0 _
        Or InStr(wname, "イオン") > 0 Or InStr(wname, "プロミネンス") > 0 _
        Or InStr(wname, "ハイドロ") > 0 Or InStr(wname, "インパルス") > 0 _
        Or InStr(wname, "フレイム") > 0 Or InStr(wname, "サンシャイン") > 0 _
    Then
        sname = "Beam.wav"
    ElseIf InStr(wname, "シューター") > 0 Then
        If InStrNotNest(wclass, "実") > 0 Then
            sname = "Missile.wav"
        Else
            sname = "Beam.wav"
        End If
    ElseIf InStr(wname, "ビーム") > 0 Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "LaserGun.wav"
        End If
        If InStr(wname, "バルカン") > 0 _
            Or InStr(wname, "マシンガン") > 0 _
        Then
            num = 4
        End If
    ElseIf InStr(wname, "機関銃") > 0 _
        Or InStr(wname, "機銃") > 0 _
        Or InStr(wname, "マシンガン") > 0 _
        Or InStr(wname, "アサルトライフル") > 0 _
        Or InStr(wname, "チェーンライフル") > 0 _
        Or InStr(wname, "パレットライフル") > 0 _
        Or InStr(wname, "マウラー砲") > 0 _
        Or InStr(wname, "ＳＭＧ") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "LaserGun.wav"
        Else
            sname = "MachineGun.wav"
        End If
        num = 1
    ElseIf InStr(wname, "機関砲") > 0 _
        Or InStr(wname, "速射砲") > 0 _
        Or InStr(wname, "マシンキャノン") > 0 _
        Or InStr(wname, "モーターカノン") > 0 _
        Or InStr(wname, "ガンクラスター") > 0 _
        Or InStr(wname, "チェーンガン") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "LaserGun.wav"
        Else
            sname = "MachineCannon.wav"
        End If
        num = 1
    ElseIf InStr(wname, "ガンポッド") > 0 _
        Or InStr(wname, "バルカン") > 0 _
        Or InStr(wname, "ガトリング") > 0 _
        Or InStr(wname, "ハンドレールガン") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "LaserGun.wav"
        Else
            sname = "GunPod.wav"
        End If
        num = 1
    ElseIf InStr(wname, "リニアキャノン") > 0 Or InStr(wname, "レールキャノン") > 0 _
        Or InStr(wname, "リニアカノン") > 0 Or InStr(wname, "レールカノン") > 0 _
        Or InStr(wname, "リニアガン") > 0 Or InStr(wname, "レールガン") > 0 _
        Or (InStr(wname, "電磁") > 0 And InStr(wname, "砲") > 0) _
    Then
        PlayWave "Thunder.wav"
        Sleep 300
        PlayWave "Cannon.wav"
        For i = 2 To num
            Sleep 130
            PlayWave "Cannon.wav"
        Next
    ElseIf InStr(wname, "ライフル") > 0 Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Rifle.wav"
        End If
    ElseIf InStr(wname, "バズーカ") > 0 _
        Or InStr(wname, "ジャイアントバズ") > 0 _
        Or InStr(wname, "シュツルムファウスト") > 0 _
        Or InStr(wname, "グレネード") > 0 _
        Or InStr(wname, "グレネイド") > 0 _
        Or InStr(wname, "ナパーム") > 0 _
        Or InStr(wname, "クレイモア") > 0 _
        Or InStr(wname, "ロケット砲") > 0 _
        Or InStr(wname, "迫撃砲") > 0 _
        Or InStr(wname, "無反動砲") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Bazooka.wav"
        End If
    ElseIf InStr(wname, "自動砲") > 0 _
        Or InStr(wname, "オートキャノン") > 0 _
    Then
        sname = "FastGun.wav"
        num = 1
    ElseIf InStr(wname, "弓") > 0 _
        Or InStr(wname, "アロー") > 0 _
        Or InStr(wname, "ボーガン") > 0 _
        Or InStr(wname, "ボウガン") > 0 _
        Or InStr(wname, "ロングボウ") > 0 _
        Or InStr(wname, "ショートボウ") > 0 _
        Or InStr(wname, "針") > 0 _
        Or InStr(wname, "髪") > 0 _
    Then
        sname = "Bow.wav"
    ElseIf InStr(wname, "マイン") > 0 _
        Or InStr(wname, "クラッカー") > 0 _
        Or InStr(wname, "手投弾") > 0 _
        Or InStr(wname, "手榴弾") > 0 _
        Or InStr(wname, "投げ") > 0 _
        Or InStr(wname, "スリング") > 0 _
        Or InStr(wname, "手裏剣") > 0 _
        Or InStr(wname, "苦無") > 0 _
        Or InStr(wname, "クナイ") > 0 _
    Then
        sname = "Swing.wav"
    ElseIf InStr(wname, "爆弾") > 0 _
        Or InStr(wname, "爆雷") > 0 _
        Or InStr(wname, "爆撃") > 0 _
    Then
        sname = "Bomb.wav"
    ElseIf InStr(wname, "機雷") > 0 Then
        sname = "Explode.wav"
    ElseIf InStr(wname, "マイクロミサイル") > 0 _
        And InStrNotNest(wclass, "Ｍ") > 0 _
    Then
        sname = "MicroMissile.wav"
        num = 1
    ElseIf InStr(wname, "全方位ミサイル") > 0 Then
        sname = "MicroMissile.wav"
        num = 1
    ElseIf InStr(wname, "ミサイル") > 0 Or InStr(wname, "ロケット") > 0 _
        Or InStr(wname, "魚雷") > 0 Or InStr(wname, "反応弾") > 0 _
        Or InStr(wname, "マルチポッド") > 0 Or InStr(wname, "マルチランチャー") > 0 _
        Or InStr(wname, "ショット") > 0 Or InStr(wname, "フルファイア") > 0 _
        Or InStr(wname, "ストリーム") > 0 Or InStr(wname, "ナックル") > 0 _
        Or InStr(wname, "パンチ") > 0 Or InStr(wname, "鉄腕") > 0 _
        Or InStr(wname, "発射") > 0 Or InStr(wname, "射出") > 0 _
        Or InStr(wname, "ランチャー") > 0 _
        Or InStr(wname, "ＡＴＭ") > 0 Or InStr(wname, "ＡＡＭ") > 0 _
        Or InStr(wname, "ＡＧＭ") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Missile.wav"
        End If
    ElseIf InStr(wname, "砲") > 0 _
        Or InStr(wname, "弾") > 0 _
        Or InStr(wname, "キャノン") > 0 _
        Or InStr(wname, "カノン") > 0 _
        Or InStr(wname, "ボム") > 0 _
        Or InStr(wname, "火球") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Cannon.wav"
        End If
    ElseIf InStr(wname, "ガン") > 0 _
        Or InStr(wname, "ピストル") > 0 _
        Or InStr(wname, "リボルヴァー") > 0 _
        Or InStr(wname, "マグナム") > 0 _
        Or InStr(wname, "ライアット") > 0 _
        Or InStr(wname, "銃") > 0 _
    Then
        If InStrNotNest(wclass, "Ｂ") > 0 Then
            sname = "Beam.wav"
        Else
            sname = "Gun.wav"
        End If
    ElseIf InStr(wname, "ソニックブレード") > 0 _
        Or InStr(wname, "ビームカッター") > 0 _
        Or InStr(wname, "スライサー") > 0 _
    Then
        sname = "Saber.wav"
    ElseIf InStr(wname, "重力") > 0 _
        Or InStr(wname, "グラビ") > 0 _
    Then
        sname = "Shock(Low).wav"
    ElseIf InStr(wname, "ストーム") > 0 _
        Or InStr(wname, "トルネード") > 0 _
        Or InStr(wname, "ハリケーン") > 0 _
        Or InStr(wname, "タイフーン") > 0 _
        Or InStr(wname, "サイクロン") > 0 _
        Or InStr(wname, "ブリザード") > 0 _
        Or InStr(wname, "竜巻") > 0 _
        Or InStr(wname, "渦巻") > 0 _
        Or InStr(wname, "台風") > 0 _
        Or InStr(wname, "嵐") > 0 _
        Or InStr(wname, "吹雪") > 0 _
        Or InStr(wname, "フリーザー") > 0 _
        Or InStr(wname, "テレキネシス") > 0 _
    Then
        sname = "Storm.wav"
        num = 1
    ElseIf InStr(wname, "ブーメラン") > 0 _
        Or InStr(wname, "ウェッブ") > 0 _
    Then
        sname = "Swing.wav"
        num = 5
    ElseIf InStr(wname, "サンダー") > 0 _
        Or InStr(wname, "ライトニング") > 0 _
        Or InStr(wname, "ボルト") > 0 _
        Or InStr(wname, "稲妻") > 0 _
        Or InStr(wname, "放電") > 0 _
        Or InStr(wname, "電撃") > 0 _
        Or InStr(wname, "電流") > 0 _
        Or InStr(wname, "雷") > 0 _
        Or InStrNotNest(wclass, "雷") > 0 _
    Then
        sname = "Thunder.wav"
        num = 1
    ElseIf InStr(wname, "火炎放射") > 0 Then
        sname = "AntiShipMissile.wav"
    ElseIf InStr(wname, "火炎") > 0 _
        Or InStr(wname, "焔") > 0 _
    Then
        sname = "Fire.wav"
        num = 1
    ElseIf InStr(wname, "魔法") > 0 _
        Or InStrNotNest(wclass, "魔") > 0 _
        Or InStr(wname, "サイコキネシス") > 0 _
        Or InStr(wname, "糸") > 0 _
        Or InStr(wname, "アンカー") > 0 _
    Then
        sname = "Whiz.wav"
    ElseIf InStr(wname, "泡") > 0 _
        Or InStr(wname, "バブル") > 0 _
    Then
        sname = "Bubble.wav"
    ElseIf Right$(wname, 1) = "液" Then
        sname = "Shower.wav"
    ElseIf Right$(wname, 3) = "ブレス" _
        Or Right$(wname, 3) = "の息" _
    Then
        If InStrNotNest(wclass, "火") > 0 Then
            sname = "AntiShipMissile.wav"
        ElseIf InStrNotNest(wclass, "冷") > 0 Then
            sname = "Storm.wav"
        ElseIf InStrNotNest(wclass, "闇") > 0 Then
            sname = "GunPod.wav"
        ElseIf InStrNotNest(wclass, "水") > 0 Then
            sname = "Hide.wav"
        Else
            sname = "AntiShipMissile.wav"
        End If
    ElseIf InStr(wname, "一斉射撃") > 0 Then
        sname = "MultipleRocketLauncher(Light).wav"
        num = 1
    ElseIf InStrNotNest(wclass, "Ｂ") > 0 Then
        'なんか分からんけどビーム
        sname = "Beam.wav"
    ElseIf InStrNotNest(wclass, "銃") > 0 Then
        'なんか分からんけど銃
        sname = "Gun.wav"
    End If
    
    '効果音なし？
    If sname = "" Then
        'フラグをクリア
        IsWavePlayed = False
        Exit Sub
    End If
    
    For i = 1 To num
        PlayWave sname
        
        'ウェイトを入れる
        Sleep 130
        If sname = "Swing.wav" Then
            Sleep 150
        End If
    Next
    
    'フラグをクリア
    IsWavePlayed = False
End Sub


'武器命中時の特殊効果
Public Sub HitEffect(u As Unit, w As Integer, t As Unit, _
    Optional ByVal hit_count As Integer)
    
    '右クリック中は特殊効果をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    If BattleAnimation Then
        HitAnimation u, w, t, hit_count
    Else
        HitSound u, w, t, hit_count
    End If
End Sub

'武器命中時のアニメーション
Public Sub HitAnimation(u As Unit, ByVal w As Integer, t As Unit, ByVal hit_count As Integer)
Dim wname As String, wclass As String, wtype As String, wtype0 As String
Dim aname As String, cname As String, sname As String
Dim attack_times As Integer
Dim double_weapon As Boolean
Dim double_attack As Boolean
Dim combo_attack As Boolean
Dim i As Integer

    '戦闘アニメ非自動選択オプション
    If IsOptionDefined("戦闘アニメ非自動選択") Then
        ShowAnimation "ダメージ命中"
        Exit Sub
    End If
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    'マップ攻撃の場合は武器にかかわらずダメージを使う
    If InStrNotNest(wclass, "Ｍ") > 0 Then
        '攻撃力0の攻撃の場合は「ダメージ」のアニメを使用しない
        If u.WeaponPower(w, "") = 0 Then
            Exit Sub
        End If
        
        wtype = "ダメージ"
        
        If IsBeamWeapon(wname, wclass, cname) _
            Or InStr(wname, "ミサイル") > 0 _
            Or InStr(wname, "ロケット") > 0 _
        Then
            sname = "Explode.wav"
        End If
        
        GoTo FoundWeaponType
    End If
    
    '二刀流？
    If InStr(wname, "ダブル") > 0 Or InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "デュアル") > 0 _
        Or InStr(wname, "双") > 0 Or InStr(wname, "二刀") > 0 _
        Or InStr(wname, "２連") > 0 Or InStr(wname, "二連") > 0 _
        Or InStr(wname, "連装") > 0 _
    Then
        double_weapon = True
    End If
    
    '連続攻撃？
    If InStr(wname, "ダブル") > 0 Or InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "コンビネーション") > 0 _
        Or InStr(wname, "連") > 0 Or InStrNotNest(wclass, "連") > 0 _
    Then
        double_attack = True
    End If
    
    '乱打？
    If InStr(wname, "乱打") > 0 Or InStr(wname, "乱舞") > 0 _
        Or InStr(wname, "乱れ") > 0 Or InStr(wname, "百烈") > 0 _
    Then
        combo_attack = True
    End If
    
    'これから武器の種類を判定
    
    'まずは白兵戦用武器の判定
    If InStrNotNest(wclass, "武") = 0 _
        And InStrNotNest(wclass, "突") = 0 _
        And InStrNotNest(wclass, "接") = 0 _
        And Not (InStrNotNest(wclass, "格") > 0 And InStrNotNest(wclass, "実") > 0) _
    Then
        GoTo SkipInfightWeapon
    End If
    
    '突撃系(武器を構えて突進する)
    
    If InStr(wname, "突撃") > 0 Or InStr(wname, "突進") > 0 _
        Or InStr(wname, "チャージ") > 0 _
    Then
        Select Case WeaponInHand
            Case ""
                '該当せず
            Case Else
                wtype = WeaponInHand & "突撃"
                GoTo FoundWeaponType
        End Select
    End If
    
    '打撃系
    
    If InStrNotNest(wclass, "実") > 0 _
        And (InStr(wname, "パンチ") > 0 Or InStr(wname, "ナックル") > 0) _
    Then
        wtype = "ロケットパンチ"
        GoTo FoundWeaponType
    End If
    
    '乱打
    If InStr(wname, "拳法") > 0 _
        Or Right$(wname, 2) = "アーツ" _
        Or Right$(wname, 5) = "ストライク" _
    Then
        wtype = "連打"
        GoTo FoundWeaponType
    End If
    
    '通常打撃
    If InStr(wname, "パンチ") > 0 Or InStr(wname, "ナックル") > 0 _
        Or InStr(wname, "ブロー") > 0 Or InStr(wname, "チョップ") > 0 _
        Or InStr(wname, "ビンタ") > 0 _
        Or InStr(wname, "殴") > 0 _
        Or Right$(wname, 1) = "手" Or Right$(wname, 1) = "腕" _
        Or InStr(wname, "格闘") > 0 _
        Or InStr(wname, "トンファー") > 0 _
        Or InStr(wname, "棒") > 0 Or InStr(wname, "杖") > 0 _
        Or InStr(wname, "スタッフ") > 0 Or InStr(wname, "メイス") > 0 _
        Or Right$(wname, 2) = "ムチ" Or InStr(wname, "鞭") > 0 _
        Or InStr(wname, "ウィップ") > 0 Or InStr(wname, "チェーン") > 0 _
        Or InStr(wname, "ロッド") > 0 _
        Or InStr(wname, "モーニングスター") > 0 Or InStr(wname, "フレイル") > 0 _
        Or InStr(wname, "ヌンチャク") > 0 Or InStr(wname, "三節根") > 0 _
        Or (InStr(wname, "チェーン") > 0 And InStr(wname, "チェーンソー") = 0) _
        Or InStr(wname, "バット") > 0 Or InStr(wname, "ギター") > 0 _
        Or InStr(wname, "竹刀") > 0 _
        Or InStr(wname, "ハリセン") > 0 _
    Then
        If combo_attack Then
            wtype = "乱打"
        ElseIf double_attack _
            Or InStr(wname, "触手") > 0 Or InStr(wname, "触腕") > 0 _
        Then
            wtype = "連打"
        Else
            wtype = "打撃"
        End If
        
        If Right$(wname, 2) = "ムチ" Or InStr(wname, "鞭") > 0 _
            Or InStr(wname, "ウィップ") > 0 Or InStr(wname, "チェーン") > 0 _
            Or InStr(wname, "触手") > 0 Or InStr(wname, "触腕") > 0 _
            Or (InStr(wname, "ロッド") > 0 And wname <> "ロッド") _
            Or InStr(wname, "竹刀") > 0 _
            Or InStr(wname, "ハリセン") > 0 _
        Then
            sname = "Whip.wav"
        ElseIf InStr(wname, "張り手") > 0 Or InStr(wname, "平手") > 0 _
            Or InStr(wname, "ビンタ") > 0 _
        Then
            sname = "Slap.wav"
        End If
        
        GoTo FoundWeaponType
    End If
    
    '強打撃
    If InStr(wname, "拳") > 0 Or InStr(wname, "掌") > 0 _
        Or InStr(wname, "打") > 0 Or InStr(wname, "勁") > 0 _
        Or InStr(wname, "ラリアート") > 0 _
        Or InStr(wname, "キック") > 0 Or InStr(wname, "蹴") > 0 _
        Or InStr(wname, "脚") > 0 Or Right$(wname, 1) = "足" _
        Or InStr(wname, "ヘッドバッド") > 0 Or InStr(wname, "頭突") > 0 _
        Or InStr(wname, "ハンマー") > 0 Or InStr(wname, "槌") > 0 _
        Or InStr(wname, "モール") > 0 _
    Then
        If combo_attack Then
            wtype = "乱打"
        ElseIf double_attack Then
            wtype = "連打"
        Else
            wtype = "強打"
        End If
        
        If InStr(wname, "拳") > 0 Or InStr(wname, "掌") > 0 _
            Or InStr(wname, "打") > 0 Or InStr(wname, "勁") > 0 _
        Then
            PlayWave "Bazooka.wav"
        End If
        
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "アッパー") > 0 Then
        wtype = "アッパー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "タックル") > 0 Or InStr(wname, "体当") > 0 _
        Or InStr(wname, "チャージ") > 0 Or InStr(wname, "ぶちかまし") > 0 _
        Or InStr(wname, "バンカー") > 0 _
    Then
        wtype = "強打"
        sname = "Crash.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "バンカー") > 0 Then
        wtype = "強打"
        sname = "Bazooka.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "怪力") > 0 Then
        wtype = "超打"
        sname = "Crash.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "格闘") > 0 Then
        wtype = "打撃"
        GoTo FoundWeaponType
    End If
    
    '斬撃系
    
    If InStr(wname, "ビーム") > 0 _
        Or InStr(wname, "プラズマ") > 0 _
        Or InStr(wname, "レーザー") > 0 _
        Or InStr(wname, "ブラスター") > 0 _
        Or InStr(wname, "ライト") > 0 _
    Then
        If InStr(wname, "プラズマ") > 0 Then
            cname = "グリーン"
        ElseIf InStr(wname, "レーザー") > 0 Then
            cname = "ブルー"
        ElseIf InStr(wname, "ライト") > 0 Then
            cname = "イエロー"
        End If
        
        If InStr(wname, "サーベル") > 0 _
            Or InStr(wname, "セイバー") > 0 _
            Or InStr(wname, "ブレード") > 0 _
            Or InStr(wname, "ソード") > 0 _
            Or InStr(wname, "剣") > 0 _
            Or InStr(wname, "刀") > 0 _
        Then
            If InStr(wname, "ハイパー") > 0 Or InStr(wname, "ロング") > 0 _
                Or InStr(wname, "大") > 0 Or InStr(wname, "高") > 0 _
            Then
                wtype = "ハイパービームサーベル"
            Else
                wtype = "ビームサーベル"
            End If
            
            If double_weapon Then
                wtype = "ダブル" & wtype
            ElseIf InStr(wname, "回転") > 0 Then
                wtype = "回転" & wtype
            End If
            
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "カッター") > 0 Then
            If InStr(wname, "ハイパー") > 0 Or InStr(wname, "ロング") > 0 _
                Or InStr(wname, "大") > 0 Or InStr(wname, "高") > 0 _
            Then
                wtype = "エナジーブレード"
            Else
                wtype = "エナジーカッター"
            End If
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "ナイフ") > 0 _
            Or InStr(wname, "ダガー") > 0 _
        Then
            wtype = "ビームナイフ"
            GoTo FoundWeaponType
        End If
        
        If InStr(wname, "ナギナタ") > 0 Then
            wtype = "回転ビームサーベル"
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "ソード") > 0 Or InStr(wname, "剣") > 0 _
        Or InStr(wname, "ナイフ") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 _
        Or InStr(wname, "カットラス") > 0 _
        Or InStr(wname, "刀") > 0 Or InStr(wname, "斬") > 0 _
        Or InStr(wname, "ブレード") > 0 Or InStr(wname, "刃") > 0 _
        Or InStr(wname, "アックス") > 0 Or InStr(wname, "斧") > 0 _
        Or InStr(wname, "グレイブ") > 0 Or InStr(wname, "ナギナタ") > 0 _
        Or InStr(wname, "切") > 0 Or InStr(wname, "裂") > 0 _
        Or InStr(wname, "カット") > 0 Or InStr(wname, "カッター") > 0 _
        Or InStr(wname, "スラッシュ") > 0 _
        Or InStr(wname, "居合") > 0 _
    Then
        If combo_attack Then
            wtype = "斬撃乱舞"
        ElseIf double_weapon Then
            wtype = "連斬撃"
        ElseIf double_attack Then
            wtype = "ダブル斬撃"
        ElseIf InStrNotNest(wclass, "火") > 0 Then
            wtype = "炎斬撃"
        ElseIf InStrNotNest(wclass, "雷") > 0 Then
            wtype = "雷斬撃"
        ElseIf InStrNotNest(wclass, "冷") > 0 Then
            wtype = "凍斬撃"
        ElseIf InStr(wname, "唐竹割") > 0 Or InStr(wname, "縦") > 0 Then
            wtype = "唐竹割"
        ElseIf InStr(wname, "居合") > 0 Or InStr(wname, "横") > 0 Then
            wtype = "なぎ払い"
        ElseIf InStr(wname, "斬") > 0 Then
            wtype = "大斬撃"
        ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
            wtype = "斬り上げ"
        ElseIf InStr(wname, "黒") > 0 Or InStr(wname, "闇") > 0 _
            Or InStr(wname, "死") > 0 _
            Or InStr(wname, "ダーク") > 0 Or InStr(wname, "デス") > 0 _
        Then
            wtype = "黒斬撃"
        Else
            wtype = "斬撃"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ショーテル") > 0 Then
        wtype = "ダブル斬撃"
        GoTo FoundWeaponType
    End If
    
    '刺突系
    
    If InStr(wname, "スピア") > 0 Or InStr(wname, "槍") > 0 _
        Or InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 _
        Or InStr(wname, "トライデント") > 0 _
        Or InStr(wname, "ジャベリン") > 0 _
        Or InStr(wname, "レイピア") > 0 _
        Or wname = "ロッド" _
    Then
        If combo_attack Then
            wtype = "乱突"
        ElseIf double_attack Then
            wtype = "連突"
        Else
            wtype = "刺突"
        End If
        GoTo FoundWeaponType
    End If
    
    'その他格闘系
    
    If InStr(wname, "爪") > 0 Or InStr(wname, "クロー") > 0 _
        Or InStr(wname, "ひっかき") > 0 _
    Then
        If InStr(wname, "アーム") > 0 Then
            wtype = "打撃"
            sname = "Crash.wav"
        Else
            wtype = "爪撃"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "噛") > 0 Or InStr(wname, "牙") > 0 _
        Or InStr(wname, "かみつき") > 0 _
    Then
        wtype = "噛み付き"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ドリル") > 0 Then
        wtype = "ドリル"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "リボン") > 0 Then
        wtype = "リボン"
        GoTo FoundWeaponType
    End If
    
    '掴み系
    
    If InStr(wname, "スープレックス") > 0 Or InStr(wname, "投げ") > 0 _
        Or wname = "返し" _
    Then
        wtype = "投げ飛ばし"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ヒールホールド") > 0 Then
        wtype = "足固め"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ブリーカー") > 0 Then
        wtype = "背負い固め"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "固め") > 0 Or InStr(wname, "ホールド") > 0 _
        Or InStr(wname, "ツイスト") > 0 _
        Or InStr(wname, "絞め") > 0 Or InStr(wname, "締め") > 0 _
        Or InStr(wname, "折り") > 0 _
    Then
        wtype = "立ち固め"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ジャイアントスイング") > 0 Then
        wtype = "ジャイアントスイング"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "地獄車") > 0 Then
        wtype = "地獄車"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ブレーンバスター") > 0 Then
        wtype = "ブレーンバスター"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "スクリューバックドライバー") > 0 Then
        wtype = "スクリューバックドライバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "スクリュードライバー") > 0 Then
        wtype = "スクリュードライバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "バックドライバー") > 0 Then
        wtype = "バックドライバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ドライバー") > 0 Then
        wtype = "ドライバー"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "踏み") > 0 Or InStr(wname, "押し") > 0 Then
        wtype = "踏み潰し"
        GoTo FoundWeaponType
    End If
    
    '詳細が分からなかった武器
    If InStrNotNest(wclass, "武") > 0 Then
        '装備しているアイテムから武器を検索
        For i = 1 To u.CountItem
            With u.Item(i)
                If .Activated _
                    And (.Part = "両手" _
                        Or .Part = "片手" _
                        Or .Part = "武器") _
                Then
                    wtype = CheckWeaponType(.Nickname, "")
                    If wtype = "" Then
                        wtype = CheckWeaponType(.Class0, "")
                    End If
                    Exit For
                End If
            End With
        Next
        Select Case wtype
            Case "スピア", "ランス", "トライデント", "和槍", _
                "エストック"
                If combo_attack Then
                    wtype = "乱突"
                ElseIf double_attack Then
                    wtype = "連突"
                Else
                    wtype = "刺突"
                End If
            Case Else
                If combo_attack Then
                    wtype = "斬撃乱舞"
                ElseIf double_weapon Then
                    wtype = "ダブル斬撃"
                ElseIf double_attack Then
                    wtype = "連斬撃"
                ElseIf InStrNotNest(wclass, "火") > 0 Then
                    wtype = "炎斬撃"
                ElseIf InStrNotNest(wclass, "雷") > 0 Then
                    wtype = "雷斬撃"
                ElseIf InStrNotNest(wclass, "冷") > 0 Then
                    wtype = "凍斬撃"
                ElseIf InStrNotNest(wclass, "Ｊ") > 0 Then
                    wtype = "斬り上げ"
                Else
                    wtype = "斬撃"
                End If
        End Select
        GoTo FoundWeaponType
    End If
    
    '詳細が分からなかった近接技
    If InStrNotNest(wclass, "突") > 0 _
        And InStrNotNest(wclass, "接") > 0 _
    Then
        If combo_attack Then
            wtype = "乱打"
        ElseIf double_attack Then
            wtype = "連打"
        Else
            wtype = "打撃"
        End If
        GoTo FoundWeaponType
    End If
    
SkipInfightWeapon:
    
    '射撃武器(格闘投擲)
    
    If InStr(wname, "斧") > 0 Or InStr(wname, "アックス") > 0 _
        Or InStr(wname, "トマホーク") > 0 _
        Or InStr(wname, "ソーサー") > 0 Or InStr(wname, "チャクラム") > 0 _
    Then
        wtype = "ダメージ"
        sname = "Saber.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "パンチ") > 0 Or InStr(wname, "ハンマー") > 0 _
        Or InStr(wname, "岩") > 0 Or InStr(wname, "鉄球") > 0 _
    Then
        wtype = "強打"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 _
        Or InStr(wname, "分銅") > 0 Or InStr(wname, "ブーメラン") > 0 _
    Then
        wtype = "打撃"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ナイフ") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "手裏剣") > 0 Or InStr(wname, "クナイ") > 0 _
        Or InStr(wname, "苦無") > 0 _
    Then
        wtype = "刺突"
        GoTo FoundWeaponType
    End If
    
    'これより通常射撃攻撃
    
    'まずは光線系の攻撃かどうかを判定する
    
    If IsBeamWeapon(wname, wclass, cname) Then
        wtype = "ビーム"
    End If
    
    If wtype = "ビーム" Then
        '実弾系武器判定をスキップ
        GoTo SkipNormalWeapon
    End If
    
    '射撃武器(実弾系)
    
    If InStr(wname, "弓") > 0 _
        Or InStr(wname, "ショートボウ") > 0 _
        Or InStr(wname, "ロングボウ") > 0 _
        Or InStr(wname, "ボウガン") > 0 _
        Or InStr(wname, "矢") > 0 Or InStr(wname, "アロー") > 0 _
    Then
        wtype = "矢"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "バルカン") > 0 Then
        wtype = "バルカン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ガトリング") > 0 Or InStr(wname, "チェーンガン") _
        Or InStr(wname, "ガンランチャー") _
    Then
        wtype = "ガトリング"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "マシンガン") > 0 Or InStr(wname, "機関銃") > 0 Then
        If InStr(wname, "ヘビー") > 0 Or InStr(wname, "重") > 0 Then
            wtype = "ヘビーマシンガン"
        Else
            wtype = "マシンガン"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "機銃") > 0 Or InStr(wname, "機関砲") > 0 Then
        wtype = "マシンガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "マシンキャノン") > 0 _
        Or InStr(wname, "オートキャノン") > 0 _
        Or InStr(wname, "速射砲") > 0 _
    Then
        wtype = "ヘビーマシンガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ショットガン") > 0 Or InStr(wname, "散弾") > 0 _
        Or InStr(wname, "拡散バズーカ") > 0 _
    Then
        wtype = "ショットガン"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ベアリング") > 0 Or InStr(wname, "クレイモア") > 0 Then
        wtype = "ベアリング"
        GoTo FoundWeaponType
    End If
    
SkipNormalWeapon:
    
    '射撃武器(エネルギー系)
    
    If InStr(wname, "怪光線") > 0 Then
        wtype = "怪光線"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "破壊光線") > 0 Then
        wtype = "破壊光線"
        GoTo FoundWeaponType
    End If
    
    If wtype = "ビーム" Then
        If InStr(CurrentWeaponType, "ビーム") > 0 _
            Or InStr(CurrentWeaponType, "レーザー") > 0 _
        Then
            '可能であれば発射時のエフェクトと統一する
            Select Case CurrentWeaponType
                Case "ビームライフル"
                    wtype = "小ビーム"
                Case "ダブルビームライフル"
                    wtype = "２連小ビーム"
                Case "ビームランチャー"
                    wtype = "中ビーム"
                Case "ダブルビームランチャー"
                    wtype = "２連中ビーム"
                Case "バスタービームライフル"
                    wtype = "大ビーム"
                Case "レーザーガン"
                    wtype = "ニードルレーザー"
                Case "レーザーマシンガン"
                    wtype = "ニードルレーザー連射"
                Case Else
                    wtype = CurrentWeaponType
            End Select
        Else
            If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 _
                Or InStr(wname, "大") > 0 _
                Or Left$(wname, 2) = "ギガ" _
            Then
                wtype = "大ビーム"
            ElseIf InStr(wname, "メガ") > 0 _
                Or InStr(wname, "ハイ") > 0 _
                Or InStr(wname, "バズーカ") > 0 _
            Then
                wtype = "中ビーム"
            ElseIf CountAttack0(u, w) >= 4 _
                Or InStr(wname, "対空") > 0 _
            Then
                wtype = "ニードルレーザー連射"
            ElseIf InStr(wname, "ピストル") > 0 _
                Or InStr(wname, "ミニ") > 0 _
                Or InStr(wname, "小") > 0 _
            Then
                wtype = "ニードルレーザー"
            ElseIf InStr(wname, "ランチャー") > 0 _
                Or InStr(wname, "キャノン") > 0 _
                Or InStr(wname, "カノン") > 0 _
                Or InStr(wname, "砲") > 0 _
            Then
                wtype = "中ビーム"
            Else
                wtype = "小ビーム"
            End If
            
            Select Case wtype
                Case "小ビーム", "中ビーム"
                    If double_weapon Then
                        wtype = "２連" & wtype
                    End If
            End Select
            
            If InStr(wname, "拡散") > 0 _
                Or InStr(wname, "放射") > 0 _
            Then
                wtype = "拡散ビーム"
            End If
            
            If InStr(wname, "ホーミング") > 0 _
                Or InStr(wname, "誘導") > 0 _
            Then
                wtype = "ホーミングレーザー"
            End If
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "光線") > 0 Then
        wtype = "怪光線"
        GoTo FoundWeaponType
    End If
    
    '爆発系
    
    If InStr(wname, "ピストル") > 0 Or InStr(wname, "拳銃") > 0 _
        Or InStr(wname, "リボルバー") > 0 Or InStr(wname, "リボルヴァー") > 0 _
        Or InStr(wname, "銃") > 0 Or Right$(wname, 2) = "ガン" _
        Or InStr(wname, "ライフル") > 0 _
    Then
        wtype = "銃弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "爆雷") > 0 Then
            wtype = "爆雷"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "爆撃") > 0 Or CurrentWeaponType = "投下爆弾" Then
            wtype = "爆撃"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ミサイル") > 0 Or InStr(wname, "ロケット") > 0 _
        Or InStr(wname, "爆弾") > 0 _
        Or InStr(wname, "ダイナマイト") > 0 Or InStr(wname, "榴弾") > 0 _
        Or InStr(wname, "反応弾") > 0 _
        Or InStr(wname, "グレネード") > 0 Or InStr(wname, "手榴弾") > 0 _
        Or InStr(wname, "クラッカー") > 0 Or InStr(wname, "ディスチャージャー") > 0 _
        Or InStr(wname, "マイン") > 0 Or InStr(wname, "ボム") > 0 _
        Or InStr(wname, "魚雷") > 0 Or InStr(wname, "機雷") > 0 _
        Or InStr(wname, "バズーカ") > 0 _
        Or InStr(wname, "シュツルムファウスト") > 0 _
    Then
        If InStr(wname, "核") > 0 Or InStr(wname, "反応") > 0 _
            Or InStr(wname, "アトミック") > 0 _
            Or InStr(wname, "超") > 0 _
        Then
            wtype = "超爆発"
        ElseIf InStr(wname, "大") > 0 Or InStr(wname, "ビック") > 0 _
            Or InStr(wname, "ジャイアント") > 0 Or InStr(wname, "メガ") > 0 _
        Then
            wtype = "大爆発"
        ElseIf InStr(wname, "小") > 0 Or InStr(wname, "ミニ") > 0 _
            Or InStr(wname, "マイクロ") > 0 _
        Then
            wtype = "小爆発"
        Else
            wtype = "爆発"
        End If
        
        '連続爆発？
        
        If wtype = "超爆発" Then
            GoTo FoundWeaponType
        End If
        
        attack_times = CountAttack0(u, w)
        If InStrNotNest(wclass, "連") > 0 Then
            attack_times = hit_count
        End If
        
        If attack_times = 1 Then
            attack_times = 0
            GoTo FoundWeaponType
        End If
        
        If wtype = "小爆発" Then
            wtype = "連続爆発"
        Else
            wtype = "連続" & wtype
        End If
        
        GoTo FoundWeaponType
    End If
    
    'その他特殊系
    
    If InStr(wname, "電撃") > 0 Or InStr(wname, "電流") > 0 _
        Or InStr(wname, "エレクト") > 0 _
    Then
        wtype = "破壊光線"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "雷") > 0 Or InStr(wname, "ライトニング") > 0 _
        Or InStr(wname, "サンダー") > 0 _
        Or Right$(wname, 2) = "稲妻" _
        Or InStrNotNest(wclass, "電") > 0 _
    Then
        If InStrNotNest(wclass, "実") = 0 Then
            wtype = "放電"
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "吹雪") > 0 Or InStr(wname, "ブリザード") > 0 _
        Or InStr(wname, "アイスストーム") > 0 _
    Then
        wtype = "吹雪"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ストーム") > 0 Or InStr(wname, "ハリケーン") > 0 _
        Or InStr(wname, "タイフーン") > 0 _
        Or InStr(wname, "台風") > 0 Or InStr(wname, "嵐") > 0 _
    Then
        wtype = "強風"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "ウィンド") > 0 Or InStr(wname, "ウインド") > 0 _
        Or InStr(wname, "風") > 0 _
    Then
        wtype = "風"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "トルネード") > 0 Or InStr(wname, "サイクロン") _
        Or InStr(wname, "竜巻") > 0 Or InStr(wname, "渦巻") > 0 _
    Then
        wtype = "竜巻"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "泡") > 0 Or InStr(wname, "バブル") > 0 _
        Or InStr(wname, "消火") > 0 _
    Then
        wtype = "泡"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "重力") > 0 Or InStr(wname, "グラビ") > 0 _
         Or InStr(wname, "ブラックホール") > 0 _
         Or InStr(wname, "縮退") > 0 _
    Then
        wtype = "重力圧縮"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "スロウ") > 0 Then
        wtype = "時間逆行"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "煙") > 0 Or InStr(wname, "スモーク") > 0 _
        Or Right$(wname, 2) = "ガス" Or Right$(wname, 1) = "霧" _
        Or InStr(wname, "胞子") > 0 _
    Then
        wtype = "煙"
        If InStr(wname, "毒") > 0 Or InStrNotNest(wclass, "毒") > 0 Then
            cname = "緑"
        End If
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "火炎弾") > 0 Then
        wtype = "火炎弾"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "火炎放射") > 0 _
        Or Right$(wname, 2) = "火炎" _
    Then
        wtype = "火炎放射"
        GoTo FoundWeaponType
    End If
    
    If Right$(wname, 5) = "ファイアー" _
        Or Right$(wname, 5) = "ファイヤー" _
        Or Right$(wname, 4) = "ファイア" _
        Or Right$(wname, 4) = "ファイヤ" _
    Then
        If InStrNotNest(wclass, "実") = 0 And Left$(wname, 2) <> "フル" Then
            If InStrNotNest(wclass, "術") > 0 Then
                wtype = "炎"
            Else
                wtype = "火炎放射"
            End If
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "息") > 0 Or Right$(wname, 3) = "ブレス" Then
        If InStrNotNest(wclass, "実") = 0 Then
            wtype = "火炎放射"
            
            Select Case SpellColor(wname, wclass)
                Case "青", "黄", "緑", "白", "黒"
                    cname = SpellColor(wname, wclass)
                    sname = "Breath.wav"
            End Select
            
            GoTo FoundWeaponType
        End If
    End If
    
    If InStr(wname, "火") > 0 Or InStr(wname, "炎") > 0 _
        Or InStr(wname, "焔") > 0 Or InStr(wname, "ファイヤー") > 0 _
    Then
        wtype = "炎"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "水鉄砲") > 0 Or InStr(wname, "放水") > 0 _
        Or InStr(wname, "水流") > 0 Or InStr(wname, "酸かけ") > 0 _
        Or Right$(wname, 1) = "液" Or Right$(wname, 1) = "酸" _
    Then
        wtype = "飛沫"
        If InStr(wname, "毒") > 0 Or InStr(wname, "毒") > 0 Then
            cname = "緑"
        ElseIf InStr(wname, "酸") > 0 Then
            cname = "白"
        Else
            cname = "青"
        End If
        sname = "Splash.wav"
        GoTo FoundWeaponType
    End If
    
    If InStr(wname, "吸収") > 0 Or InStr(wname, "ドレイン") > 0 _
         Or InStrNotNest(wclass, "吸") > 0 Or InStrNotNest(wclass, "減") > 0 _
    Then
        wtype = "吸収"
        GoTo FoundWeaponType
    End If
    
    '攻撃力0の攻撃の場合は「ダメージ」のアニメを使用しない
    If u.WeaponPower(w, "") = 0 Then
        Exit Sub
    End If
    
    'デフォルト
    wtype = "ダメージ"
    
FoundWeaponType:
    
    'アニメの不整合を防ぐため、吹き飛ばし時はアニメ効果を打撃に抑えておく
    Select Case wtype
        Case "強打", "超打"
            If InStrNotNest(wclass, "吹") > 0 Or InStrNotNest(wclass, "Ｋ") > 0 Then
                wtype = "打撃"
            End If
    End Select
    
    '表示色を最終決定
    If InStr(wname, "レッド") > 0 Or InStr(wname, "赤") > 0 Then
        cname = "赤"
    ElseIf InStr(wname, "ブルー") > 0 Or InStr(wname, "青") > 0 Then
        cname = "青"
    ElseIf InStr(wname, "イエロー") > 0 Or InStr(wname, "黄") > 0 Then
        cname = "黄"
    ElseIf InStr(wname, "グリーン") > 0 Or InStr(wname, "緑") > 0 Then
        cname = "緑"
    ElseIf InStr(wname, "ピンク") > 0 Or InStr(wname, "桃") > 0 Then
        cname = "桃"
    ElseIf InStr(wname, "ブラウン") > 0 Or InStr(wname, "橙") > 0 Then
        cname = "橙"
    ElseIf InStr(wname, "ブラック") > 0 Or InStr(wname, "黒") > 0 _
        Or InStr(wname, "ダーク") > 0 Or InStr(wname, "闇") > 0 _
    Then
        cname = "黒"
    ElseIf InStr(wname, "ホワイト") > 0 Or InStr(wname, "白") > 0 _
        Or InStr(wname, "ホーリー") > 0 Or InStr(wname, "聖") > 0 _
    Then
        cname = "白"
    End If
    
    '２種類のアニメを組み合わせる場合
    If Len(wtype0) > 0 Then
        '表示する命中アニメの種類
        aname = wtype0 & "命中"
        
        '色
        If Len(cname) > 0 Then
            aname = aname & " " & cname
        End If
        
        '命中アニメ表示
        ShowAnimation aname
    End If
    
    '表示する命中アニメの種類
    aname = wtype & "命中"
    
    '色
    If Len(cname) > 0 Then
        aname = aname & " " & cname
    End If
    
    '効果音
    If Len(sname) > 0 Then
        aname = aname & " " & sname
    End If
    
    '命中数
    If attack_times > 0 Then
        aname = aname & " " & Format$(attack_times)
    End If
    
    '命中アニメ表示
    ShowAnimation aname
End Sub

'武器命中時の効果音
Public Sub HitSound(u As Unit, w As Integer, t As Unit, ByVal hit_count As Integer)
Dim wname As String, wclass As String
Dim num As Integer, i As Integer
    
    '右クリック中は効果音をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '効果音の再生回数
    num = CountAttack(u, w)
    
    '武器に応じて効果音を再生
    If InStrNotNest(wclass, "武") > 0 _
        Or InStrNotNest(wclass, "突") > 0 _
        Or InStrNotNest(wclass, "接") > 0 _
        Or InStrNotNest(wclass, "実") > 0 _
    Then
        If InStr(wname, "ディスカッター") > 0 Or InStr(wname, "リッパー") > 0 _
            Or InStr(wname, "スパイド") > 0 _
            Or InStr(wname, "居合") > 0 Or InStr(wname, "閃") > 0 _
        Then
            PlayWave "Swing.wav"
            Sleep 200
            PlayWave "Sword.wav"
            For i = 2 To num
                Sleep 200
                PlayWave "Sword.wav"
            Next
        ElseIf InStr(wname, "プログレッシブナイフ") > 0 _
            Or InStr(wname, "ドリル") > 0 _
        Then
            PlayWave "Drill.wav"
        ElseIf InStr(wname, "サーベル") > 0 Or InStr(wname, "セイバー") > 0 _
            Or InStr(wname, "ソード") > 0 Or InStr(wname, "ブレード") > 0 _
            Or InStr(wname, "スパッド") > 0 Or InStr(wname, "セーバー") > 0 _
            Or InStr(wname, "ダガー") > 0 Or InStr(wname, "ナイフ") > 0 _
            Or InStr(wname, "トマホーク") > 0 Or InStr(wname, "メイス") > 0 _
            Or InStr(wname, "アックス") > 0 _
            Or InStr(wname, "グレイブ") > 0 Or InStr(wname, "ナギナタ") > 0 _
            Or InStr(wname, "ビアンキ") > 0 Or InStr(wname, "ウェッブ") > 0 _
            Or InStr(wname, "ザンバー") > 0 Or InStr(wname, "マーカー") > 0 _
            Or InStr(wname, "バスター") > 0 Or InStr(wname, "ブラスター") > 0 _
            Or InStr(wname, "クロー") > 0 Or InStr(wname, "ジザース") > 0 _
            Or InStr(wname, "ブーメラン") > 0 Or InStr(wname, "ソーサー") > 0 _
            Or InStr(wname, "レザー") > 0 Or InStr(wname, "レイバー") > 0 _
            Or InStr(wname, "サイズ") > 0 Or InStr(wname, "ショーテル") > 0 _
            Or InStr(wname, "カッター") > 0 Or InStr(wname, "スパイク") > 0 _
            Or InStr(wname, "カトラス") > 0 Or InStr(wname, "エッジ") > 0 _
            Or (InStr(wname, "剣") > 0 And InStr(wname, "手裏剣") = 0) _
            Or InStr(wname, "切") > 0 Or InStr(wname, "斬") > 0 _
            Or InStr(wname, "刀") > 0 Or InStr(wname, "刃") > 0 _
            Or InStr(wname, "斧") > 0 Or InStr(wname, "鎌") > 0 _
            Or InStr(wname, "かま") > 0 Or InStr(wname, "カマ") > 0 _
            Or InStr(wname, "爪") > 0 Or InStr(wname, "かぎづめ") > 0 _
            Or InStr(wname, "ハサミ") > 0 Or InStr(wname, "バサミ") > 0 _
            Or InStr(wname, "羽") > 0 _
        Then
            If Not t.IsHero _
                Or InStr(wname, "ビーム") > 0 _
                Or InStr(wname, "プラズマ") > 0 _
                Or InStr(wname, "レーザー") > 0 _
                Or InStr(wname, "セイバー") > 0 _
            Then
                PlayWave "Saber.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Saber.wav"
                Next
            Else
                PlayWave "Swing.wav"
                Sleep 190
                PlayWave "Slash.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Slash.wav"
                Next
            End If
        ElseIf InStr(wname, "ランサー") > 0 _
            Or InStr(wname, "ランス") > 0 Or InStr(wname, "スピア") > 0 _
            Or InStr(wname, "トライデント") > 0 Or InStr(wname, "ハーケン") > 0 _
            Or InStr(wname, "槍") > 0 Or InStr(wname, "もり") > 0 _
            Or InStr(wname, "手裏剣") > 0 _
            Or InStr(wname, "苦無") > 0 Or InStr(wname, "クナイ") > 0 _
            Or (InStr(wname, "突き") > 0 _
                And InStr(wname, "拳") = 0 And InStr(wname, "頭") = 0) _
        Then
            If Not t.IsHero _
                Or InStr(wname, "ビーム") > 0 _
                Or InStr(wname, "プラズマ") > 0 _
                Or InStr(wname, "レーザー") > 0 _
                Or InStr(wname, "ランサー") > 0 _
            Then
                PlayWave "Saber.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Saber.wav"
                Next
            Else
                PlayWave "Swing.wav"
                Sleep 190
                PlayWave "Stab.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Stab.wav"
                Next
            End If
        ElseIf InStr(wname, "牙") > 0 Or InStr(wname, "ファング") > 0 _
            Or InStr(wname, "噛") > 0 Or InStr(wname, "かみつき") > 0 _
            Or InStr(wname, "顎") > 0 _
        Then
            If Not t.IsHero Then
                PlayWave "Saber.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Saber.wav"
                Next
            Else
                PlayWave "Stab.wav"
                For i = 2 To num
                    Sleep 350
                    PlayWave "Stab.wav"
                Next
            End If
        ElseIf InStr(wname, "ストライク") > 0 _
            Or InStr(wname, "アーツ") > 0 _
            Or InStr(wname, "拳法") > 0 _
            Or InStr(wname, "振動拳") > 0 _
        Then
            PlayWave "Combo.wav"
        ElseIf InStr(wname, "格闘") > 0 Or InStr(wname, "パンチ") > 0 _
            Or InStr(wname, "キック") > 0 Or InStr(wname, "チョップ") > 0 _
            Or InStr(wname, "ナックル") > 0 Or InStr(wname, "ブロー") > 0 _
            Or InStr(wname, "ハンマー") > 0 Or InStr(wname, "トンファー") > 0 _
            Or InStr(wname, "ヌンチャク") > 0 Or InStr(wname, "パイプ") > 0 _
            Or InStr(wname, "ラリアット") > 0 Or InStr(wname, "アーム") > 0 _
            Or InStr(wname, "ヘッドバット") > 0 Or InStr(wname, "スリング") > 0 _
            Or InStr(wname, "頭突き") > 0 _
            Or InStr(wname, "脚") > 0 _
            Or InStr(wname, "蹴") > 0 _
            Or InStr(wname, "棒") > 0 _
            Or InStr(wname, "石") > 0 _
            Or InStr(wname, "角") > 0 _
            Or InStr(wname, "尻尾") > 0 _
            Or InStr(wname, "鉄腕") > 0 _
        Then
            PlayWave "Punch.wav"
            For i = 2 To num
                Sleep 120
                PlayWave "Punch.wav"
            Next
        ElseIf InStr(wname, "体当たり") > 0 _
            Or InStr(wname, "タックル") > 0 _
            Or InStr(wname, "ぶちかまし") > 0 _
            Or InStr(wname, "突進") > 0 _
            Or InStr(wname, "突撃") > 0 _
            Or InStr(wname, "怪力") > 0 _
            Or InStr(wname, "鉄拳") > 0 _
            Or InStr(wname, "メガトンパンチ") > 0 _
            Or InStr(wname, "鉄球") > 0 _
            Or InStr(wname, "ボール") > 0 _
            Or InStr(wname, "車輪") > 0 _
            Or InStr(wname, "キャタピラ") > 0 _
            Or InStr(wname, "シールド") > 0 _
        Then
            PlayWave "Crash.wav"
        ElseIf InStr(wname, "拳") > 0 _
            Or InStr(wname, "掌") > 0 _
            Or InStr(wname, "打") > 0 _
            Or InStr(wname, "勁") > 0 _
        Then
            PlayWave "Bazooka.wav"
            For i = 2 To num
                Sleep 120
                PlayWave "Bazooka.wav"
            Next
        ElseIf InStr(wname, "踏み") > 0 _
            Or InStr(wname, "押し") > 0 _
            Or InStr(wname, "ドロップ") > 0 _
        Then
            PlayWave "Shock(Low).wav"
        ElseIf InStr(wname, "張り手") > 0 _
            Or InStr(wname, "ビンタ") > 0 _
        Then
            PlayWave "Slap.wav"
            For i = 2 To num
                Sleep 120
                PlayWave "Slap.wav"
            Next
        ElseIf InStr(wname, "弓") > 0 _
            Or InStr(wname, "矢") > 0 _
            Or InStr(wname, "アロー") > 0 _
            Or InStr(wname, "ボーガン") > 0 _
            Or InStr(wname, "ボウガン") > 0 _
            Or InStr(wname, "ショートボウ") > 0 _
            Or InStr(wname, "ロングボウ") > 0 _
            Or InStr(wname, "針") > 0 _
            Or InStr(wname, "ニードル") > 0 _
        Then
            PlayWave "Stab.wav"
            For i = 2 To num
                Sleep 120
                PlayWave "Stab.wav"
            Next
        ElseIf InStr(wname, "鞭") > 0 _
            Or InStr(wname, "ムチ") > 0 _
            Or InStr(wname, "ウイップ") > 0 _
            Or InStr(wname, "チェーン") > 0 _
            Or InStr(wname, "ロッド") > 0 _
            Or InStr(wname, "テンタク") > 0 _
            Or InStr(wname, "テイル") > 0 _
            Or InStr(wname, "尾") > 0 _
            Or InStr(wname, "触手") > 0 _
            Or InStr(wname, "触腕") > 0 _
            Or InStr(wname, "舌") > 0 _
            Or InStr(wname, "巻き") > 0 _
            Or InStr(wname, "糸") > 0 _
        Then
            PlayWave "Whip.wav"
        ElseIf InStr(wname, "投げ") > 0 _
            Or InStr(wname, "スープレック") > 0 _
            Or (InStr(wname, "返し") > 0 And InStrNotNest(wclass, "突") > 0) _
        Then
            PlayWave "Swing.wav"
            Sleep 500
            PlayWave "Shock(Low).wav"
            For i = 2 To num
                Sleep 700
                PlayWave "Swing.wav"
                Sleep 500
                PlayWave "Shock(Low).wav"
            Next
        ElseIf InStr(wname, "大雪山おろし") > 0 Then
            PlayWave "Swing.wav"
            Sleep 700
            PlayWave "Swing.wav"
            Sleep 500
            PlayWave "Swing.wav"
            Sleep 300
            PlayWave "Shock(Low).wav"
        ElseIf InStr(wname, "関節") > 0 _
            Or InStr(wname, "固め") > 0 _
            Or InStr(wname, "折り") > 0 _
            Or InStr(wname, "締め") > 0 _
            Or InStr(wname, "絞め") > 0 _
            Or InStr(wname, "アームロック") > 0 _
            Or InStr(wname, "ホールド") > 0 _
        Then
            PlayWave "Swing.wav"
            Sleep 190
            PlayWave "BreakOff.wav"
        ElseIf InStrNotNest(wclass, "核") > 0 _
            Or InStr(wname, "核") > 0 _
            Or InStr(wname, "反応弾") > 0 _
        Then
            PlayWave "Explode(Nuclear).wav"
        ElseIf InStr(wname, "ミサイル") > 0 _
            Or InStr(wname, "ロケット") > 0 _
            Or InStr(wname, "魚雷") > 0 _
            Or InStr(wname, "マルチポッド") > 0 _
            Or InStr(wname, "マルチランチャー") > 0 _
            Or InStr(wname, "爆弾") > 0 _
            Or InStr(wname, "爆雷") > 0 _
            Or InStr(wname, "爆撃") > 0 _
            Or Right$(wname, 3) = "マイン" _
            Or Right$(wname, 2) = "ボム" _
        Then
            PlayWave "Explode(Small).wav"
            For i = 2 To num
                Sleep 130
                PlayWave "Explode(Small).wav"
            Next
        ElseIf InStr(wname, "アンカー") > 0 Then
            '無音
        ElseIf InStrNotNest(wclass, "武") > 0 Then
            'なんか分からんけど武器
            PlayWave "Saber.wav"
            For i = 2 To num
                Sleep 350
                PlayWave "Saber.wav"
            Next
        ElseIf InStrNotNest(wclass, "突") > 0 Then
            'なんか分からんけど突進技
            PlayWave "Punch.wav"
            For i = 2 To num
                Sleep 120
                PlayWave "Punch.wav"
            Next
        Else
            If Not t.IsHero Then
                PlayWave "Explode(Small).wav"
                For i = 2 To num
                    Sleep 130
                    PlayWave "Explode(Small).wav"
                Next
            End If
        End If
    Else
        If InStr(wname, "ストーム") > 0 _
            Or InStr(wname, "トルネード") > 0 _
            Or InStr(wname, "ハリケーン") > 0 _
            Or InStr(wname, "タイフーン") > 0 _
            Or InStr(wname, "サイクロン") > 0 _
            Or InStr(wname, "ブリザード") > 0 _
            Or InStr(wname, "竜巻") > 0 _
            Or InStr(wname, "渦巻") > 0 _
            Or InStr(wname, "台風") > 0 _
            Or InStr(wname, "嵐") > 0 _
        Then
            '命中時は無音
        ElseIf Right$(wname, 1) = "液" Then
            PlayWave "Inori.wav"
        ElseIf InStr(wname, "発火") > 0 _
            Or InStr(wname, "パイロキネシス") > 0 _
        Then
            PlayWave "Fire.wav"
        ElseIf wname = "テレキネシス" Then
            PlayWave "Crash.wav"
        ElseIf InStr(wname, "吸収") > 0 Then
            PlayWave "Charge.wav"
        ElseIf InStrNotNest(wclass, "核") > 0 Then
            PlayWave "Explode(Nuclear).wav"
        Else
            If Not t.IsHero Then
                PlayWave "Explode(Small).wav"
                For i = 2 To num
                    Sleep 130
                    PlayWave "Explode(Small).wav"
                Next
            End If
        End If
    End If
    
    'フラグをクリア
    IsWavePlayed = False
End Sub

'回避時の効果音
Public Sub DodgeEffect(u As Unit, w As Integer)
Dim wname As String, wclass As String
Dim sname As String

    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '特殊効果が指定されていればそれを使用
    If u.IsSpecialEffectDefined(wname & "(回避)") Then
        u.SpecialEffect wname & "(回避)"
        Exit Sub
    End If
    
    If BattleAnimation Then
        Exit Sub
    End If
    
    '攻撃時の効果音が風切り音のみであれば風切り音は不要
    sname = u.SpecialEffectData(wname)
    If InStr(sname, ";") > 0 Then
        sname = Mid$(sname, InStr(sname, ";"))
    End If
    If sname = "Swing.wav" Then
        Exit Sub
    End If
    
    '風切り音が必要かどうか判定
    If InStrNotNest(wclass, "武") _
        Or InStrNotNest(wclass, "突") _
        Or InStrNotNest(wclass, "接") _
    Then
        PlayWave "Swing.wav"
    ElseIf InStrNotNest(wclass, "実") Then
        If InStr(wname, "鞭") > 0 _
            Or InStr(wname, "ムチ") > 0 _
            Or InStr(wname, "ウイップ") > 0 _
            Or InStr(wname, "チェーン") > 0 _
            Or InStr(wname, "ロッド") > 0 _
            Or InStr(wname, "テンタク") > 0 _
            Or InStr(wname, "テイル") > 0 _
            Or InStr(wname, "尾") > 0 _
            Or InStr(wname, "触手") > 0 _
            Or InStr(wname, "触腕") > 0 _
            Or InStr(wname, "舌") > 0 _
            Or InStr(wname, "巻き") > 0 _
            Or InStr(wname, "糸") > 0 _
        Then
            PlayWave "Swing.wav"
        End If
    End If
End Sub

'武器切り払い時の効果音
Public Sub ParryEffect(u As Unit, w As Integer, t As Unit)
Dim wname As String, wclass As String
Dim sname As String, num As Integer
Dim i As Integer
    
    '右クリック中は効果音をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '効果音生成回数を設定
    num = CountAttack(u, w)
    If InStr(wname, "マシンガン") > 0 _
        Or InStr(wname, "機関銃") > 0 _
        Or InStr(wname, "アサルトライフル") > 0 _
        Or InStr(wname, "バルカン") > 0 _
    Then
        num = 4
    End If
    
    '命中音を設定
    If InStrNotNest(wclass, "銃") Or InStrNotNest(wclass, "格") _
        Or InStrNotNest(wclass, "武") Or InStrNotNest(wclass, "突") _
        Or InStr(wname, "弓") > 0 Or InStr(wname, "アロー") > 0 _
        Or InStr(wname, "ロングボウ") > 0 Or InStr(wname, "ショートボウ") > 0 _
        Or InStr(wname, "ボーガン") > 0 Or InStr(wname, "ボウガン") > 0 _
        Or InStr(wname, "針") > 0 Or InStr(wname, "ニードル") > 0 _
        Or InStr(wname, "ランサー") > 0 Or InStr(wname, "ダガー") > 0 _
        Or InStr(wname, "剣") > 0 _
    Then
        sname = "Sword.wav"
    ElseIf InStrNotNest(wclass, "実") Then
        sname = "Explode(Small).wav"
    ElseIf InStrNotNest(wclass, "Ｂ") Then
        sname = "BeamCoat.wav"
    Else
        sname = "Explode(Small).wav"
    End If
    
    '切り払い音を再生
    PlayWave "Saber.wav"
    Sleep 100
    PlayWave sname
    For i = 2 To num
        Sleep 130
        PlayWave "Saber.wav"
        Sleep 100
        PlayWave sname
    Next
    
    'フラグをクリア
    IsWavePlayed = False
End Sub

'シールド防御時の特殊効果
Public Sub ShieldEffect(u As Unit)
    '戦闘アニメ非自動選択オプション
    If IsOptionDefined("戦闘アニメ非自動選択") Then
        ShowAnimation "シールド防御発動"
        Exit Sub
    End If
    
    'シールドのタイプを識別
    With u
        If .IsFeatureAvailable("エネルギーシールド") Then
            ShowAnimation "ビームシールド発動"
        ElseIf .IsFeatureAvailable("小型シールド") Then
            ShowAnimation "シールド防御発動 28"
        ElseIf .IsFeatureAvailable("大型シールド") Then
            ShowAnimation "シールド防御発動 40"
        Else
            ShowAnimation "シールド防御発動"
        End If
    End With
End Sub

'吸収・融合の特殊効果
Public Sub AbsorbEffect(u As Unit, w As Integer, t As Unit)
Dim wname As String, wclass As String, cname As String

    '右クリック中は特殊効果をスキップ
    If IsRButtonPressed() Then
        Exit Sub
    End If
    
    '戦闘アニメオフの場合は効果音再生のみ
    If Not BattleAnimation _
        Or IsOptionDefined("戦闘アニメ非自動選択") _
    Then
        PlayWave "Charge.wav"
        Exit Sub
    End If
    
    With u.Weapon(w)
        wname = .Nickname
        wclass = .Class
    End With
    
    '描画色を決定
    cname = SpellColor(wname, wclass)
    If cname = "" Then
        IsBeamWeapon wname, wclass, cname
    End If
    
    'アニメを表示
    ShowAnimation "粒子集中発動 " & cname
End Sub


'状態変化時の特殊効果
Public Sub CriticalEffect(ctype As String, ByVal w As Integer, ByVal ignore_death As Boolean)
Dim aname As String, sname As String
Dim i As Integer
    
    If Len(ctype) = 0 Then
        ShowAnimation "デフォルトクリティカル"
    Else
        For i = 1 To LLength(ctype)
            aname = LIndex(ctype, i) & "クリティカル"
            
            If aname = "即死クリティカル" And ignore_death Then
                GoTo NextLoop
            End If
            
            If FindNormalLabel("戦闘アニメ_" & aname) = 0 Then
                GoTo NextLoop
            End If
            
            sname = ""
            
            If aname = "ショッククリティカル" Then
                If SelectedUnit.IsWeaponClassifiedAs(w, "冷") Then
                    '冷気による攻撃で行動不能になった場合は効果音をオフ
                    sname = "-.wav"
                End If
            End If
            
            If sname <> "" Then
                ShowAnimation aname & " " & sname
            Else
                ShowAnimation aname
            End If
NextLoop:
        Next
    End If
End Sub


'効果音の再生回数を決定
Private Function CountAttack(u As Unit, ByVal w As Integer, _
    Optional ByVal hit_count As Integer) As Integer
    'メッセージスピードが「超高速」なら繰り返し数を１に設定
    If MessageWait <= 200 Then
        CountAttack = 1
        Exit Function
    End If
    
    '連続攻撃の場合、命中数が指定されたならそちらにあわせる
    If hit_count > 0 And InStr(u.Weapon(w).Class, "連") > 0 Then
        CountAttack = hit_count
        Exit Function
    End If
    
    CountAttack = MinLng(CountAttack0(u, w), 4)
End Function

Private Function CountAttack0(u As Unit, ByVal w As Integer) As Integer
Dim wname As String, wclass As String

    wname = u.WeaponNickname(w)
    wclass = u.Weapon(w).Class
    
    '連続攻撃の場合は攻撃回数にあわせる
    If InStrNotNest(wclass, "連") > 0 Then
        CountAttack0 = u.WeaponLevel(w, "連")
        Exit Function
    End If
    
    If InStr(wname, "連") > 0 Then
        If InStr(wname, "２４連") > 0 Then
            CountAttack0 = 8
            Exit Function
        End If
        If InStr(wname, "２２連") > 0 Then
            CountAttack0 = 8
            Exit Function
        End If
        If InStr(wname, "２０連") > 0 Or InStr(wname, "二十連") > 0 Then
            CountAttack0 = 8
            Exit Function
        End If
        If InStr(wname, "１８連") > 0 Or InStr(wname, "十八連") > 0 Then
            CountAttack0 = 7
            Exit Function
        End If
        If InStr(wname, "１６連") > 0 Or InStr(wname, "十六連") > 0 Then
            CountAttack0 = 7
            Exit Function
        End If
        If InStr(wname, "１４連") > 0 Or InStr(wname, "十四連") > 0 Then
            CountAttack0 = 7
            Exit Function
        End If
        If InStr(wname, "１２連") > 0 Or InStr(wname, "十二連") > 0 Then
            CountAttack0 = 6
            Exit Function
        End If
        If InStr(wname, "１連") > 0 Or InStr(wname, "一連") > 0 Then
            CountAttack0 = 6
            Exit Function
        End If
        If InStr(wname, "１０連") > 0 Or InStr(wname, "十連") > 0 Then
            CountAttack0 = 6
            Exit Function
        End If
        If InStr(wname, "９連") > 0 Or InStr(wname, "九連") > 0 Then
            CountAttack0 = 5
            Exit Function
        End If
        If InStr(wname, "８連") > 0 Or InStr(wname, "八連") > 0 Then
            CountAttack0 = 5
            Exit Function
        End If
        If InStr(wname, "７連") > 0 Or InStr(wname, "七連") > 0 Then
            CountAttack0 = 5
            Exit Function
        End If
        If InStr(wname, "６連") > 0 Or InStr(wname, "六連") > 0 Then
            CountAttack0 = 4
            Exit Function
        End If
        If InStr(wname, "５連") > 0 Or InStr(wname, "五連") > 0 Then
            CountAttack0 = 4
        End If
        If InStr(wname, "４連") > 0 Or InStr(wname, "四連") > 0 Then
            CountAttack0 = 4
            Exit Function
        End If
        If InStr(wname, "３連") > 0 Or InStr(wname, "三連") > 0 Then
            CountAttack0 = 3
            Exit Function
        End If
        If InStr(wname, "２連") > 0 Or InStr(wname, "二連") > 0 Then
            CountAttack0 = 2
            Exit Function
        End If
        
        If InStr(wname, "連打") > 0 _
            Or InStr(wname, "連射") > 0 _
            Or InStr(wname, "多連") > 0 _
        Then
            CountAttack0 = 3
            Exit Function
        End If
        
        CountAttack0 = 2
        Exit Function
    End If
    
    If InStr(wname, "全弾") > 0 Or InStr(wname, "斉") > 0 _
        Or InStr(wname, "乱射") > 0 _
        Or InStr(wname, "フルファイア") > 0 _
        Or InStr(wname, "スプリット") > 0 _
        Or InStr(wname, "マルチ") > 0 _
        Or InStr(wname, "パラレル") > 0 _
        Or InStr(wname, "分身") > 0 _
        Or InStr(wname, "乱打") > 0 Or InStr(wname, "乱舞") > 0 _
        Or InStr(wname, "乱れ") > 0 Or InStr(wname, "百烈") > 0 _
        Or InStr(wname, "千本") > 0 Or InStr(wname, "千手") > 0 _
        Or InStr(wname, "ファンネル") > 0 _
        Or InStr(wname, "ビット") > 0 _
    Then
        CountAttack0 = 4
        Exit Function
    End If
    
    If InStr(wname, "マシンガン") > 0 Or InStr(wname, "機銃") > 0 _
        Or InStr(wname, "機関銃") > 0 _
        Or InStr(wname, "バルカン") > 0 _
        Or InStr(wname, "ガトリング") > 0 _
        Or (InStr(wname, "パルス") > 0 And InStr(wname, "インパルス") = 0) _
        Or InStr(wname, "速射") > 0 _
        Or InStr(wname, "ロケットランチャー") > 0 _
        Or InStr(wname, "ミサイルランチャー") > 0 _
        Or InStr(wname, "ミサイルポッド") > 0 _
    Then
        CountAttack0 = 4
        Exit Function
    End If
    
    If InStr(wname, "トリプル") > 0 _
        Or InStr(wname, "インコム") > 0 _
        Or InStr(wname, "ファミリア") > 0 _
        Or InStr(wname, "爆撃") > 0 _
        Or InStr(wname, "爆弾") > 0 _
        Or InStr(wname, "爆雷") > 0 _
        Or InStr(wname, "艦載機") > 0 _
    Then
        CountAttack0 = 3
        Exit Function
    End If
    
    If InStr(wname, "ツイン") > 0 _
        Or InStr(wname, "ダブル") > 0 _
        Or InStr(wname, "デュアル") > 0 _
        Or InStr(wname, "マイクロ") > 0 _
        Or InStr(wname, "双") > 0 _
        Or InStr(wname, "二丁") > 0 _
        Or InStr(wname, "二刀") > 0 _
    Then
        CountAttack0 = 2
        Exit Function
    End If
    
    CountAttack0 = 1
End Function

'光線系の攻撃かどうかを判定し、表示色を決定
Private Function IsBeamWeapon(wname As String, ByVal wclass As String, cname As String) As Boolean
    If InStrNotNest(wclass, "実") > 0 Then
        '光線系攻撃ではあり得ない
        Exit Function
    End If
    
    If InStr(wname, "ビーム") > 0 _
        Or InStrNotNest(wclass, "Ｂ") > 0 _
    Then
        IsBeamWeapon = True
    Else
        If Right$(wname, 2) = "ガス" Then
            Exit Function
        End If
    End If
    
    If InStr(wname, "反物質") > 0 _
        Or InStr(wname, "熱線") > 0 _
        Or InStr(wname, "ブラスター") > 0 _
    Then
        IsBeamWeapon = True
        cname = "レッド"
    ElseIf InStr(wname, "フェイザー") > 0 _
        Or InStr(wname, "粒子") > 0 _
    Then
        IsBeamWeapon = True
        If InStr(wname, "メガ粒子") > 0 Then
            cname = "イエロー"
        Else
            cname = "ピンク"
        End If
    ElseIf InStr(wname, "冷凍") > 0 _
        Or InStr(wname, "冷線") > 0 _
        Or InStr(wname, "フリーザー") > 0 _
    Then
        IsBeamWeapon = True
        cname = "ブルー"
    ElseIf InStr(wname, "中間子") > 0 _
        Or InStr(wname, "中性子") > 0 _
        Or InStr(wname, "ニュートロン") > 0 _
        Or InStr(wname, "ニュートリノ") > 0 _
    Then
        IsBeamWeapon = True
        cname = "グリーン"
    ElseIf InStr(wname, "プラズマ") > 0 Then
        IsBeamWeapon = True
        cname = "オレンジ"
    ElseIf InStr(wname, "レーザー") > 0 _
        Or InStr(wname, "光子") > 0 _
    Then
        IsBeamWeapon = True
        cname = "イエロー"
    ElseIf InStr(wname, "陽子") > 0 Then
        IsBeamWeapon = True
        cname = "ホワイト"
    End If
    
    If cname = "" Then
        If InStr(wname, "粒子") > 0 Then
            If InStr(wname, "メガ粒子") > 0 Then
                cname = "イエロー"
            Else
                cname = "ピンク"
            End If
        ElseIf InStr(wname, "イオン") > 0 _
            Or InStr(wname, "冷凍") > 0 _
            Or InStr(wname, "電子") > 0 _
        Then
            cname = "ブルー"
        End If
    End If
    
    If Not IsBeamWeapon And cname <> "" Then
        If Right$(wname, 2) = "光線" _
            Or Right$(wname, 1) = "砲" _
            Or Right$(wname, 1) = "銃" _
        Then
            IsBeamWeapon = True
        End If
    End If
End Function

'魔法の表示色
Private Function SpellColor(wname As String, ByVal wclass As String) As String
Dim sclass As String
Dim i As Integer
    
    sclass = wname & wclass
    
    '武器名＆属性に含まれる漢字から判定
    For i = 1 To Len(sclass)
        Select Case Mid$(sclass, i, 1)
            Case "炎", "焔", "火", "血", "灼", "熱", "溶"
                SpellColor = "赤"
                Exit Function
            Case "水", "海", "流", "波", "河"
                SpellColor = "青"
                Exit Function
            Case "風", "嵐", "旋", "樹", "木", "草", "葉", "芽", "毒"
                SpellColor = "緑"
                Exit Function
            Case "邪", "闇", "暗", "死", "冥", "獄", "悪", "夜", "重", "影", "陰", "呪", "殺"
                SpellColor = "黒"
                Exit Function
            Case "土", "地", "金", "砂", "岩", "石", "山", "岳"
                SpellColor = "黄"
                Exit Function
            Case "生", "命", "魅", "誘", "乱", "♂", "♀"
                SpellColor = "桃"
                Exit Function
            Case "聖", "光", "星", "月", "氷", "雪", "冷", "凍", "冬"
                SpellColor = "白"
                Exit Function
            Case "日", "陽"
                SpellColor = "橙"
                Exit Function
        End Select
    Next
    
    '武器名から判定
    If InStr(wname, "ファイヤー") > 0 _
        Or InStr(wname, "フレア") > 0 _
        Or InStr(wname, "ヒート") > 0 _
        Or InStr(wname, "ブラッド") > 0 _
    Then
        SpellColor = "赤"
        Exit Function
    End If
    
    If InStr(wname, "ウォーター") > 0 _
        Or InStr(wname, "アクア") > 0 _
    Then
        SpellColor = "青"
        Exit Function
    End If
    
    If InStr(wname, "ウッド") > 0 _
        Or InStr(wname, "フォレスト") > 0 _
        Or InStr(wname, "ポイズン") > 0 _
    Then
        SpellColor = "緑"
        Exit Function
    End If
    
    If InStr(wname, "イビル") > 0 _
        Or InStr(wname, "エビル") > 0 _
        Or InStr(wname, "ダーク") > 0 _
        Or InStr(wname, "デス") > 0 _
        Or InStr(wname, "ナイト") > 0 _
        Or InStr(wname, "シャドウ") > 0 _
        Or InStr(wname, "カース") > 0 _
        Or InStr(wname, "カーズ") > 0 _
    Then
        SpellColor = "黒"
        Exit Function
    End If
    
    If InStr(wname, "アース") > 0 _
        Or InStr(wname, "サンド") > 0 _
        Or InStr(wname, "ロック") > 0 _
        Or InStr(wname, "ストーン") > 0 _
    Then
        SpellColor = "黄"
        Exit Function
    End If
    
    If InStr(wname, "ライフ") > 0 Then
        SpellColor = "桃"
        Exit Function
    End If
    
    If InStr(wname, "ホーリー") > 0 _
        Or InStr(wname, "スター") > 0 _
        Or InStr(wname, "ムーン") > 0 _
        Or InStr(wname, "コールド") > 0 _
        Or InStr(wname, "アイス") > 0 _
        Or InStr(wname, "フリーズ") > 0 _
    Then
        SpellColor = "白"
        Exit Function
    End If
    
    If InStr(wname, "サン") Then
        SpellColor = "橙"
        Exit Function
    End If
End Function


'破壊アニメーションを表示する
Public Sub DieAnimation(u As Unit)
Dim i As Integer
Dim PT As POINTAPI
Dim fname As String, draw_mode As String

    With u
        EraseUnitBitmap .X, .Y
        
        '人間ユニットでない場合は爆発を表示
        If Not .IsHero Then
            ExplodeAnimation .Size, .X, .Y
            Exit Sub
        End If
        
        GetCursorPos PT
        
        'メッセージウインドウ上でマウスボタンを押した場合
        If Screen.ActiveForm Is frmMessage Then
            With frmMessage
                If .Left \ Screen.TwipsPerPixelX <= PT.X _
                    And PT.X <= (.Left + .Width) \ Screen.TwipsPerPixelX _
                    And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                    And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
                Then
                    If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                        '右ボタンで爆発スキップ
                        Exit Sub
                    End If
                End If
            End With
        End If
        
        'メインウインドウ上でマウスボタンを押した場合
        If Screen.ActiveForm Is MainForm Then
            With MainForm
                If .Left \ Screen.TwipsPerPixelX <= PT.X _
                    And PT.X <= (.Left + .Width) \ Screen.TwipsPerPixelX _
                    And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                    And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
                Then
                    If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                        '右ボタンで爆発スキップ
                        Exit Sub
                    End If
                End If
            End With
        End If
        
        '倒れる音
        Select Case .Area
            Case "地上"
                PlayWave "FallDown.wav"
            Case "空中"
                If MessageWait > 0 Then
                    PlayWave "Bomb.wav"
                    Sleep 500
                End If
                If TerrainClass(.X, .Y) = "水" Or TerrainClass(.X, .Y) = "深海" Then
                    PlayWave "Splash.wav"
                Else
                    PlayWave "FallDown.wav"
                End If
        End Select
        
        'ユニット消滅のアニメーション
        
        'メッセージがウエイト無しならアニメーションもスキップ
        If MessageWait = 0 Then
            Exit Sub
        End If
        
        Select Case .Party0
            Case "味方", "ＮＰＣ"
                fname = "Bitmap\Anime\Common\EFFECT_Tile(Ally)"
            Case "敵"
                fname = "Bitmap\Anime\Common\EFFECT_Tile(Enemy)"
            Case "中立"
                fname = "Bitmap\Anime\Common\EFFECT_Tile(Neutral)"
        End Select
        If FileExists(ScenarioPath & fname & ".bmp") Then
            fname = ScenarioPath & fname
        Else
            fname = AppPath & fname
        End If
        If Not FileExists(fname & "01.bmp") Then
            Exit Sub
        End If
        
        Select Case MapDrawMode
            Case "夜"
                draw_mode = "暗"
            Case Else
                draw_mode = MapDrawMode
        End Select
        
        For i = 1 To 6
            DrawPicture fname & ".bmp", _
                MapToPixelX(.X), MapToPixelY(.Y), _
                32, 32, 0, 0, 0, 0, draw_mode
            DrawPicture "Unit\" & .Bitmap, _
                MapToPixelX(.X), MapToPixelY(.Y), _
                32, 32, 0, 0, 0, 0, "透過 " & draw_mode
            DrawPicture fname & "0" & Format$(i) & ".bmp", _
                MapToPixelX(.X), MapToPixelY(.Y), _
                32, 32, 0, 0, 0, 0, "透過 " & draw_mode
            MainForm.picMain(0).Refresh
            Sleep 50
        Next
        ClearPicture
        MainForm.picMain(0).Refresh
    End With
End Sub

'爆発アニメーションを表示する
Public Sub ExplodeAnimation(tsize As String, ByVal tx As Integer, ByVal ty As Integer)
Dim i As Integer
Dim PT As POINTAPI
Static init_explode_animation As Boolean
Static explode_image_path As String
Static explode_image_num As Integer

    '初めて実行する際に、爆発用画像があるフォルダをチェック
    If Not init_explode_animation Then
        '爆発用画像のパス
        If FileExists(ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then
            explode_image_path = ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode"
        ElseIf FileExists(ScenarioPath & "Bitmap\Event\Explode01.bmp") Then
            explode_image_path = ScenarioPath & "Bitmap\Event\Explode"
        ElseIf FileExists(AppPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then
            explode_image_path = AppPath & "Bitmap\Anime\Explode\EFFECT_Explode"
        Else
            explode_image_path = AppPath & "Bitmap\Event\Explode"
        End If
        
        '爆発用画像の個数
        i = 2
        Do While FileExists(explode_image_path & Format$(i, "00") & ".bmp")
            i = i + 1
        Loop
        explode_image_num = i - 1
    End If
    
    GetCursorPos PT
    
    'メッセージウインドウ上でマウスボタンを押した場合
    If Screen.ActiveForm Is frmMessage Then
        With frmMessage
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .Width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '右ボタンで爆発スキップ
                    Exit Sub
                End If
            End If
        End With
    End If
    
    'メインウインドウ上でマウスボタンを押した場合
    If Screen.ActiveForm Is MainForm Then
        With MainForm
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .Width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '右ボタンで爆発スキップ
                    Exit Sub
                End If
            End If
        End With
    End If
    
    '爆発音
    Select Case tsize
        Case "XL", "LL"
            PlayWave "Explode(Far).wav"
        Case "L", "M", "S", "SS"
            PlayWave "Explode.wav"
    End Select
    
    'メッセージがウエイト無しなら爆発もスキップ
    If MessageWait = 0 Then
        Exit Sub
    End If
    
    '爆発の表示
    If InStr(explode_image_path, "\Anime\") > 0 Then
        '戦闘アニメ版の画像を使用
        Select Case tsize
            Case "XL"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, _
                        160, 160, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 130
                Next
            Case "LL"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 56, MapToPixelY(ty) - 56, _
                        144, 144, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 100
                Next
            Case "L"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, _
                        128, 128, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 70
                Next
            Case "M"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 40, MapToPixelY(ty) - 40, _
                        112, 112, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 50
                Next
            Case "S"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 24, MapToPixelY(ty) - 24, _
                        80, 80, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 40
                Next
            Case "SS"
                For i = 1 To explode_image_num
                    ClearPicture
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, _
                        48, 48, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 40
                Next
        End Select
        ClearPicture
        MainForm.picMain(0).Refresh
    Else
        '汎用イベント画像版の画像を使用
        Select Case tsize
            Case "XL"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, _
                        160, 160, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 130
                Next
            Case "LL"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, _
                        128, 128, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 100
                Next
            Case "L"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 32, MapToPixelY(ty) - 32, _
                        96, 96, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 70
                Next
            Case "M"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 16, MapToPixelY(ty) - 16, _
                        64, 64, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 50
                Next
            Case "S"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, _
                        48, 48, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 40
                Next
            Case "SS"
                For i = 1 To explode_image_num
                    DrawPicture explode_image_path & Format$(i, "00") & ".bmp", _
                        MapToPixelX(tx), MapToPixelY(ty), _
                        32, 32, 0, 0, 0, 0, "透過"
                    MainForm.picMain(0).Refresh
                    Sleep 40
                Next
        End Select
        ClearPicture
        MainForm.picMain(0).Refresh
    End If
End Sub

'攻撃無効化時の特殊効果とメッセージを表示する
Public Sub NegateEffect(u As Unit, t As Unit, _
    ByVal w As Integer, wname As String, ByVal dmg As Long, _
    fname As String, fdata As String, ByVal ecost As Integer, _
    msg As String, ByVal be_quiet As Boolean)
Dim defined As Boolean

    If LIndex(fdata, 1) = "Ｂ" _
        Or LIndex(fdata, 2) = "Ｂ" _
        Or LIndex(fdata, 3) = "Ｂ" _
    Then
        If Not be_quiet Then
            If t.IsMessageDefined("ビーム無効化(" & fname & ")") Then
                t.PilotMessage "ビーム無効化(" & fname & ")"
            Else
                t.PilotMessage "ビーム無効化"
            End If
        End If
        
        If t.IsAnimationDefined("ビーム無効化", fname) Then
            t.PlayAnimation "ビーム無効化", fname
        ElseIf t.IsSpecialEffectDefined("ビーム無効化", fname) Then
            t.SpecialEffect "ビーム無効化", fname
        ElseIf dmg < 0 Then
            AbsorbEffect u, w, t
        ElseIf BattleAnimation Then
            ShowAnimation "ビームコート発動 - " & fname
        ElseIf Not IsWavePlayed Then
            PlayWave "BeamCoat.wav"
        End If
        
        If u.IsAnimationDefined(wname & "(攻撃無効化)") Then
            u.PlayAnimation wname & "(攻撃無効化)"
        ElseIf u.IsSpecialEffectDefined(wname & "(攻撃無効化)") Then
            u.SpecialEffect wname & "(攻撃無効化)"
        End If
        
        If t.IsSysMessageDefined("ビーム無効化", fname) Then
            t.SysMessage "ビーム無効化", fname
        ElseIf fname = "" Then
            If dmg < 0 Then
                DisplaySysMessage msg & t.Nickname & "が攻撃を吸収した。"
            Else
                DisplaySysMessage msg & t.Nickname & "が攻撃を防いだ。"
            End If
        Else
            If dmg < 0 Then
                DisplaySysMessage msg & t.Nickname & "の[" _
                    & fname & "]が攻撃を吸収した。"
            Else
                DisplaySysMessage msg & t.Nickname & "の[" _
                    & fname & "]が攻撃を防いだ。"
            End If
        End If
    Else
        If Not be_quiet Then
            If t.IsMessageDefined("攻撃無効化(" & fname & ")") Then
                t.PilotMessage "攻撃無効化(" & fname & ")"
            Else
                t.PilotMessage "攻撃無効化"
            End If
        End If
        
        If t.IsAnimationDefined("攻撃無効化", fname) Then
            t.PlayAnimation "攻撃無効化", fname
            defined = True
        ElseIf t.IsSpecialEffectDefined("攻撃無効化", fname) Then
            t.SpecialEffect "攻撃無効化", fname
            defined = True
        ElseIf dmg < 0 Then
            AbsorbEffect u, w, t
            defined = True
        ElseIf BattleAnimation Then
            If InStr(fdata, "バリア無効化無効") = 0 Or ecost > 0 Then
                If fname = "バリア" Then
                    ShowAnimation "バリア発動"
                ElseIf fname = "" Then
                    ShowAnimation "バリア発動 - 攻撃無効化"
                Else
                    ShowAnimation "バリア発動 - " & fname
                End If
                defined = True
            End If
        End If
        
        If u.IsAnimationDefined(wname & "(攻撃無効化)") Then
            u.PlayAnimation wname & "(攻撃無効化)"
            defined = True
        ElseIf u.IsSpecialEffectDefined(wname & "(攻撃無効化)") Then
            u.SpecialEffect wname & "(攻撃無効化)"
            defined = True
        End If
        
        If Not defined Then
            HitEffect u, w, t
        End If
        
        If t.IsSysMessageDefined("攻撃無効化", fname) Then
            t.SysMessage "攻撃無効化", fname
        ElseIf fname = "" Then
            If dmg < 0 Then
                DisplaySysMessage msg & t.Nickname & "は攻撃を吸収した。"
            Else
                DisplaySysMessage msg & t.Nickname & "は攻撃を防いだ。"
            End If
        Else
            If dmg < 0 Then
                DisplaySysMessage msg & t.Nickname & "の[" _
                    & fname & "]が攻撃を吸収した。"
            Else
                DisplaySysMessage msg & t.Nickname & "の[" _
                    & fname & "]が攻撃を防いだ。"
            End If
        End If
    End If
End Sub
