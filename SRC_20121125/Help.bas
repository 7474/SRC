Attribute VB_Name = "Help"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'特殊能力＆武器属性の解説表示を行うモジュール


'パイロット p の特殊能力の解説を表示
Public Sub SkillHelp(p As Pilot, sindex As String)
Dim stype As String, sname As String
Dim msg As String
Dim prev_mode As Boolean

    '特殊能力の名称を調べる
    If IsNumeric(sindex) Then
        sname = p.SkillName(CInt(sindex))
    Else
        '付加されたパイロット用特殊能力
        If InStr(sindex, "Lv") > 0 Then
            stype = Left$(sindex, InStr(sindex, "Lv") - 1)
        Else
            stype = sindex
        End If
        sname = p.SkillName(stype)
    End If
    
    msg = SkillHelpMessage(p, sindex)
    
    '解説の表示
    If Len(msg) > 0 Then
        prev_mode = AutoMessageMode
        AutoMessageMode = False
        
        OpenMessageForm
        If AutoMoveCursor Then
            MoveCursorPos "メッセージウィンドウ"
        End If
        DisplayMessage "システム", "<b>" & sname & "</b>;" & msg
        CloseMessageForm
        
        AutoMessageMode = prev_mode
    End If
End Sub

'パイロット p の特殊能力の解説
Public Function SkillHelpMessage(p As Pilot, sindex As String) As String
Dim stype As String, sname As String, sname0 As String
Dim slevel As Double, sdata As String, is_level_specified As Boolean
Dim msg As String
Dim u As Unit, u2 As Unit, uname As String, fdata As String
Dim i As Integer

    '特殊能力の名称、レベル、データを調べる
    With p
        If IsNumeric(sindex) Then
            stype = .Skill(CInt(sindex))
            slevel = .SkillLevel(CInt(sindex))
            sdata = .SkillData(CInt(sindex))
            sname = .SkillName(CInt(sindex))
            sname0 = .SkillName0(CInt(sindex))
            is_level_specified = .IsSkillLevelSpecified(CInt(sindex))
        Else
            '付加されたパイロット用特殊能力
            If InStr(sindex, "Lv") > 0 Then
                stype = Left$(sindex, InStr(sindex, "Lv") - 1)
            Else
                stype = sindex
            End If
            stype = .SkillType(stype)
            slevel = .SkillLevel(stype)
            sdata = .SkillData(stype)
            sname = .SkillName(stype)
            sname0 = .SkillName0(stype)
            is_level_specified = .IsSkillLevelSpecified(stype)
        End If
        
        'パイロットが乗っているユニット
        Set u = .Unit
        If u.Name = "ステータス表示用ダミーユニット" Then
            If IsLocalVariableDefined("搭乗ユニット[" & .ID & "]") Then
                uname = LocalVariableList.Item("搭乗ユニット[" & .ID & "]").StringValue
                If uname <> "" Then
                    Set u2 = u
                    Set u = UList.Item(uname)
                End If
            End If
        End If
    End With
    
    Select Case stype
        Case "オーラ"
            If u.FeatureName0("バリア") = "オーラバリア" Then
                msg = "オーラ技「オ」の攻撃力と" & u.FeatureName0("オーラバリア") & _
                    "の強度に" & Format$(CLng(100 * slevel)) & "の修正を与える。"
            Else
                msg = "オーラ技「オ」の攻撃力の強度に" & Format$(CLng(100 * slevel)) & _
                    "の修正を与える。"
            End If
            If u.IsFeatureAvailable("オーラ変換器") Then
                msg = msg & _
                    "また、" & Term("ＨＰ", u) & "、" & Term("ＥＮ", u) & "、" & _
                    Term("装甲", u) & "、" & Term("運動性") & _
                    "がレベルに合わせてそれぞれ増加する。"
            End If
            
        Case "分身"
            msg = Format$(CLng(100 * slevel \ 16)) & "% の確率で分身し、攻撃を回避する。"
            
        Case "超感覚"
            msg = Term("命中", u) & "・" & Term("回避", u)
            If slevel > 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel + 3)) & " の修正を与える。"
            Else
                msg = msg & "に +0 の修正を与える。"
            End If
            If slevel > 3 Then
                msg = msg & _
                    ";思念誘導攻撃(サ)の射程を" & Format$(CLng(slevel \ 4)) & "だけ延長する。"
            End If
            
        Case "知覚強化"
            msg = Term("命中", u) & "・" & Term("回避", u)
            If slevel > 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel + 3)) & " の修正を与える。;"
            Else
                msg = msg & "に +0 の修正を与える。;"
            End If
            If slevel > 3 Then
                msg = msg & _
                    "思念誘導攻撃(サ)の射程を" & Format$(CLng(slevel \ 4)) & "だけ延長する。"
            End If
            msg = msg & _
                "精神不安定により" & Term("ＳＰ", u) & "消費量が20%増加する"
            
        Case "念力"
            msg = Term("命中", u) & "・" & Term("回避", u)
            If slevel > 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel + 3)) & " の修正を与える。"
            Else
                msg = msg & "に +0 の修正を与える。"
            End If
            
        Case "切り払い"
            msg = "格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を " & _
                Format$(CLng(100 * slevel \ 16)) & "% の確率で切り払って回避する。"
                
        Case "迎撃"
            msg = "実弾攻撃(実)による攻撃を " & _
                Format$(CLng(100 * slevel \ 16)) & "% の確率で迎撃する。"
            
        Case "サイボーグ"
            msg = Term("命中", u) & "・" & Term("回避", u)
            msg = msg & "に +5 の修正を与える。"
            
        Case "Ｓ防御"
            If u.IsFeatureAvailable("盾") Then
                msg = "シールド防御を行い、ダメージを" & _
                    Format$(CLng(100 * slevel + 400)) & "減少させる。"
            Else
                msg = Format$(CLng(100 * slevel \ 16)) & "% の確率でシールド防御を行う。"
            End If
            
        Case "資金獲得"
             msg = "敵を倒した時に得られる" & Term("資金")
            If Not is_level_specified Then
                msg = msg & "が 50% 増加する。"
            ElseIf slevel >= 0 Then
                msg = msg & "が " & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = msg & "が " & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "浄化"
            msg = "浄化技(浄)を使うことで敵の" & _
                p.SkillName0("再生") & "能力を無効化。"
            
        Case "同調率"
            If u.IsHero Then
                msg = "同調により"
            Else
                msg = "機体に同調し"
            End If
            msg = msg & Term("運動性", u) & "・攻撃力を強化する。"
            
        Case "同調率成長"
            If slevel >= 0 Then
                msg = p.SkillName0("同調率") & "の成長率が " & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = p.SkillName0("同調率") & "の成長率が " & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "霊力"
            msg = "現在の" & sname0 & "値にあわせて" & Term("ＨＰ", u) & "・" & _
                Term("ＥＮ", u) & "・" & Term("装甲", u) & "・" & Term("移動力", u) & _
                "を強化する。"
            
        Case "霊力成長"
            If slevel >= 0 Then
                msg = p.SkillName0("霊力") & "の成長率が " & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = p.SkillName0("霊力") & "の成長率が " & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "底力"
            msg = Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & "の1/4以下の時に発動。;" & _
                "命中＆回避 +30%、クリティカル発生率 +50%。"
            
        Case "超底力"
            msg = Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & "の1/4以下の時に発動。;" & _
                "命中＆回避 +50%、クリティカル発生率 +50%。"
            
        Case "覚悟"
            msg = Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & "の1/4以下の時に発動。;"
            If IsOptionDefined("ダメージ倍率低下") Then
                msg = msg & "攻撃力10%アップ、クリティカル発生率 +50%。"
            Else
                msg = msg & "攻撃力1.2倍、クリティカル発生率 +50%。"
            End If
            
        Case "不屈"
            msg = Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & "の1/2以下の時に発動。;" & _
                "損傷率に応じて防御力が増加する。"
            
        Case "素質"
            If Not is_level_specified Then
                msg = "入手する経験値が50%増加する。"
            ElseIf slevel >= 0 Then
                msg = "入手する経験値が " & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = "入手する経験値が " & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "遅成長"
            msg = "入手する経験値が半減する。"
                
        Case "再生", "英雄"
            msg = Term("ＨＰ", u) & "が０になった時に" & Format$(CLng(100 * slevel \ 16)) & _
                "%の確率で復活する。"
            
        Case "超能力"
            msg = Term("命中", u) & "・" & Term("回避", u) & "・" & Term("ＣＴ率", u) & _
                "にそれぞれ +5。;" & _
                "サイキック攻撃(超)の攻撃力に +" & Format$(CLng(100 * slevel)) & "。;" & _
                Term("ＳＰ", u) & "消費量を20%削減する。"
            
        Case "悟り"
            msg = Term("命中", u) & "・" & Term("回避", u) & "に +10 の修正を与える。"
            
        Case "超反応"
            msg = Term("命中", u) & "・" & Term("回避", u) & "・" & Term("ＣＴ率", u)
            If slevel >= 0 Then
                msg = msg & "にそれぞれ +" & _
                    Format$(CLng(2 * slevel)) & " の修正を与える。"
            Else
                msg = msg & "にそれぞれ " & _
                    Format$(CLng(2 * slevel)) & " の修正を与える。"
            End If
            
        Case "術"
            Select Case slevel
                Case 1
                    i = 0
                Case 2
                    i = 10
                Case 3
                    i = 20
                Case 4
                    i = 30
                Case 5
                    i = 40
                Case 6
                    i = 50
                Case 7
                    i = 55
                Case 8
                    i = 60
                Case 9
                    i = 65
                Case Is >= 10
                    i = 70
                Case Else
                    i = 0
            End Select
            msg = "術属性を持つ武装・" & Term("アビリティ", u) & "及び必要技能が" & _
                sname0 & "の武装・" & Term("アビリティ", u) & "の消費" & Term("ＥＮ", u) & _
                "を" & Format$(i) & "%減少させる。"
            
        Case "技"
            Select Case slevel
                Case 1
                    i = 0
                Case 2
                    i = 10
                Case 3
                    i = 20
                Case 4
                    i = 30
                Case 5
                    i = 40
                Case 6
                    i = 50
                Case 7
                    i = 55
                Case 8
                    i = 60
                Case 9
                    i = 65
                Case Is >= 10
                    i = 70
                Case Else
                    i = 0
            End Select
            msg = "技属性を持つ武装・" & Term("アビリティ", u) & "及び必要技能が" & _
                 sname0 & "の武装・" & Term("アビリティ", u) & "の消費" & Term("ＥＮ", u) & _
                 "を" & Format$(i) & "%減少させる。"
            
        Case "集中力"
            msg = Term("スペシャルパワー", u) & "の" & Term("ＳＰ", u) & _
                "消費量が元の80%に減少する。"
            
        Case "闘争本能"
            If p.MinMorale > 100 Then
                If Not p.IsSkillLevelSpecified("闘争本能") Then
                    msg = "出撃時の" & Term("気力", u) & "が" & _
                        Format$(p.MinMorale + 5 * slevel) & "に増加する。"
                ElseIf slevel >= 0 Then
                    msg = "出撃時の" & Term("気力", u) & "が" & _
                        Format$(p.MinMorale + 5 * slevel) & "に増加する。"
                Else
                    msg = "出撃時の" & Term("気力", u) & "が" & _
                        Format$(p.MinMorale + 5 * slevel) & "に減少する。"
                End If
            Else
                If Not p.IsSkillLevelSpecified("闘争本能") Then
                    msg = "出撃時の" & Term("気力", u) & "が105に増加する。"
                ElseIf slevel >= 0 Then
                    msg = "出撃時の" & Term("気力", u) & "が" & _
                        Format$(100 + 5 * slevel) & "に増加する。"
                Else
                    msg = "出撃時の" & Term("気力", u) & "が" & _
                        Format$(100 + 5 * slevel) & "に減少する。"
                End If
            End If
            
        Case "潜在力開放"
            If IsOptionDefined("ダメージ倍率低下") Then
                msg = Term("気力", u) & "130以上で発動し、ダメージを 20% 増加させる。"
            Else
                msg = Term("気力", u) & "130以上で発動し、ダメージを 25% 増加させる。"
            End If
                
        Case "指揮"
            msg = "半径" & StrConv(Format$(p.CommandRange), vbWide) & _
                "マス以内にいる味方ザコ・汎用及び階級所有パイロットの" & _
                Term("命中", u) & "・" & Term("回避", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(5 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(5 * slevel)) & "。"
            End If
            
        Case "階級"
            If InStr(sname, "階級Lv") = 0 Then
                msg = "階級レベル" & StrConv(Format$(CLng(slevel)), vbWide) & "に相当する。;"
            End If
            msg = msg & _
                "半径" & StrConv(Format$(p.CommandRange), vbWide) & _
                "マス以内にいるザコ及び階級所有パイロットに指揮効果を与える。"
            
        Case "格闘サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("格闘", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "射撃サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("射撃", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "魔力サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("魔力", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "命中サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("命中", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "回避サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("回避", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "技量サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("技量", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "反応サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & Term("反応", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(2 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(2 * slevel)) & "。"
            End If
            
        Case "サポート"
            msg = "自分がサポートパイロットの時にメインパイロットの" & _
                Term("命中", u) & "・" & Term("回避", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(3 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(3 * slevel)) & "。"
            End If
            
        Case "広域サポート"
            msg = "半径２マス以内にいる味方パイロットの" & _
                Term("命中", u) & "・" & Term("回避", u)
            If slevel >= 0 Then
                msg = msg & "に +" & Format$(CLng(5 * slevel)) & "。"
            Else
                msg = msg & "に " & Format$(CLng(5 * slevel)) & "。"
            End If
            
        Case "援護"
            msg = "隣接するユニットにサポートアタックとサポートガードを" & _
                "１ターンにそれぞれ" & Format$(CLng(slevel)) & "回行う。"
            
        Case "援護攻撃"
            msg = "隣接するユニットにサポートアタックを１ターンに" & _
                Format$(CLng(slevel)) & "回行う。"
            
        Case "援護防御"
            msg = "隣接するユニットにサポートガードを１ターンに" & _
                Format$(CLng(slevel)) & "回行う。"
            
        Case "統率"
            msg = "自分から攻撃をかけた場合、" & _
                "サポートアタックが同時援護攻撃に変更される。;" & _
                "（１ターンに " & Format$(CLng(slevel)) & "回）"
            
        Case "チーム"
            msg = sdata & "に所属する。" & _
                "同じ" & sdata & "のユニットに対してのみ援護や指揮を行う。"
            
        Case "カウンター"
            msg = "１ターンに " & Format$(CLng(slevel)) & "回" & _
                "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。"
            
        Case "先手必勝"
            If LLength(sdata) = 2 Then
                msg = "パイロットの" & Term("気力", u) & "が" & LIndex(sdata, 2) & "以上で発動。"
            Else
                msg = "パイロットの" & Term("気力", u) & "が120以上で発動。"
            End If
            msg = msg & "反撃が必ずカウンター攻撃になり、相手の攻撃に先制して反撃を行う。"
            
        Case "先読み"
            msg = Format$(CLng(100 * slevel \ 16)) & "%の確率で" & _
                "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。"
            
        Case "再攻撃"
            msg = "自分の攻撃の直後に " & _
                Format$(CLng(100 * slevel \ 16)) & "% の確率で再攻撃を行う。" & _
                "ただしパイロットの" & Term("反応", u) & "が相手を下回る場合、確率は半減。"
            
        Case "２回行動"
            msg = "１ターンに２回、行動が可能になる。"
                
        Case "耐久"
            If slevel >= 0 Then
                msg = "ダメージ計算の際に" & Term("装甲", u) & "を" & _
                    Format$(CLng(5 * slevel)) & "%増加させる。"
            Else
                msg = "ダメージ計算の際に" & Term("装甲", u) & "を" & _
                    Format$(CLng(5 * Abs(slevel))) & "%減少させる。"
            End If
            
        Case "ＳＰ低成長"
            msg = "レベルアップ時の最大" & Term("ＳＰ", u) & "の増加量が通常の半分に減少する。"
            
        Case "ＳＰ高成長"
            msg = "レベルアップ時の最大" & Term("ＳＰ", u) & "の増加量が通常の1.5倍に増加する。"
            
        Case "ＳＰ回復"
            msg = "毎ターン" & Term("ＳＰ", u) & "がパイロットレベル/8+5回復する(+" & _
                Format$(p.Level \ 8 + 5) & ")。"
            
        Case "格闘成長"
            '攻撃力低成長オプションが指定されているかどうかで解説を変更する。
            msg = "レベルアップ時の" & Term("格闘", u) & "の増加量が"
            If IsOptionDefined("攻撃力低成長") Then
                msg = msg & Format$(slevel + 0.5) & "になる。"
            Else
                msg = msg & Format$(slevel + 1) & "になる。"
            End If
            
        Case "射撃成長"
            '攻撃力低成長オプション、術技能の有無によってデフォルト解説を変更する。
            If p.HasMana() Then
                msg = "レベルアップ時の" & Term("魔力", u) & "の増加量が"
            Else
                msg = "レベルアップ時の" & Term("射撃", u) & "の増加量が"
            End If
            If IsOptionDefined("攻撃力低成長") Then
                msg = msg & Format$(slevel + 0.5) & "になる。"
            Else
                msg = msg & Format$(slevel + 1) & "になる。"
            End If
            
        Case "命中成長"
            msg = "レベルアップ時の" & Term("命中", u) & "の増加量が" & Format$(slevel + 2) & "になる。"
            
        Case "回避成長"
            msg = "レベルアップ時の" & Term("回避", u) & "の増加量が" & Format$(slevel + 2) & "になる。"
            
        Case "技量成長"
            msg = "レベルアップ時の" & Term("技量", u) & "の増加量が" & Format$(slevel + 1) & "になる。"
            
        Case "反応成長"
            msg = "レベルアップ時の" & Term("反応", u) & "の増加量が" & Format$(slevel + 1) & "になる。"
            
        Case "防御成長"
            '防御力低成長オプションが指定されているかどうかで解説を変更する。
            msg = "レベルアップ時の" & Term("防御", u) & "の増加量が"
            If IsOptionDefined("防御力低成長") Then
                msg = msg & Format$(slevel + 0.5) & "になる。"
            Else
                msg = msg & Format$(slevel + 1) & "になる。"
            End If
            
        Case "精神統一"
            msg = Term("ＳＰ", u) & "が最大" & Term("ＳＰ", u) & "の20%未満(" & Format$(p.MaxSP \ 5) & _
                "未満)の場合、" & "ターン開始時に" & Term("ＳＰ", u) & "が最大" & Term("ＳＰ", u) & _
                "の10%分回復する(+" & Format$(p.MaxSP \ 10) & ")。"
            
        Case "損傷時気力増加"
            If slevel >= -1 Then
                msg = "ダメージを受けた際に" & Term("気力", u) & "+" & Format$(CLng(slevel + 1)) & "。"
            Else
                msg = "ダメージを受けた際に" & Term("気力", u) & Format$(CLng(slevel + 1)) & "。"
            End If
            
        Case "命中時気力増加"
            If slevel >= 0 Then
                msg = "攻撃を命中させた際に" & Term("気力", u) & "+" & Format$(CLng(slevel)) & _
                    "。(マップ攻撃は例外)"
            Else
                msg = "攻撃を命中させた際に" & Term("気力", u) & Format$(CLng(slevel)) & _
                    "。(マップ攻撃は例外)"
            End If
            
        Case "失敗時気力増加"
            If slevel >= 0 Then
                msg = "攻撃を外してしまった際に" & Term("気力", u) & "+" & Format$(CLng(slevel)) & _
                    "。(マップ攻撃は例外)"
            Else
                msg = "攻撃を外してしまった際に" & Term("気力", u) & Format$(CLng(slevel)) & _
                    "。(マップ攻撃は例外)"
            End If
            
        Case "回避時気力増加"
            If slevel >= 0 Then
                msg = "攻撃を回避した際に" & Term("気力", u) & "+" & Format$(CLng(slevel)) & "。"
            Else
                msg = "攻撃を回避した際に" & Term("気力", u) & Format$(CLng(slevel)) & "。"
            End If
            
        Case "起死回生"
            msg = Term("ＳＰ", u) & "、" & Term("ＨＰ", u) & "、" & Term("ＥＮ", u) & _
                "の全てが最大値の20%以下になると毎ターン最初に発動。" & _
                Term("ＳＰ", u) & "、" & Term("ＨＰ", u) & "、" & Term("ＥＮ", u) & "が全快する。"
            
        Case "戦術"
            msg = "思考パターン決定の際に用いられる" & Term("技量", u)
            If slevel >= 0 Then
                msg = msg & "初期値がレベル×10増加(+" & Format$(CLng(10 * slevel)) & ")。"
            Else
                msg = msg & "初期値がレベル×10減少(" & Format$(CLng(10 * slevel)) & ")。"
            End If
            
        Case "得意技"
            msg = "「" & p.SkillData(stype) & "」属性を持つ武器・" & Term("アビリティ", u) & _
                "によるダメージ・効果量が 20% 増加。" & _
                "また、" & Term("アビリティ", u) & "の継続時間が 40% 増加。"
            
        Case "不得手"
            msg = "「" & p.SkillData(stype) & "」属性を持つ武器・" & Term("アビリティ", u) & _
                "によるダメージ・効果量が 20% 減少。" & _
                "また、" & Term("アビリティ", u) & "の継続時間が 40% 減少。"
            
        Case "ハンター"
            msg = "ターゲットが"
            For i = 2 To LLength(sdata)
                If i = 3 Then
                    msg = msg & "や"
                ElseIf 3 > 2 Then
                    msg = msg & "、"
                End If
                msg = msg & LIndex(sdata, i)
            Next
            msg = msg & "である場合、ターゲットに与えるダメージが"
            If slevel >= 0 Then
                msg = msg & Format$(10 * slevel) & "%増加する。"
            Else
                msg = msg & Format$(-10 * slevel) & "%減少する。"
            End If
            
        Case "ＳＰ消費減少"
            msg = Term("スペシャルパワー", u)
            For i = 2 To LLength(sdata)
                msg = msg & "「" & LIndex(sdata, i) & "」"
            Next
            msg = msg & "の" & Term("ＳＰ", u) & "消費量が"
            If slevel >= 0 Then
                msg = msg & Format$(10 * slevel) & "%減少する。"
            Else
                msg = msg & Format$(-10 * slevel) & "%増加する。"
            End If
            
        Case "スペシャルパワー自動発動"
            msg = Term("気力", u) & "が" & LIndex(sdata, 3) & "以上で発動し、" & _
                "毎ターン最初に「" & LIndex(sdata, 2) & "」が自動でかかる。" & _
                "（" & Term("ＳＰ", u) & "は消費しない）"
            
        Case "修理"
            msg = "修理装置や回復" & Term("アビリティ", u) & "を使った際の" & _
                Term("ＨＰ", u) & "回復量が "
            If slevel >= 0 Then
                msg = msg & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = msg & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "補給"
            If IsOptionDefined("移動後補給不可") Then
                msg = "移動後に補給装置を使用できるようになる。また、"
            End If
            msg = msg & "補給" & Term("アビリティ", u) & "を使った際の" & _
                Term("ＥＮ", u) & "回復量が "
            If slevel >= 0 Then
                msg = msg & Format$(10 * slevel) & "% 増加する。"
            Else
                msg = msg & Format$(-10 * slevel) & "% 減少する。"
            End If
            
        Case "気力上限"
            i = 150
            If slevel <> 0 Then
                i = MaxLng(slevel, 0)
            End If
            msg = Term("気力", u) & "の上限が" & Format$(i) & "になる。"
            
        Case "気力下限"
            i = 50
            If slevel <> 0 Then
                i = MaxLng(slevel, 0)
            End If
            msg = Term("気力", u) & "の下限が" & Format$(i) & "になる。"
        
' ADD START MARGE
        Case "遊撃"
            msg = "移動後使用可能な武器・" & Term("アビリティ", u) & _
                "を使った後に、残った移動力を使って移動できる。"
' ADD END MARGE
        
        Case Else
            'ダミー能力
            
            'パイロット側で解説を定義している？
            With p
                sdata = .SkillData(sname0)
                If ListIndex(sdata, 1) = "解説" Then
                    msg = ListIndex(sdata, ListLength(sdata))
                    If Left$(msg, 1) = """" Then
                        msg = Mid$(msg, 2, Len(msg) - 2)
                    End If
                    SkillHelpMessage = msg
                    Exit Function
                End If
            End With
            
            'ユニット側で解説を定義している？
            With u
                For i = 1 To .CountFeature
                    If .Feature(i) = stype Then
                        fdata = .FeatureData(i)
                        If ListIndex(fdata, 1) = "解説" Then
                            msg = ListIndex(fdata, ListLength(fdata))
                        End If
                    End If
                Next
            End With
            If Not u2 Is Nothing Then
                With u2
                    For i = 1 To .CountFeature
                        If .Feature(i) = stype Then
                            fdata = .FeatureData(i)
                            If ListIndex(fdata, 1) = "解説" Then
                                msg = ListIndex(fdata, ListLength(fdata))
                            End If
                        End If
                    Next
                End With
            End If
            
            If msg = "" Then
                Exit Function
            End If
            
            'ユニット側で解説を定義している場合
            If Left$(msg, 1) = """" Then
                msg = Mid$(msg, 2, Len(msg) - 2)
            End If
    End Select
    
    'パイロット側で解説を定義している？
    With p
        sdata = .SkillData(sname0)
        If ListIndex(sdata, 1) = "解説" Then
            msg = ListIndex(sdata, ListLength(sdata))
            If Left$(msg, 1) = """" Then
                msg = Mid$(msg, 2, Len(msg) - 2)
            End If
        End If
    End With
    
    'ユニット側で解説を定義している？
    With u
        For i = 1 To .CountFeature
            If .Feature(i) = sname0 Then
                fdata = .FeatureData(i)
                If ListIndex(fdata, 1) = "解説" Then
                    msg = ListIndex(fdata, ListLength(fdata))
                    If Left$(msg, 1) = """" Then
                        msg = Mid$(msg, 2, Len(msg) - 2)
                    End If
                End If
            End If
        Next
    End With
    If Not u2 Is Nothing Then
        With u2
            For i = 1 To .CountFeature
                If .Feature(i) = sname0 Then
                    fdata = .FeatureData(i)
                    If ListIndex(fdata, 1) = "解説" Then
                        msg = ListIndex(fdata, ListLength(fdata))
                        If Left$(msg, 1) = """" Then
                            msg = Mid$(msg, 2, Len(msg) - 2)
                        End If
                    End If
                End If
            Next
        End With
    End If
    
    '等身大基準の際は「パイロット」という語を使わないようにする
    If IsOptionDefined("等身大基準") Then
        ReplaceString msg, "メインパイロット", "ユニット"
        ReplaceString msg, "サポートパイロット", "サポート"
        ReplaceString msg, "パイロットレベル", "レベル"
        ReplaceString msg, "パイロット", "ユニット"
    End If
    
    SkillHelpMessage = msg
End Function


'ユニット u の findex 番目の特殊能力の解説を表示
Public Sub FeatureHelp(u As Unit, ByVal findex As Variant, ByVal is_additional As Boolean)
Dim fname As String
Dim msg As String
Dim prev_mode As Boolean

    With u
        '特殊能力の名称を調べる
        If findex = "武器・防具クラス" Then
            fname = findex
        ElseIf IsNumeric(findex) Then
            fname = .AllFeatureName(CInt(findex))
        Else
            fname = .AllFeatureName(findex)
        End If
    End With
    
    msg = FeatureHelpMessage(u, findex, is_additional)
    
    '解説の表示
    If Len(msg) > 0 Then
        prev_mode = AutoMessageMode
        AutoMessageMode = False
        
        OpenMessageForm
        If AutoMoveCursor Then
            MoveCursorPos "メッセージウィンドウ"
        End If
        DisplayMessage "システム", "<b>" & fname & "</b>;" & msg
        CloseMessageForm
        
        AutoMessageMode = prev_mode
    End If
End Sub

'ユニット u の findex 番目の特殊能力の解説
Public Function FeatureHelpMessage(u As Unit, ByVal findex As Variant, _
    ByVal is_additional As Boolean) As String
Dim fid As Integer
Dim ftype As String, fname As String, fname0 As String
Dim fdata As String, flevel As Double, opt As String, lv_mod As Double
Dim flevel_specified As Boolean
Dim msg As String
Dim i As Integer, idx As Integer, buf As String
Dim prob As Integer
Dim p As Pilot, sname  As String, slevel As Double
Dim uname As String

    With u
        'メインパイロット
        Set p = .MainPilot
        
        '特殊能力の名称、レベル、データを調べる
        If findex = "武器・防具クラス" Then
            ftype = findex
            fname = findex
        ElseIf IsNumeric(findex) Then
            fid = CInt(findex)
            ftype = .AllFeature(fid)
            fname = .AllFeatureName(fid)
            fdata = .AllFeatureData(fid)
            flevel = .AllFeatureLevel(fid)
            flevel_specified = .AllFeatureLevelSpecified(fid)
        Else
            ftype = .AllFeature(findex)
            fname = .AllFeatureName(findex)
            fdata = .AllFeatureData(findex)
            flevel = .AllFeatureLevel(findex)
            flevel_specified = .AllFeatureLevelSpecified(findex)
            For fid = 1 To .CountFeature
                If .AllFeature(fid) = findex Then
                    Exit For
                End If
            Next
        End If
        If InStr(fname, "Lv") > 0 Then
            fname0 = Left$(fname, InStr(fname, "Lv") - 1)
        Else
            fname0 = fname
        End If
        
        '重複可能な特殊能力の場合、レベルのみが異なる能力のレベルは累積する
        Select Case ftype
            Case "フィールド", "アーマー", "レジスト", "攻撃回避"
                For i = 1 To u.CountAllFeature
                    If i <> fid _
                        And .AllFeature(i) = ftype _
                        And .AllFeatureData(i) = fdata _
                    Then
                        flevel = flevel + .AllFeatureLevel(i)
                    End If
                Next
        End Select
    End With
    
    Select Case ftype
        Case "シールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                "ダメージを半減。"
            
        Case "大型シールド"
            sname = p.SkillName0("Ｓ防御")
            If p.IsSkillAvailable("Ｓ防御") Then
                prob = (p.SkillLevel("Ｓ防御") + 1) * 100 \ 16
            End If
            msg = "(" & sname & "Lv+1)/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                "ダメージを半減。"
            
        Case "小型シールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                "ダメージを2/3に減少。"
            
        Case "エネルギーシールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            If flevel > 0 Then
                msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                    "ダメージを半減した上で更に" & Format$(100 * flevel) & "減少。"
            Else
                msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                    "ダメージを半減。"
            End If
            msg = msg & "発動時に5ＥＮ消費。「無」属性を持つ武器には無効。"
            
        Case "アクティブシールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            If p.IsSkillAvailable("Ｓ防御") Then
                prob = (p.SkillLevel("Ｓ防御") + 2) * 100 \ 16
            End If
            msg = "(" & sname & "Lv+2)/16の確率(" & Format$(prob) & "%)で防御を行い、" & _
                "ダメージを半減。"
            
        Case "盾"
            sname = p.SkillName0("Ｓ防御")
            slevel = p.SkillLevel("Ｓ防御")
            If slevel > 0 Then
                slevel = 100 * slevel + 400
            End If
            msg = Format$(flevel) & "回、攻撃によって貫通されるまでシールド防御を行い、" & _
                "ダメージを減少させる(-" & Format$(CLng(slevel)) & ")。;" & _
                "ただし攻撃側が「破」属性を持っていた場合、一度に２回分破壊される。;" & _
                "ダメージの減少量はパイロットの" & sname & "レベルによって決まる。"
            
        Case "バリア"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            msg = msg & _
                "ダメージ" & Format$(CLng(1000 * flevel)) & "以下の攻撃を無効化。"
            If IsNumeric(LIndex(fdata, 3)) Then
                If StrToLng(LIndex(fdata, 3)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
                End If
            Else
                msg = msg & _
                    ";発動時に10ＥＮ消費。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "バリアシールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で発動し、"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            msg = msg & _
                "ダメージ" & Format$(CLng(1000 * flevel)) & "以下の攻撃を無効化。"
            If IsNumeric(LIndex(fdata, 3)) Then
                If StrToLng(LIndex(fdata, 3)) > 0 Then
                    msg = msg & _
                        "発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
                End If
            Else
                msg = msg & _
                    "発動時に10" & Term("ＥＮ", u) & "消費。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "広域バリア"
            If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
                msg = "半径" & StrConv(LIndex(fdata, 2), vbWide) & _
                    "マス以内の味方ユニットに対する"
                i = CInt(LIndex(fdata, 2))
            Else
                msg = "隣接する味方ユニットに対する"
                i = 1
            End If
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            msg = msg & _
                "ダメージ" & Format$(CLng(1000 * flevel)) & "以下の攻撃を無効化。"
            If IsNumeric(LIndex(fdata, 4)) Then
                If StrToLng(LIndex(fdata, 4)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 4) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 4), 2) & Term("ＥＮ", u) & "増加。"
                End If
            Else
                msg = msg & _
                    ";発動時に" & Format$(20 * i) & Term("ＥＮ", u) & "消費。"
            End If
            If StrToLng(LIndex(fdata, 5)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 5) & "以上で使用可能。"
            End If
            msg = msg & _
                ";ただし攻撃側も有効範囲内にいる場合は無効化。"
            
        Case "フィールド"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(500 * flevel)) & "減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-500 * flevel)) & "増加させる。"
            End If
            If StrToLng(LIndex(fdata, 3)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "アクティブフィールド"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で発動し、"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(500 * flevel)) & "減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-500 * flevel)) & "増加させる。"
            End If
            If StrToLng(LIndex(fdata, 3)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "広域フィールド"
            If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
                msg = "半径" & StrConv(LIndex(fdata, 2), vbWide) & _
                    "マス以内の味方ユニットに対する"
                i = CInt(LIndex(fdata, 2))
            Else
                msg = "隣接する味方ユニットに対する"
                i = 1
            End If
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            If flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(500 * flevel)) & "減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-500 * flevel)) & "増加させる。"
            End If
            If IsNumeric(LIndex(fdata, 4)) Then
                If StrToLng(LIndex(fdata, 4)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 4) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 4), 2) & Term("ＥＮ", u) & "増加。"
                End If
            Else
                msg = msg & _
                    ";発動時に" & Format$(20 * i) & Term("ＥＮ", u) & "消費。"
            End If
            If StrToLng(LIndex(fdata, 5)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 5) & "以上で使用可能。"
            End If
            msg = msg & _
                ";ただし攻撃側も有効範囲内にいる場合は無効化。"
            
        Case "プロテクション"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel > 10 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel - 100)) & "%吸収する。"
            ElseIf flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel)) & "%減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-10 * flevel)) & "%増加させる。"
            End If
            If Not IsNumeric(LIndex(fdata, 3)) Then
                msg = msg & _
                    ";発動時に10" & Term("ＥＮ", u) & "増加。"
            ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 0.5
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 0.2
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "アクティブプロテクション"
            sname = p.SkillName0("Ｓ防御")
            prob = p.SkillLevel("Ｓ防御") * 100 \ 16
            msg = sname & "Lv/16の確率(" & Format$(prob) & "%)で発動し、"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel > 10 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel - 100)) & "%吸収する。"
            ElseIf flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel)) & "%減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-10 * flevel)) & "%増加させる。"
            End If
            If Not IsNumeric(LIndex(fdata, 3)) Then
                msg = msg & _
                    ";発動時に10" & Term("ＥＮ", u) & "増加。"
            ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            For i = 5 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "バリア無効化無効"
                        msg = msg & _
                            ";バリア無効化によって無効化されない。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 0.5
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & "%)。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & "%)。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 0.2
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & "%)。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & "%)。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & "%)。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & "%)。"
                End Select
            Next
            
        Case "広域プロテクション"
            If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
                msg = "半径" & StrConv(LIndex(fdata, 2), vbWide) & _
                    "マス以内の味方ユニットに対する"
                i = CInt(LIndex(fdata, 2))
            Else
                msg = "隣接する味方ユニットに対する"
                i = 1
            End If
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            If flevel > 10 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel - 100)) & "%吸収する。"
            ElseIf flevel >= 0 Then
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(10 * flevel)) & "%減少させる。"
            Else
                msg = msg & _
                    "攻撃のダメージを" & Format$(CLng(-10 * flevel)) & "%増加させる。"
            End If
            If IsNumeric(LIndex(fdata, 4)) Then
                If StrToLng(LIndex(fdata, 4)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 4) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 4), 2) & Term("ＥＮ", u) & "増加。"
                End If
            Else
                msg = msg & _
                    ";発動時に" & Format$(20 * i) & Term("ＥＮ", u) & "消費。"
            End If
            If StrToLng(LIndex(fdata, 5)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 5) & "以上で使用可能。"
            End If
            msg = msg & _
                ";ただし攻撃側も有効範囲内にいる場合は無効化。"
            
        Case "アーマー"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel >= 0 Then
                msg = msg & _
                    "攻撃に対して装甲を" & Format$(CLng(100 * flevel)) & "増加させる。"
            Else
                msg = msg & _
                    "攻撃に対して装甲を" & Format$(CLng(-100 * flevel)) & "減少させる。"
            End If
            If StrToLng(LIndex(fdata, 3)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 3) & "以上で使用可能。"
            End If
            For i = 4 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 2
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "レジスト"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel > 10 Then
                msg = msg & _
                    "攻撃に対してダメージを" & Format$(100 - CLng(10 * flevel)) & "%吸収する。"
            ElseIf flevel >= 0 Then
                msg = msg & _
                    "攻撃に対してダメージを" & Format$(CLng(10 * flevel)) & "%軽減させる。"
            Else
                msg = msg & _
                    "攻撃に対してダメージを" & Format$(CLng(-10 * flevel)) & "%増加させる。"
            End If
            If StrToLng(LIndex(fdata, 3)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 3) & "以上で使用可能。"
            End If
            For i = 4 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 5
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & "%)。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & "%)。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 2
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & "%)。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & "%)。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & "%)。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 50
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & "%)。"
                End Select
            Next
            
        Case "当て身技"
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            
            If flevel <> 1 Then
                msg = msg & _
                    "ダメージ" & Format$(CLng(500 * flevel)) & "までの"
            End If
            
            msg = msg & "攻撃を"
            
            buf = LIndex(fdata, 4)
            If IsNumeric(buf) Then
                If buf <> "100" Then
                    msg = msg & buf & "%の確率で受け止め、"
                Else
                    msg = msg & "受け止め、"
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で受け止め、"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で受け止め、"
            End If
            
            buf = LIndex(fdata, 2)
            If InStr(buf, "(") > 0 Then
                buf = Left$(buf, InStr(buf, "(") - 1)
            End If
            msg = msg & buf & "で反撃。"
            
            If StrToLng(LIndex(fdata, 5)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 5) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 5), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 6)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 6) & "以上で使用可能。"
            End If
            For i = 7 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を相殺。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "反射"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            
            If flevel <> 1 Then
                msg = msg & _
                    "ダメージ" & Format$(CLng(500 * flevel)) & "までの"
            End If
            
            msg = msg & "攻撃を"
            
            buf = LIndex(fdata, 3)
            If IsNumeric(buf) Then
                If buf <> "100" Then
                    msg = msg & buf & "%の確率で反射。"
                Else
                    msg = msg & "反射。"
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で反射。"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で反射。"
            End If
            
            If StrToLng(LIndex(fdata, 4)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 4) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 4), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 5)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 5) & "以上で使用可能。"
            End If
            For i = 6 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "阻止"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel <> 1 Then
                msg = msg & _
                    "ダメージ" & Format$(CLng(500 * flevel)) & "以下の"
            End If
            msg = msg & "攻撃を"
            
            buf = LIndex(fdata, 3)
            If IsNumeric(buf) Then
                If buf <> "100" Then
                    msg = msg & buf & "%の確率で阻止。"
                Else
' MOD START MARGE
'                    msg = msg & buf & "阻止。"
                    msg = msg & "阻止。"
' MOD END MARGE
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で阻止。"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で阻止。"
            End If
            
            If StrToLng(LIndex(fdata, 4)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 4) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 4), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 5)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 5) & "以上で使用可能。"
            End If
            For i = 6 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を中和。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "広域阻止"
            If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
                msg = "半径" & StrConv(LIndex(fdata, 2), vbWide) & _
                    "マス以内の味方ユニットに対する"
                i = CInt(LIndex(fdata, 2))
            Else
                msg = "隣接する味方ユニットに対する"
                i = 1
            End If
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = msg & _
                        "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = msg & _
                        "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            If flevel <> 1 Then
                msg = msg & _
                    "ダメージ" & Format$(CLng(500 * flevel)) & "以下の"
            End If
            msg = msg & "攻撃を"
            
            buf = LIndex(fdata, 4)
            If IsNumeric(buf) Then
                If buf <> "100" Then
' MOD START MARGE
'                    msg = msg & "%の確率で阻止。"
                    msg = msg & buf & "%の確率で阻止。"
' MOD END MARGE
                Else
                    msg = msg & "阻止。"
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で阻止。"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で阻止。"
            End If
            
            If StrToLng(LIndex(fdata, 5)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 5) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 5), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 6)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 6) & "以上で使用可能。"
            End If
            msg = msg & _
                ";ただし攻撃側も有効範囲内にいる場合は無効化。"
            
        Case "融合"
            prob = flevel * 100 \ 16
            msg = Format$(flevel) & "/16の確率(" & Format$(prob) & "%)で発動し、" & _
                "ダメージを" & Term("ＨＰ", u) & "に変換。;" & _
                "ただし、「武」「突」「接」による攻撃には無効。"
            
        Case "変換"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            msg = msg + "攻撃を受けた際にダメージを" & Term("ＥＮ", u) & "に変換。;" & _
                "変換効率は " & Term("ＥＮ", u) & "増加 ＝ "
            msg = msg + Format$(0.01 * flevel)
            msg = msg + " × ダメージ"
            
        Case "ビーム吸収"
            msg = "ビームによる攻撃のダメージをＨＰに変換"
            
        Case "自動反撃"
            If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
                If Left$(LIndex(fdata, 3), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 3), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 3) & "」属性を持つ"
                End If
            End If
            
            If flevel <> 1 Then
                msg = msg & _
                    "ダメージ" & Format$(CLng(500 * flevel)) & "までの"
            End If
            
            msg = msg & "攻撃を受けた際に"
            
            buf = LIndex(fdata, 4)
            If IsNumeric(buf) Then
                If buf <> "100" Then
                    msg = msg & buf & "%の確率で、"
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で、"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で、"
            End If
            
            buf = LIndex(fdata, 2)
            If InStr(buf, "(") > 0 Then
                buf = Left$(buf, InStr(buf, "(") - 1)
            End If
            msg = msg & buf & "による自動反撃が発動する。"
            
            If StrToLng(LIndex(fdata, 5)) > 0 Then
                msg = msg & _
                    ";発動時に" & LIndex(fdata, 5) & Term("ＥＮ", u) & "消費。"
            ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then
                msg = msg & _
                    ";発動時に" & Mid$(LIndex(fdata, 5), 2) & Term("ＥＮ", u) & "増加。"
            End If
            If StrToLng(LIndex(fdata, 6)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 6) & "以上で使用可能。"
            End If
            For i = 7 To LLength(fdata)
                opt = LIndex(fdata, i)
                idx = InStr(opt, "*")
                If idx > 0 Then
                    lv_mod = StrToDbl(Mid$(opt, idx + 1))
                    opt = Left$(opt, idx - 1)
                Else
                    lv_mod = -1
                End If
                Select Case p.SkillType(opt)
                    Case "相殺"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、隣接時に効果は相殺。"
                    Case "中和"
                        msg = msg & _
                            ";" & fname0 & "を持つユニット同士の場合、" & _
                            "隣接時にレベル分だけ効果を相殺。"
                    Case "近接無効"
                        msg = msg & _
                            ";「武」「突」「接」による攻撃には無効。"
                    Case "手動"
                        msg = msg & _
                            ";防御選択時にのみ発動。"
                    Case "能力必要"
                        'スキップ
                    Case "同調率"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 20
                        End If
                        If u.SyncLevel >= 30 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(+" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        ElseIf u.SyncLevel > 0 Then
                            msg = msg & _
                                ";パイロットの" & sname & "により強度が変化(" & _
                                Format$(lv_mod * (u.SyncLevel - 30)) & ")。"
                        End If
                    Case "霊力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 10
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PlanaLevel) & ")。"
                    Case "オーラ"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.AuraLevel) & ")。"
                    Case "超能力"
                        sname = p.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "により強度が増加(+" & _
                            Format$(lv_mod * u.PsychicLevel) & ")。"
                    Case Else
                        sname = u.SkillName0(opt)
                        If lv_mod = -1 Then
                            lv_mod = 200
                        End If
                        msg = msg & _
                            ";パイロットの" & sname & "レベルにより強度が増加(+" & _
                            Format$(lv_mod * u.SkillLevel(opt)) & ")。"
                End Select
            Next
            
        Case "ＨＰ回復"
            msg = "毎ターン最大" & Term("ＨＰ", u) & "の" & Format$(10 * flevel) & "%分の" & _
                Term("ＨＰ", u) & "を回復。"
            
        Case "ＥＮ回復"
            msg = "毎ターン最大" & Term("ＥＮ", u) & "の" & Format$(10 * flevel) & "%分の" & _
                Term("ＥＮ", u) & "を回復。"
            
        Case "霊力回復"
            sname = p.SkillName0("霊力")
            msg = "毎ターン最大" & sname & "の" & Format$(10 * flevel) & "%分の" & _
                sname & "を回復。"
            
        Case "ＨＰ消費"
            msg = "毎ターン最大" & Term("ＨＰ", u) & "の" & Format$(10 * flevel) & "%分の" & _
                Term("ＨＰ", u) & "を消費。"
            
        Case "ＥＮ消費"
            msg = "毎ターン最大" & Term("ＥＮ", u) & "の" & Format$(10 * flevel) & "%分の" & _
                Term("ＥＮ", u) & "を消費。"
            
        Case "霊力消費"
            sname = p.SkillName0("霊力")
            msg = "毎ターン最大" & sname & "の" & Format$(10 * flevel) & "%分の" & _
                sname & "を消費。"
            
        Case "分身"
            msg = "50%の確率で攻撃を完全に回避。;" & _
                "発動条件：" & Term("気力", u) & "130以上"
            
        Case "超回避"
            msg = "あらゆる攻撃を" & Format$(10 * flevel) & "%の確率で回避。"
            If IsNumeric(LIndex(fdata, 2)) Then
                If StrToLng(LIndex(fdata, 2)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 2) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 2)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 2), 2) & Term("ＥＮ", u) & "増加。"
                End If
            End If
            If StrToLng(LIndex(fdata, 3)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 3) & "以上で使用可能。"
            End If
            If LIndex(fdata, 4) = "手動" Then
                msg = msg & _
                    ";回避選択時にのみ発動。"
            End If
            
        Case "緊急テレポート"
            msg = "攻撃を受けた際に" & Format$(10 * flevel) & "%の確率で" & _
                "テレポートし、攻撃を回避。;" & _
                "テレポート先は" & LIndex(fdata, 2) & "マス以内の範囲の内、" & _
                "最も敵から遠い地点から選ばれる。"
            If IsNumeric(LIndex(fdata, 3)) Then
                If StrToLng(LIndex(fdata, 3)) > 0 Then
                    msg = msg & _
                        ";発動時に" & LIndex(fdata, 3) & Term("ＥＮ", u) & "消費。"
                ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then
                    msg = msg & _
                        ";発動時に" & Mid$(LIndex(fdata, 3), 2) & Term("ＥＮ", u) & "増加。"
                End If
            End If
            If StrToLng(LIndex(fdata, 4)) > 50 Then
                msg = msg & _
                    ";" & Term("気力", u) & LIndex(fdata, 4) & "以上で使用可能。"
            End If
            If LIndex(fdata, 5) = "手動" Then
                msg = msg & _
                    ";回避選択時にのみ発動。"
            End If
            
        Case "ダミー"
            buf = fname
            If InStr(buf, "Lv") Then
                buf = Left$(buf, InStr(buf, "Lv") - 1)
            End If
            msg = buf & "を身代わりにして攻撃を" & Format$(flevel) & "回まで回避。"
            
        Case "攻撃回避"
            If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
                If Left$(LIndex(fdata, 2), 1) = "!" Then
                    msg = "「" & Mid$(LIndex(fdata, 2), 2) & "」属性を持たない"
                Else
                    msg = "「" & LIndex(fdata, 2) & "」属性を持つ"
                End If
            End If
            If flevel >= 0 Then
                msg = msg & _
                    "攻撃の命中率を本来の" & Format$(CLng(100 - 10 * flevel)) & "%に減少させる。"
            Else
                msg = msg & _
                    "攻撃の命中率を本来の" & Format$(CLng(100 - 10 * flevel)) & "%に増加させる。"
            End If
            If StrToLng(LIndex(fdata, 3)) > 50 Then
                msg = msg & _
                    Term("気力", u) & LIndex(fdata, 3) & "以上で使用可能。"
            End If
            
        Case "抵抗力"
            If flevel >= 0 Then
                msg = "武器の特殊効果を受ける確率を" & Format$(10 * flevel) & "%減少させる。"
            Else
                msg = "武器の特殊効果を受ける確率を" & Format$(-10 * flevel) & "%増加させる。"
            End If
            
        Case "修理装置"
            msg = "他のユニットの" & Term("ＨＰ", u)
            Select Case flevel
                Case 1
                    msg = msg & "を最大" & Term("ＨＰ", u) & "の30%だけ回復。"
                Case 2
                    msg = msg & "を最大" & Term("ＨＰ", u) & "の50%だけ回復。"
                Case 3
                    msg = msg & "を全快。"
            End Select
            
        Case "補給装置"
            msg = "他のユニットの" & Term("ＥＮ", u) & "と弾薬を全快。;" & _
                "ただしユニットのパイロットの" & Term("気力", u) & "は-10。"
            If IsOptionDefined("移動後補給不可") Then
                msg = msg & "移動後は使用不可。"
            End If
            
        Case "修理不可"
            For i = 2 To fdata
                buf = LIndex(fdata, i)
                If Left$(buf, 1) = "!" Then
                    buf = Mid$(buf, 2)
                    msg = msg & buf & "以外では" & Term("ＨＰ", u) & "を回復出来ない。"
                Else
                    msg = msg & buf & "では" & Term("ＨＰ", u) & "を回復出来ない。"
                End If
            Next
            msg = msg & buf & ";ただし、" & Term("スペシャルパワー", u) & _
                "や地形、母艦による回復は可能。"
            
        Case "霊力変換器"
            sname = p.SkillName0("霊力")
            msg = sname & "に合わせて各種能力が上昇する。"
            If flevel_specified Then
                msg = msg & _
                    ";（" & sname & "上限 = " & Format$(flevel) & "）"
            End If
            
        Case "オーラ変換器"
            sname = p.SkillName0("オーラ")
            msg = sname & "レベルに合わせて各種能力が上昇する。"
            If flevel_specified Then
                msg = msg & _
                    ";（" & sname & "上限レベル = " & Format$(flevel) & "）"
            End If
            
        Case "サイキックドライブ"
            sname = p.SkillName0("超能力")
            msg = sname & "レベルごとに" & Term("装甲", u) & "+100、" & Term("運動性", u) & "+5"
            If flevel_specified Then
                msg = msg & _
                    ";（" & sname & "上限レベル = " & Format$(flevel) & "）"
            End If
            
        Case "シンクロドライブ"
            sname = p.SkillName0("同調率")
            msg = sname & "に合わせて各種能力が上昇する。"
            If flevel_specified Then
                msg = msg & ";（" & sname & "上限 = " & Format$(flevel) & "%）"
            End If
            
        Case "ステルス"
            If flevel_specified Then
                msg = "敵から" & StrConv(Format$(flevel), vbWide) & _
                    "マス以内にいない限り発見されない。" & _
                    "ただし自分から攻撃すると１ターン無効。"
            Else
                msg = "敵から３マス以内にいない限り発見されない。" & _
                    "ただし自分から攻撃すると１ターン無効。"
            End If
            
        Case "ステルス無効化"
            msg = "敵のステルス能力を無効化する。"
            
        Case "テレポート"
            msg = "テレポートを行い、" & Term("移動力", u) & Format$(u.Speed + flevel) & _
                "で地形を無視して移動。;"
            If LLength(fdata) > 1 Then
                If CInt(LIndex(fdata, 2)) > 0 Then
                    msg = msg & LIndex(fdata, 2) & Term("ＥＮ", u) & "消費。"
                End If
            Else
                msg = msg & "40" & Term("ＥＮ", u) & "消費。"
            End If
            
        Case "ジャンプ"
            msg = Term("移動力", u) & Format$(u.Speed + flevel) & _
                "で地上地形を無視しながらジャンプ移動。"
            If LLength(fdata) > 1 Then
                If StrToLng(LIndex(fdata, 2)) > 0 Then
                    msg = msg & ";" & LIndex(fdata, 2) & Term("ＥＮ", u) & "消費。"
                End If
            End If
            
        Case "水泳"
            msg = "水中を泳いで移動可能。深海等の深い水の地形に進入することが出来る。" & _
                "ただし水中での移動コストが１になる訳ではない。"
            
        Case "水上移動"
            msg = "水上に浮かんで移動可能。"
            
        Case "ホバー移動"
            msg = "空中に浮きながら移動することで砂漠と雪原の移動コストが１になる。" & _
                "また、水上移動も可能。ただし移動時に5" & Term("ＥＮ", u) & "消費。"
            
        Case "透過移動"
            msg = "障害物を無視して移動。"
            
        Case "すり抜け移動"
            msg = "敵ユニットがいるマスを通過可能。"
            
        Case "線路移動"
            msg = "線路上のみを移動可能。"
            
        Case "移動制限"
            msg = msg & LIndex(fdata, 2)
            For i = 3 To LLength(fdata)
                msg = msg & "、" & LIndex(fdata, i)
            Next
            msg = msg & "上のみを移動可能。"
            
        Case "進入不可"
            msg = msg & LIndex(fdata, 2)
            For i = 3 To LLength(fdata)
                msg = msg & "、" & LIndex(fdata, i)
            Next
            msg = msg & "には進入不可。"
            
        Case "地形適応"
            msg = msg & LIndex(fdata, 2)
            For i = 3 To LLength(fdata)
                msg = msg & "、" & LIndex(fdata, i)
            Next
            msg = msg & "における移動コストが１になる。"
            
        Case "追加移動力"
            msg = LIndex(fdata, 2) & "にいると、" & Term("移動力", u) & "が"
            If flevel >= 0 Then
                msg = msg & Format$(flevel) & "増加。"
            Else
                msg = msg & Format$(-flevel) & "減少。"
            End If
            
        Case "母艦"
            msg = "他のユニットを格納し、修理・運搬可能。"
            
        Case "格納不可"
            msg = "母艦に格納することが出来ない。"
            
        Case "両手利き"
            msg = "両手に武器を装備可能。"
            
        Case "部隊ユニット"
            msg = "複数のユニットによって構成された部隊ユニット。"
            
        Case "召喚ユニット"
            msg = "召喚されたユニット。"
            
        Case "変形"
            If u.IsHero Then
                buf = "変化"
            Else
                buf = "変形"
            End If
            If LLength(fdata) > 2 Then
                msg = "以下の形態に" & buf & "; "
                For i = 2 To LLength(fdata)
                    If u.OtherForm(LIndex(fdata, i)).IsAvailable() Then
                        If u.Nickname = UDList.Item(LIndex(fdata, i)).Nickname Then
                            uname = UDList.Item(LIndex(fdata, i)).Name
                            If Right$(uname, 5) = "(前期型)" Then
                                uname = Left$(uname, Len(uname) - 5)
                            ElseIf Right$(uname, 5) = "・前期型)" Then
                                uname = Left$(uname, Len(uname) - 5) & ")"
                            ElseIf Right$(uname, 5) = "(後期型)" Then
                                uname = Left$(uname, Len(uname) - 5)
                            End If
                        Else
                            uname = UDList.Item(LIndex(fdata, i)).Nickname
                        End If
                        msg = msg & uname & "  "
                    End If
                Next
            Else
                If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
                    uname = UDList.Item(LIndex(fdata, 2)).Name
                Else
                    uname = UDList.Item(LIndex(fdata, 2)).Nickname
                End If
                If Right$(uname, 5) = "(前期型)" Then
                    uname = Left$(uname, Len(uname) - 5)
                ElseIf Right$(uname, 5) = "・前期型)" Then
                    uname = Left$(uname, Len(uname) - 5) & ")"
                ElseIf Right$(uname, 5) = "(後期型)" Then
                    uname = Left$(uname, Len(uname) - 5)
                End If
                msg = "<B>" & uname & "</B>に" & buf & "。"
            End If
            
        Case "パーツ分離"
            If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
                uname = UDList.Item(LIndex(fdata, 2)).Name
            Else
                uname = UDList.Item(LIndex(fdata, 2)).Nickname
            End If
            If Right$(uname, 5) = "(前期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            ElseIf Right$(uname, 5) = "・前期型)" Then
                uname = Left$(uname, Len(uname) - 5) & ")"
            ElseIf Right$(uname, 5) = "(後期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            End If
            msg = "パーツを分離し" & uname & "に変形。"
            If flevel_specified Then
                msg = msg & ";ユニット破壊時に" & Format$(10 * flevel) & "%の確率で発動。"
            End If
            
        Case "パーツ合体"
            If u.Nickname = UDList.Item(fdata).Nickname Then
                uname = UDList.Item(fdata).Name
            Else
                uname = UDList.Item(fdata).Nickname
            End If
            If Right$(uname, 5) = "(前期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            ElseIf Right$(uname, 5) = "・前期型)" Then
                uname = Left$(uname, Len(uname) - 5) & ")"
            ElseIf Right$(uname, 5) = "(後期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            End If
            msg = "パーツと合体し" & uname & "に変形。"
            
        Case "ハイパーモード"
            If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
                uname = UDList.Item(LIndex(fdata, 2)).Name
            Else
                uname = UDList.Item(LIndex(fdata, 2)).Nickname
            End If
            If Right$(uname, 5) = "(前期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            ElseIf Right$(uname, 5) = "・前期型)" Then
                uname = Left$(uname, Len(uname) - 5) & ")"
            ElseIf Right$(uname, 5) = "(後期型)" Then
                uname = Left$(uname, Len(uname) - 5)
            End If
            If u.Nickname <> uname Then
                uname = "<B>" & uname & "</B>"
            Else
                uname = ""
            End If
            If InStr(fdata, "気力発動") > 0 Then
                msg = Term("気力", u) & Format$(100 + 10 * flevel) & "で特殊形態" & uname & "に"
            ElseIf flevel <= 5 Then
                msg = Term("気力", u) & Format$(100 + 10 * flevel) & "、" & _
                    "もしくは" & Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & _
                    "の1/4以下で特殊形態" & uname & "に"
            Else
                msg = Term("ＨＰ", u) & "が最大" & Term("ＨＰ", u) & _
                    "の1/4以下で特殊形態" & uname & "に"
            End If
            If InStr(fdata, "自動発動") > 0 Then
                msg = msg & "自動"
            End If
            If u.IsHero Then
                msg = msg & "変身。"
            Else
                msg = msg & "変形。"
            End If
            
        Case "合体"
            If u.IsHero Then
                buf = "変化。"
            Else
                buf = "変形。"
            End If
            If LLength(fdata) > 3 Then
                If UDList.IsDefined(LIndex(fdata, 2)) Then
                    msg = "以下のユニットと合体し<B>" & _
                        UDList.Item(LIndex(fdata, 2)).Nickname & "</B>に" & buf & "; "
                Else
                    msg = "以下のユニットと合体し<B>" & _
                        LIndex(fdata, 2) & "</B>に" & buf & "; "
                End If
                
                For i = 3 To LLength(fdata)
                    If UDList.IsDefined(LIndex(fdata, i)) Then
                        msg = msg _
                            & UDList.Item(LIndex(fdata, i)).Nickname & "  "
                    Else
                        msg = msg _
                            & LIndex(fdata, i) & "  "
                    End If
                Next
            Else
                If UDList.IsDefined(LIndex(fdata, 3)) Then
                    msg = UDList.Item(LIndex(fdata, 3)).Nickname & "と合体し"
                Else
                    msg = LIndex(fdata, 3) & "と合体し"
                End If
                If UDList.IsDefined(LIndex(fdata, 2)) Then
                    msg = msg & _
                        UDList.Item(LIndex(fdata, 2)).Nickname & "に" & buf
                Else
                    msg = msg & _
                        LIndex(fdata, 2) & "に" & buf
                End If
            End If
            
        Case "分離"
            msg = "以下のユニットに分離。; "
            For i = 2 To LLength(fdata)
                If UDList.IsDefined(LIndex(fdata, i)) Then
                    msg = msg & _
                        UDList.Item(LIndex(fdata, i)).Nickname & "  "
                Else
                    msg = msg & _
                        LIndex(fdata, i) & "  "
                End If
            Next
            
        Case "不安定"
            msg = Term("ＨＰ", u) & "が最大値の1/4以下になると暴走する。"
            
        Case "支配"
            If LLength(fdata) = 2 Then
                If Not PDList.IsDefined(LIndex(fdata, 2)) Then
                    ErrorMessage "支配対象のパイロット「" & LIndex(fdata, 2) _
                        & "」のデータが定義されていません"
                    Exit Function
                End If
                msg = PDList.Item(LIndex(fdata, 2)).Nickname & _
                    "の存在を維持し、仕えさせている。"
            Else
                msg = "以下のユニットの存在を維持し、仕えさせている。;"
                For i = 2 To LLength(fdata)
                    If Not PDList.IsDefined(LIndex(fdata, 2)) Then
                        ErrorMessage "支配対象のパイロット「" & LIndex(fdata, i) _
                            & "」のデータが定義されていません"
                        Exit Function
                    End If
                    msg = msg & _
                        PDList.Item(LIndex(fdata, i)).Nickname & "  "
                Next
            End If
            
        Case "ＥＣＭ"
            msg = "半径３マス以内の味方ユニットに対する攻撃の命中率を元の"
            If flevel >= 0 Then
                msg = msg & Format$(100 - 5 * flevel) & "%に減少させる。"
            Else
                msg = msg & Format$(100 - 5 * flevel) & "%に増加させる。"
            End If
            buf = fname
            If InStr(buf, "Lv") Then
                buf = Left$(buf, InStr(buf, "Lv") - 1)
            End If
            msg = msg & "同時に相手の" & buf & "能力の効果を無効化。"
            msg = msg & ";思念誘導攻撃や近接攻撃には無効。"
            
        Case "ブースト"
            If IsOptionDefined("ダメージ倍率低下") Then
                msg = Term("気力", u) & "130以上で発動し、ダメージを 20% アップ。"
            Else
                msg = Term("気力", u) & "130以上で発動し、ダメージを 25% アップ。"
            End If
            
        Case "防御不可"
            msg = "攻撃を受けた際に防御運動を取ることが出来ない。"
            
        Case "回避不可"
            msg = "攻撃を受けた際に回避運動を取ることが出来ない。"
            
        Case "格闘強化"
            If flevel >= 0 Then
                msg = "パイロットの" & Term("格闘", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
            Else
                msg = "パイロットの" & Term("格闘", u) & "を" & Format$(CInt(5 * flevel)) & "。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "射撃強化"
            If p.HasMana() Then
                If flevel >= 0 Then
                    msg = "パイロットの" & Term("魔力", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
                Else
                    msg = "パイロットの" & Term("魔力", u) & "を" & Format$(CInt(5 * flevel)) & "。"
                End If
            Else
                If flevel >= 0 Then
                    msg = "パイロットの" & Term("射撃", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
                Else
                    msg = "パイロットの" & Term("射撃", u) & "を" & Format$(CInt(5 * flevel)) & "。"
                End If
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "命中強化"
            If flevel >= 0 Then
                msg = "パイロットの" & Term("命中", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
            Else
                msg = "パイロットの" & Term("命中", u) & "を" & Format$(CInt(5 * flevel)) & "。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & _
                    "気力" & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "回避強化"
            If flevel >= 0 Then
                msg = "パイロットの" & Term("回避", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
            Else
                msg = "パイロットの" & Term("回避", u) & "を" & Format$(CInt(5 * flevel)) & "。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "技量強化"
            If flevel >= 0 Then
                msg = "パイロットの" & Term("技量", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
            Else
                msg = "パイロットの" & Term("技量", u) & "を" & Format$(CInt(5 * flevel)) & "。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "反応強化"
            If flevel >= 0 Then
                msg = "パイロットの" & Term("反応", u) & "を+" & Format$(CInt(5 * flevel)) & "。"
            Else
                msg = "パイロットの" & Term("反応", u) & "を" & Format$(CInt(5 * flevel)) & "。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "ＨＰ強化"
            If flevel >= 0 Then
                msg = "最大" & Term("ＨＰ", u) & "を" & Format$(CInt(200 * flevel)) & "増加。"
            Else
                msg = "最大" & Term("ＨＰ", u) & "を" & Format$(CInt(-200 * flevel)) & "減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "ＥＮ強化"
            If flevel >= 0 Then
                msg = "最大" & Term("ＥＮ", u) & "を" & Format$(CInt(10 * flevel)) & "増加。"
            Else
                msg = "最大" & Term("ＥＮ", u) & "を" & Format$(CInt(-10 * flevel)) & "減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "装甲強化"
            If flevel >= 0 Then
                msg = Term("装甲", u) & "を" & Format$(CInt(100 * flevel)) & "増加。"
            Else
                msg = Term("装甲", u) & "を" & Format$(CInt(-100 * flevel)) & "減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "運動性強化"
            If flevel >= 0 Then
                msg = Term("運動性", u) & "を" & Format$(CInt(5 * flevel)) & "増加。"
            Else
                msg = Term("運動性", u) & "を" & Format$(CInt(-5 * flevel)) & "減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "移動力強化"
            If flevel >= 0 Then
                msg = Term("移動力", u) & "を" & Format$(CInt(flevel)) & "増加。"
            Else
                msg = Term("移動力", u) & "を" & Format$(CInt(flevel)) & "減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "ＨＰ割合強化"
            If flevel >= 0 Then
                msg = "最大" & Term("ＨＰ", u) & "を" & Format$(CInt(5 * flevel)) & "%分増加。"
            Else
                msg = "最大" & Term("ＨＰ", u) & "を" & Format$(CInt(-5 * flevel)) & "%分減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "ＥＮ割合強化"
            If flevel >= 0 Then
                msg = "最大" & Term("ＥＮ", u) & "を" & Format$(CInt(5 * flevel)) & "%分増加。"
            Else
                msg = "最大" & Term("ＥＮ", u) & "を" & Format$(CInt(-5 * flevel)) & "%分減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "装甲割合強化"
            If flevel >= 0 Then
                msg = Term("装甲", u) & "を" & Format$(CInt(5 * flevel)) & "%分増加。"
            Else
                msg = Term("装甲", u) & "を" & Format$(CInt(-5 * flevel)) & "%分減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "運動性割合強化"
            If flevel >= 0 Then
                msg = Term("運動性", u) & "を" & Format$(CInt(5 * flevel)) & "%分増加。"
            Else
                msg = Term("運動性", u) & "を" & Format$(CInt(-5 * flevel)) & "%分減少。"
            End If
            If IsNumeric(LIndex(fdata, 2)) Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "以上で発動。"
            End If
            
        Case "武器・防具クラス"
            fdata = Trim$(u.WeaponProficiency)
            If fdata <> "" Then
                msg = "武器【" & fdata & "】;"
            Else
                msg = "武器【-】;"
            End If
            fdata = Trim$(u.ArmorProficiency)
            If fdata <> "" Then
                msg = msg & "防具【" & fdata & "】"
            Else
                msg = msg & "防具【-】"
            End If
            
        Case "追加攻撃"
            If LIndex(fdata, 3) <> "全" Then
                buf = LIndex(fdata, 3)
                If Left$(buf, 1) = "@" Then
                    msg = Mid$(buf, 2) & "による"
                Else
                    msg = "「" & buf & "」属性を持つ武器による"
                End If
            End If
            
            msg = msg & "攻撃の後に、"
            
            buf = LIndex(fdata, 4)
            If IsNumeric(buf) Then
                If buf <> "100" Then
                    msg = msg & buf & "%の確率で"
                End If
            ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then
                i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
                sname = u.SkillName0(Left$(buf, i - 1))
                prob = (u.SkillLevel(Left$(buf, i - 1)) + CInt(Mid$(buf, i))) * 100 \ 16
                msg = msg & _
                    "(" & sname & "Lv" & Mid$(buf, i) & ")/16の確率(" & _
                    Format$(prob) & "%)で"
            Else
                sname = u.SkillName0(buf)
                prob = u.SkillLevel(buf) * 100 \ 16
                msg = msg & _
                    sname & "Lv/16の確率(" & Format$(prob) & "%)で"
            End If
            
            buf = LIndex(fdata, 2)
            If InStr(buf, "(") > 0 Then
                buf = Left$(buf, InStr(buf, "(") - 1)
            End If
            msg = msg & buf & "による追撃を行う。"
            
            If StrToLng(LIndex(fdata, 5)) > 0 Then
                msg = msg & ";発動時に" & LIndex(fdata, 5) & "ＥＮ消費。"
            ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then
                msg = msg & ";発動時に" & Mid$(LIndex(fdata, 5), 2) & "ＥＮ増加。"
            End If
            If StrToLng(LIndex(fdata, 6)) > 50 Then
                msg = msg & ";" & Term("気力", u) & LIndex(fdata, 6) & "以上で使用可能。"
            End If
            If InStr(fdata, "連鎖不可") > 0 Then
                msg = msg & "連鎖不可。"
            End If
            
        Case "ＺＯＣ"
            If u.FeatureLevel("ＺＯＣ") < 0 Then
                msg = "このユニットはＺＯＣによる影響を与えることが出来ない。"
            Else
                msg = "このユニットから"
                If LLength(fdata) < 2 Then
                    buf = "1"
                Else
                    buf = LIndex(fdata, 2)
                End If
                
                opt = LIndex(fdata, 3)
                If InStr(opt, "直線") > 0 Then
                    msg = msg & buf & "マス以内の直線上"
                ElseIf InStr(opt, " 水平") > 0 Then
                    msg = msg & "左右" & buf & "マス以内の直線上"
                ElseIf InStr(opt, " 垂直") > 0 Then
                    msg = msg & "上下" & buf & "マス以内の直線上"
                Else
                    msg = msg & buf & "マス以内"
                End If
                msg = msg & "を通過する敵ユニットに、ＺＯＣによる影響を与える。"
            End If
            
        Case "ＺＯＣ無効化"
            If flevel = 1 Then
                msg = "このユニットは敵ユニットによるＺＯＣの影響を受けない。"
            Else
                msg = "このユニットは敵ユニットによる" & Format(flevel) _
                    & "レベル以下のＺＯＣの影響を受けない。"
            End If
            
        Case "隣接ユニットＺＯＣ無効化"
            If flevel = 1 Then
                msg = "このユニットが隣接する敵ユニットによるＺＯＣを無効化する。"
            Else
                msg = "このユニットが隣接する敵ユニットによる" & Format(flevel) _
                    & "レベル以下のＺＯＣを無効化する。"
            End If
        
        Case "広域ＺＯＣ無効化"
            msg = "このユニットから"
            If LLength(fdata) < 2 Then
                buf = "1"
            Else
                buf = LIndex(fdata, 2)
            End If
            
            If flevel = 1 Then
                msg = msg & buf & "マス以内に設定されたＺＯＣの影響を無効化する。"
            Else
                msg = msg & buf & "マス以内に設定された" & Format(flevel) _
                    & "レベル以下のＺＯＣの影響を無効化する。"
            End If

' ADD START MARGE
        Case "地形効果無効化"
            If LLength(fdata) > 1 Then
                For i = 2 To LLength(fdata)
                    If i > 2 Then
                        msg = msg & "、"
                    End If
                    msg = msg & LIndex(fdata, i)
                Next
                msg = msg & "の"
            Else
                msg = msg & "全地形の"
            End If
            msg = msg & "ＨＰ・ＥＮ減少や状態付加等の特殊効果を無効化する。"
' ADD END MARGE
        
        Case Else
            If is_additional Then
                '付加された能力の場合、ユニット用特殊能力に該当しなければ
                'パイロット用特殊能力とみなす
                msg = SkillHelpMessage(u.MainPilot, ftype)
                If Len(msg) > 0 Then
                    Exit Function
                End If
                
                '実はダミー能力？
                If Len(fdata) > 0 Then
                    msg = ListIndex(fdata, ListLength(fdata))
                    If Left$(msg, 1) = """" Then
                        msg = Mid$(msg, 2, Len(msg) - 2)
                    End If
                End If
                
                '解説が存在しない？
                If Len(msg) = 0 Then
                    Exit Function
                End If
            ElseIf Len(fdata) > 0 Then
                'ダミー能力の場合
                msg = ListIndex(fdata, ListLength(fdata))
                If Left$(msg, 1) = """" Then
                    msg = Mid$(msg, 2, Len(msg) - 2)
                End If
            ElseIf ListIndex(u.AllFeatureData(fname), 1) <> "解説" Then
                '解説がない場合
                Exit Function
            End If
            
    End Select
    
    fdata = u.AllFeatureData(fname0)
    If ListIndex(fdata, 1) = "解説" Then
        '解説を定義している場合
        msg = ListTail(fdata, 2)
        If Left$(msg, 1) = """" Then
            msg = Mid$(msg, 2, Len(msg) - 2)
        End If
    End If
    
    '等身大基準の際は「パイロット」という語を使わないようにする
    If IsOptionDefined("等身大基準") Then
        ReplaceString msg, "パイロット", "ユニット"
    End If
    
    FeatureHelpMessage = msg
End Function

'ユニット u の武器＆アビリティ属性 atr の名称
Public Function AttributeName(u As Unit, atr As String, _
    Optional ByVal is_ability As Boolean) As String
Dim fdata As String
    
    Select Case atr
        Case "全"
            AttributeName = "全ての攻撃"
        Case "格"
            AttributeName = "格闘系攻撃"
        Case "射"
            AttributeName = "射撃系攻撃"
        Case "複"
            AttributeName = "複合技"
        Case "Ｐ"
            AttributeName = "移動後使用可能攻撃"
        Case "Ｑ"
            AttributeName = "移動後使用不能攻撃"
        Case "Ｒ"
            AttributeName = "低改造武器"
        Case "改"
            AttributeName = "低改造武器"
        Case "攻"
            AttributeName = "攻撃専用"
        Case "反"
            AttributeName = "反撃専用"
        Case "武"
            AttributeName = "格闘武器"
        Case "突"
            AttributeName = "突進技"
        Case "接"
            AttributeName = "接近戦攻撃"
        Case "Ｊ"
            AttributeName = "ジャンプ攻撃"
        Case "Ｂ"
            AttributeName = "ビーム攻撃"
        Case "実"
            AttributeName = "実弾攻撃"
        Case "オ"
            AttributeName = "オーラ技"
        Case "超"
            AttributeName = "サイキック攻撃"
        Case "シ"
            AttributeName = "同調率対象攻撃"
        Case "サ"
            AttributeName = "思念誘導攻撃"
        Case "体"
            AttributeName = "生命力換算攻撃"
        Case "吸"
            AttributeName = Term("ＨＰ", u) & "吸収攻撃"
        Case "減"
            AttributeName = Term("ＥＮ", u) & "破壊攻撃"
        Case "奪"
            AttributeName = Term("ＥＮ", u) & "吸収攻撃"
        Case "貫"
            AttributeName = "貫通攻撃"
        Case "無"
            AttributeName = "バリア無効化攻撃"
        Case "浄"
            AttributeName = "浄化技"
        Case "封"
            AttributeName = "封印技"
        Case "限"
            AttributeName = "限定技"
        Case "殺"
            AttributeName = "抹殺攻撃"
        Case "浸"
            AttributeName = "浸蝕攻撃"
        Case "破"
            AttributeName = "シールド貫通攻撃"
        Case "♂"
            AttributeName = "対男性用攻撃"
        Case "♀"
            AttributeName = "対女性用攻撃"
        Case "Ａ"
            AttributeName = "自動充填式攻撃"
        Case "Ｃ"
            AttributeName = "チャージ式攻撃"
        Case "合"
            AttributeName = "合体技"
        Case "共"
            If Not is_ability Then
                AttributeName = "弾薬共有武器"
            Else
                AttributeName = "使用回数共有" & Term("アビリティ", u)
            End If
        Case "斉"
            AttributeName = "一斉発射"
        Case "永"
            AttributeName = "永続武器"
        Case "術"
            AttributeName = "術"
        Case "技"
            AttributeName = "技"
        Case "視"
            AttributeName = "視覚攻撃"
        Case "音"
            If Not is_ability Then
                AttributeName = "音波攻撃"
            Else
                AttributeName = "音波" & Term("アビリティ", u)
            End If
        Case "気"
            AttributeName = Term("気力", u) & "消費攻撃"
        Case "霊", "プ"
            AttributeName = "霊力消費攻撃"
        Case "失"
            AttributeName = Term("ＨＰ", u) & "消費攻撃"
        Case "銭"
            AttributeName = Term("資金", u) & "消費攻撃"
        Case "消"
            AttributeName = "消耗技"
        Case "自"
            AttributeName = "自爆攻撃"
        Case "変"
            AttributeName = "変形技"
        Case "間"
            AttributeName = "間接攻撃"
        Case "Ｍ直"
            AttributeName = "直線型マップ攻撃"
        Case "Ｍ拡"
            AttributeName = "拡散型マップ攻撃"
        Case "Ｍ扇"
            AttributeName = "扇型マップ攻撃"
        Case "Ｍ全"
            AttributeName = "全方位型マップ攻撃"
        Case "Ｍ投"
            AttributeName = "投下型マップ攻撃"
        Case "Ｍ移"
            AttributeName = "移動型マップ攻撃"
        Case "Ｍ線"
            AttributeName = "線状マップ攻撃"
        Case "識"
            AttributeName = "識別型マップ攻撃"
        Case "縛"
            AttributeName = "捕縛攻撃"
        Case "Ｓ"
            AttributeName = "ショック攻撃"
        Case "劣"
            AttributeName = "装甲劣化攻撃"
        Case "中"
            AttributeName = "バリア中和攻撃"
        Case "石"
            AttributeName = "石化攻撃"
        Case "凍"
            AttributeName = "凍結攻撃"
        Case "痺"
            AttributeName = "麻痺攻撃"
        Case "眠"
            AttributeName = "催眠攻撃"
        Case "乱"
            AttributeName = "混乱攻撃"
        Case "魅"
            AttributeName = "魅了攻撃"
        Case "憑"
            AttributeName = "憑依攻撃"
        Case "盲"
            AttributeName = "目潰し攻撃"
        Case "毒"
            AttributeName = "毒攻撃"
        Case "撹"
            AttributeName = "撹乱攻撃"
        Case "恐"
            AttributeName = "恐怖攻撃"
        Case "不"
            AttributeName = "攻撃封印攻撃"
        Case "止"
            AttributeName = "足止め攻撃"
        Case "黙"
            AttributeName = "沈黙攻撃"
        Case "除"
            AttributeName = "特殊効果除去攻撃"
        Case "即"
            AttributeName = "即死攻撃"
        Case "告"
            AttributeName = "死の宣告"
        Case "脱"
            AttributeName = Term("気力", u) & "減少攻撃"
        Case "Ｄ"
            AttributeName = Term("気力", u) & "吸収攻撃"
        Case "低攻"
            AttributeName = "攻撃力低下攻撃"
        Case "低防"
            AttributeName = "防御力低下攻撃"
        Case "低運"
            AttributeName = Term("運動性", u) & "低下攻撃"
        Case "低移"
            AttributeName = Term("移動力", u) & "低下攻撃"
        Case "精"
            AttributeName = "精神攻撃"
        Case "先"
            AttributeName = "先制攻撃"
        Case "後"
            AttributeName = "後攻攻撃"
        Case "連"
            AttributeName = "連続攻撃"
        Case "再"
            AttributeName = "再攻撃"
        Case "吹"
            AttributeName = "吹き飛ばし攻撃"
        Case "Ｋ"
            AttributeName = "ノックバック攻撃"
        Case "引"
            AttributeName = "引き寄せ攻撃"
        Case "転"
            AttributeName = "強制転移攻撃"
        Case "忍"
            AttributeName = "暗殺技"
        Case "尽"
            AttributeName = "全" & Term("ＥＮ", u) & "消費攻撃"
        Case "盗"
            AttributeName = "盗み"
        Case "Ｈ"
            AttributeName = "ホーミング攻撃"
        Case "追"
            AttributeName = "自己追尾攻撃"
        Case "有"
            AttributeName = "有線式誘導攻撃"
        Case "誘"
            AttributeName = "特殊誘導攻撃"
        Case "爆"
            AttributeName = "爆発攻撃"
        Case "空"
            AttributeName = "対空攻撃"
        Case "固"
            AttributeName = "ダメージ固定攻撃"
        Case "衰"
            AttributeName = Term("ＨＰ", u) & "減衰攻撃"
        Case "滅"
            AttributeName = Term("ＥＮ", u) & "減衰攻撃"
        Case "踊"
            AttributeName = "踊らせ攻撃"
        Case "狂"
            AttributeName = "狂戦士化攻撃"
        Case "ゾ"
            AttributeName = "ゾンビ化攻撃"
        Case "害"
            AttributeName = "回復能力阻害攻撃"
        Case "習"
            AttributeName = "ラーニング"
        Case "写"
            AttributeName = "能力コピー"
        Case "化"
            AttributeName = "変化"
        Case "痛"
            AttributeName = "クリティカル"
        Case "援"
            AttributeName = "支援専用" & Term("アビリティ", u)
        Case "難"
            AttributeName = "高難度" & Term("アビリティ", u)
        Case "地", "水", "火", "風", "冷", "雷", "光", "闇", "聖", "死", "木"
            AttributeName = atr & "属性"
        Case "魔"
            AttributeName = "魔法攻撃"
        Case "時"
            AttributeName = "時間操作攻撃"
        Case "重"
            AttributeName = "重力攻撃"
        Case "銃", "剣", "刀", "槍", "斧", "弓"
            AttributeName = atr & "攻撃"
        Case "銃"
        Case "機"
            AttributeName = "対機械用攻撃"
        Case "感"
            AttributeName = "対エスパー用攻撃"
        Case "竜"
            AttributeName = "竜殺しの武器"
        Case "瀕"
            AttributeName = "瀕死時限定攻撃"
        Case "対"
            AttributeName = "特定レベル限定攻撃"
        Case "ラ"
            AttributeName = "ラーニング可能技"
        Case "禁"
            AttributeName = "使用禁止"
        Case "小"
            AttributeName = "最小射程"
        Case "散"
            AttributeName = "拡散攻撃"
        Case Else
            If Left$(atr, 1) = "弱" Then
                AttributeName = Mid$(atr, 2) & "属性弱点付加攻撃"
            ElseIf Left$(atr, 1) = "効" Then
                AttributeName = Mid$(atr, 2) & "属性有効付加攻撃"
            ElseIf Left$(atr, 1) = "剋" Then
                AttributeName = Mid$(atr, 2) & "属性使用妨害攻撃"
            End If
    End Select
    
    If Not u Is Nothing Then
        fdata = u.FeatureData(atr)
        If ListIndex(fdata, 1) = "解説" Then
            '解説を定義している場合
            AttributeName = ListIndex(fdata, 2)
            Exit Function
        End If
    End If
    
    If is_ability Then
        If Right$(AttributeName, 2) = "攻撃" _
            Or Right$(AttributeName, 2) = "武器" _
        Then
            AttributeName = Left$(AttributeName, Len(AttributeName) - 2) _
                & Term("アビリティ", u)
        End If
    End If
End Function

'ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
Public Sub AttributeHelp(u As Unit, atr As String, ByVal idx As Integer, _
    Optional ByVal is_ability As Boolean)
Dim msg As String, aname As String
Dim prev_mode As Boolean
    
    msg = AttributeHelpMessage(u, atr, idx, is_ability)
    
    '解説の表示
    If Len(msg) > 0 Then
        prev_mode = AutoMessageMode
        AutoMessageMode = False
        
        OpenMessageForm
        If AutoMoveCursor Then
            MoveCursorPos "メッセージウィンドウ"
        End If
        If InStr(atr, "L") > 0 Then
            aname = AttributeName(u, Left$(atr, InStr(atr, "L") - 1), is_ability) & _
                "レベル" & StrConv(Format$(Mid$(atr, InStr(atr, "L") + 1)), vbWide)
        Else
            aname = AttributeName(u, atr, is_ability)
        End If
        DisplayMessage "システム", "<b>" & aname & "</b>;" & msg
        CloseMessageForm
        
        AutoMessageMode = prev_mode
    End If
End Sub

'ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
Public Function AttributeHelpMessage(u As Unit, atr As String, ByVal idx As Integer, _
    ByVal is_ability As Boolean) As String
Dim atype As String, alevel As Double
Dim msg As String, whatsthis As String
Dim waname As String, wanickname As String, uname As String
Dim p As Pilot
Dim i As Integer, j As Integer, buf As String
Dim fdata As String
    
    '属性レベルの収得
    If InStr(atr, "L") > 0 Then
        atype = Left$(atr, InStr(atr, "L") - 1)
        alevel = CDbl(Mid$(atr, InStr(atr, "L") + 1))
    Else
        atype = atr
        alevel = DEFAULT_LEVEL
    End If
    
    With u
        '武器(アビリティ)名
        If Not is_ability Then
            waname = .Weapon(idx).Name
            wanickname = .WeaponNickname(idx)
            whatsthis = "攻撃"
        Else
            waname = .Ability(idx).Name
            wanickname = .AbilityNickname(idx)
            whatsthis = Term("アビリティ", u)
        End If
        
        'メインパイロット
        Set p = .MainPilot
    End With
    
    Select Case atype
        Case "格"
            msg = "パイロットの" & Term("格闘", u) & "を使って攻撃力を算出。"
        Case "射"
            If p.HasMana() Then
                msg = "パイロットの" & Term("魔力", u) & "を使って攻撃力を算出。"
            Else
                msg = "パイロットの" & Term("射撃", u) & "を使って攻撃力を算出。"
            End If
        Case "複"
            If p.HasMana() Then
                msg = "格闘と魔法の両方を使った攻撃。" & _
                    "パイロットの" & Term("格闘", u) & "と" & Term("魔力", u) & "の" & _
                    "平均値を使って攻撃力を算出する。"
            Else
                msg = "格闘と射撃の両方を使った攻撃。" & _
                    "パイロットの" & Term("格闘", u) & "と" & Term("射撃", u) & "の" & _
                    "平均値を使って攻撃力を算出する。"
            End If
        Case "Ｐ"
            msg = "射程にかかわらず移動後に使用可能。"
        Case "Ｑ"
            msg = "射程にかかわらず移動後は使用不能。"
        Case "攻"
            msg = "攻撃時にのみ使用可能。"
        Case "反"
            msg = "反撃時にのみ使用可能。"
        Case "Ｒ"
            If alevel = DEFAULT_LEVEL Then
                msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。"
            Else
                msg = "ユニットランクや特殊能力による攻撃力上昇が" & Format$(10 * alevel) & _
                    "％になる。"
            End If
            msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。"
        Case "改"
            If alevel = DEFAULT_LEVEL Then
                msg = "ユニットランクによる攻撃力上昇が通常の半分。"
            Else
                msg = "ユニットランクによる攻撃力上昇が" & Format$(10 * alevel) & _
                    "％になる。"
            End If
        Case "武"
            msg = "この武器を使って実弾攻撃などを切り払うことが可能。" & _
                "切り払いの対象になる。"
        Case "突"
            msg = "切り払いの対象になる。"
        Case "接"
            msg = "投げ技等、相手に密着して繰り出す格闘戦攻撃。;" & _
                "切り払い無効。"
        Case "Ｊ"
            msg = "ジャンプ攻撃時の地形適応を指定したレベルだけ上げる。"
        Case "Ｂ"
            msg = "対ビーム用防御能力の対象になる。"
        Case "実"
            msg = "切り払いと迎撃の対象になる。"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際もダメージが低下しない。"
            End If
        Case "オ"
            msg = "パイロットの" & p.SkillName0("オーラ") & "レベルによって攻撃力が変化。"
        Case "超"
            msg = "パイロットの" & p.SkillName0("超能力") & "レベルによって攻撃力が変化。"
        Case "シ"
            msg = "パイロットの" & p.SkillName0("同調率") & "によって攻撃力が変化。"
        Case "サ"
            msg = "パイロットの" & p.SkillName0("超感覚") & "レベルによって射程が変化。"
            If IsOptionDefined("距離修正") Then
                msg = msg & "距離による命中率低下がない。また、"
            End If
            msg = msg & "ＥＣＭによる影響を受けない。"
        Case "体"
            msg = "生命力を攻撃力に換える攻撃。ユニットの" & Term("ＨＰ", u) & _
                "によって攻撃力が変化する。"
        Case "吸"
            msg = "与えたダメージの１／４を吸収し、自分の" & Term("ＨＰ", u) & "に変換。"
        Case "減"
            msg = Term("ＨＰ", u) & "にダメージを与えると同時に相手の" & _
                Term("ＥＮ", u) & "を減少させる。"
        Case "奪"
            msg = Term("ＨＰ", u) & "にダメージを与えると同時に相手の" & _
                Term("ＥＮ", u) & "を減少させ、" & _
                "減少させた" & Term("ＥＮ", u) & "の半分を自分のものにする。"
        Case "貫"
            If alevel > 0 Then
                msg = "相手の" & Term("装甲", u) & "を本来の" & Format$(100 - 10 * alevel) & _
                    "％の値とみなしてダメージ計算を行う。"
            Else
                msg = "相手の" & Term("装甲", u) & "を半分とみなしてダメージ計算を行う。"
            End If
        Case "無"
            msg = "バリアやフィールドなどの防御能力の効果を無視してダメージを与える。"
        Case "浸"
            msg = "シールド防御を無視してダメージを与える。"
        Case "破"
            msg = "シールド防御の効果を半減させる。"
        Case "浄"
            msg = "敵の" & p.SkillName0("再生") & "能力を無効化。"
        Case "封"
            msg = "特定の弱点を持つ敵にのみ有効な武装。" & _
                "弱点をついたときにのみダメージを与えることが出来る。"
        Case "限"
            msg = "特定の弱点を持つ敵にのみ有効な武装。" & _
                "限定属性以降に指定した属性で;" & _
                "弱点をついたときにのみダメージを与えることが出来る。"
        Case "殺"
            msg = "相手を一撃で倒せる場合にのみ有効な攻撃。;" & _
                "相手は防御＆シールド防御出来ない。"
        Case "♂"
            msg = "男性にのみ有効。"
        Case "♀"
            msg = "女性にのみ有効。"
        Case "Ｃ"
            msg = "チャージコマンドを使用してチャージ完了の状態にならないと使用不能。"
        Case "Ａ"
            msg = "使用すると" & Format$(alevel) & _
                "ターン後に再チャージが完了するまで使用不能。"
            If Not is_ability Then
                For i = 1 To u.CountWeapon
                    If i <> idx And wanickname = u.WeaponNickname(i) Then
                        msg = msg & "同名の武器も連動して使用不能になる。"
                        Exit For
                    End If
                Next
                If u.IsWeaponClassifiedAs(idx, "共") _
                    And u.Weapon(idx).Bullet = 0 _
                Then
                    msg = msg & "同レベルの弾薬共有武器も連動して使用不能になる。"
                End If
            Else
                For i = 1 To u.CountAbility
                    If i <> idx And wanickname = u.AbilityNickname(i) Then
                        msg = msg & "同名の" & Term("アビリティ", u) & "も連動して使用不能になる。"
                        Exit For
                    End If
                Next
                If u.IsAbilityClassifiedAs(idx, "共") _
                    And u.Ability(idx).Stock = 0 _
                Then
                    msg = msg & "同レベルの使用回数共有" & Term("アビリティ", u) & _
                         "も連動して使用不能になる。"
                End If
            End If
        Case "合"
            For i = 1 To u.CountFeature
                If u.Feature(i) = "合体技" _
                    And LIndex(u.FeatureData(i), 1) = waname _
                Then
                    Exit For
                End If
            Next
            If i > u.CountFeature Then
                ErrorMessage u.Name & "の合体技「" & waname _
                    & "」に対応した合体技能力がありません"
                Exit Function
            End If
            If LLength(u.FeatureData(i)) = 2 Then
                uname = LIndex(u.FeatureData(i), 2)
                If UDList.IsDefined(uname) Then
                    uname = UDList.Item(uname).Nickname
                End If
                If uname = u.Nickname Then
                    msg = "他の" & uname & "と協力して行う技。"
                Else
                    msg = uname & "と協力して行う技。"
                End If
            Else
                msg = "以下のユニットと協力して行う技。;"
                For j = 2 To LLength(u.FeatureData(i))
                    uname = LIndex(u.FeatureData(i), j)
                    If UDList.IsDefined(uname) Then
                        uname = UDList.Item(uname).Nickname
                    End If
                    msg = msg & uname & "  "
                Next
            End If
        Case "共"
            If Not is_ability Then
                msg = "複数の武器で弾薬を共有していることを示す。"
                If alevel > 0 Then
                    msg = msg & ";同レベルの弾薬共有武器間で弾薬を共有している。"
                End If
            Else
                msg = "複数の" & Term("アビリティ", u) & "で使用回数を共有していることを示す。"
                If alevel > 0 Then
                    msg = msg & ";同レベルの使用回数共有" & Term("アビリティ", u) & _
                        "間で使用回数を共有している。"
                End If
            End If
        Case "斉"
            If Not is_ability Then
                msg = "弾数制の武器全ての弾数を消費して攻撃を行う。"
            Else
                msg = "回数制の" & Term("アビリティ", u) & "全ての使用回数を消費する。"
            End If
        Case "永"
            msg = "切り払いや迎撃されない限り弾数が減少しない。"
        Case "術"
            buf = p.SkillName0("術")
            If buf = "非表示" Then
                buf = "術"
            End If
            msg = buf & "技能によって" & Term("ＥＮ", u) & "消費量が減少。"
            If is_ability Then
                msg = msg & ";パイロットの" & Term("魔力", u) & "によって威力が増減する。"
            End If
            msg = msg & ";沈黙状態の時には使用不能｡"
        Case "技"
            buf = p.SkillName0("技")
            If buf = "非表示" Then
                buf = "技"
            End If
            msg = buf & "技能によって" & Term("ＥＮ", u) & "消費量が減少。"
        Case "音"
            If Not is_ability Then
                msg = "声などの音を使った攻撃であることを示す｡"
            Else
                msg = "声などの音を使った" & Term("アビリティ", u) & "であることを示す｡"
            End If
            msg = msg & "沈黙状態の時には使用不能｡ "
        Case "視"
            msg = "視覚に働きかける攻撃。盲目状態のユニットには効かない。"
        Case "気"
            msg = "使用時に気力" & Format$(5 * alevel) & "を消費。"
        Case "霊", "プ"
            msg = "使用時に" & Format$(5 * alevel) & p.SkillName0("霊力") & "を消費。"
        Case "失"
            msg = "使用時に" & Format$(alevel * u.MaxHP \ 10) & "の" & Term("ＨＰ", u) & "を失う。"
        Case "銭"
            msg = "使用時に" & Format$(MaxLng(alevel, 1) * u.Value \ 10) & _
                "の" & Term("資金", u) & "が必要。;" & _
                Term("資金", u) & "が足りない場合は使用不可。"
        Case "消"
            msg = "使用後に1ターン消耗状態に陥り、回避・反撃不能。"
        Case "尽"
            If Not is_ability Then
                If alevel > 0 Then
                    msg = _
                        "全" & Term("ＥＮ", u) & "を使って攻撃し、使用後に" & _
                        Term("ＥＮ", u) & "が0になる。;" & _
                        "(残り" & Term("ＥＮ", u) & "－必要" & Term("ＥＮ", u) & _
                        ")×" & StrConv(Format$(alevel), vbWide) & _
                        "だけ攻撃力が上昇。"
                Else
                    msg = "全" & Term("ＥＮ", u) & "を使って攻撃し、使用後にＥＮが0になる。"
                End If
            Else
                msg = "使用後に" & Term("ＥＮ", u) & "が0になる。"
            End If
        Case "自"
            msg = "使用後に自爆。"
        Case "変"
            If u.IsFeatureAvailable("変形技") Then
                For i = 1 To u.CountFeature
                    If u.Feature(i) = "変形技" _
                        And LIndex(u.FeatureData(i), 1) = waname _
                    Then
                        uname = LIndex(u.FeatureData(i), 2)
                        Exit For
                    End If
                Next
            End If
            If uname = "" Then
                uname = LIndex(u.FeatureData("ノーマルモード"), 1)
            End If
            If UDList.IsDefined(uname) Then
                With UDList.Item(uname)
                    If u.Nickname <> .Nickname Then
                        uname = .Nickname
                    Else
                        uname = .Name
                    End If
                End With
            End If
            msg = "使用後に" & uname & "へ変化する。"
        Case "間"
            msg = "視界外などから間接的に攻撃を行うことにより" & _
                "相手の反撃を封じる武器。"
        Case "Ｍ直"
            msg = "上下左右の一方向に対する直線状の効果範囲を持つ。"
        Case "Ｍ拡"
            msg = "上下左右の一方向に対する幅３マスの直線状の効果範囲を持つ。"
        Case "Ｍ扇"
            msg = "上下左右の一方向に対する扇状の効果範囲を持つ。;" & _
                "扇の広がり方の度合いはレベルによって異なる。"
        Case "Ｍ全"
            msg = "ユニットの周り全域に対する効果範囲を持つ。"
        Case "Ｍ投"
            msg = "指定した地点を中心とした一定範囲の効果範囲を持つ。"
        Case "Ｍ移"
            msg = "使用後に指定した地点までユニットが移動し、" & _
                "ユニットが通過した場所が効果範囲になる。"
        Case "Ｍ線"
            msg = "指定した地点とユニットを結ぶ直線が効果範囲になる。"
        Case "識"
            msg = "効果範囲内にいる味方ユニットを自動的に識別し、敵のみにダメージを与える。"
        Case "縛"
            If alevel = DEFAULT_LEVEL Then
                alevel = 2
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "行動不能にする。"
        Case "Ｓ"
            If alevel = DEFAULT_LEVEL Then
                alevel = 1
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "行動不能にする。"
        Case "劣"
            If alevel = DEFAULT_LEVEL Then
                msg = "クリティカル発生時に相手の装甲を半減させる。"
            Else
                msg = "クリティカル発生時に相手の装甲を"
                If alevel > 0 Then
                    msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                Else
                    msg = msg & "その戦闘中のみ"
                End If
                msg = msg & "半減させる。"
            End If
        Case "中"
            If alevel = DEFAULT_LEVEL Then
                alevel = 1
            End If
            msg = "クリティカル発生時に相手が持つバリア等の防御能力を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "無効化する。"
        Case "石"
            If alevel = DEFAULT_LEVEL Then
                msg = "クリティカル発生時に相手を石化させる。"
            Else
                msg = "クリティカル発生時に相手を"
                If alevel > 0 Then
                    msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                Else
                    msg = msg & "その戦闘中のみ"
                End If
                msg = msg & "石化させる。"
            End If
        Case "凍"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "凍らせる。"
            msg = msg & ";凍結した相手は" & Term("装甲", u) & "が半減するが、"
            msg = msg & "ダメージを与えると凍結は解除される。"
        Case "痺"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "麻痺させる。"
        Case "眠"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "眠らせる。"
            msg = msg & ";眠った相手への攻撃のダメージは１.５倍になるが、睡眠も解除される。"
            msg = msg & ";性格が機械の敵には無効。"
        Case "乱"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "混乱させる。"
        Case "魅"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "魅了する。"
        Case "憑"
            If alevel = DEFAULT_LEVEL Then
                msg = "クリティカル発生時に相手を乗っ取って支配する。"
            Else
                msg = "クリティカル発生時に相手を"
                If alevel > 0 Then
                    msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                Else
                    msg = msg & "その戦闘中のみ"
                End If
                msg = msg & "乗っ取って支配する。"
            End If
        Case "盲"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "盲目にする。"
        Case "毒"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "毒状態にする。"
        Case "撹"
            If alevel = DEFAULT_LEVEL Then
                alevel = 2
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "撹乱する。"
        Case "恐"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "恐怖に陥れる。"
        Case "不"
            If alevel = DEFAULT_LEVEL Then
                alevel = 1
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "攻撃不能にする。"
        Case "止"
            If alevel = DEFAULT_LEVEL Then
                alevel = 1
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "移動不能にする。"
        Case "黙"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "沈黙状態にする。"
        Case "除"
            If Not is_ability Then
                msg = "クリティカル発生時に相手にかけられた" & Term("アビリティ", u) & _
                    "による特殊効果を打ち消す。"
            Else
                msg = Term("アビリティ", u) & "実行時に、それまでに相手にかけられていた" & _
                    Term("アビリティ", u) & "による特殊効果が解除される。"
            End If
        Case "即"
            msg = "クリティカル発生時に相手を即死させる。"
        Case "告"
            If alevel > 0 Then
                msg = "クリティカル発生時に相手を「死の宣告」状態にし、" & _
                    StrConv(Format$(CInt(alevel)), vbWide) & "ターン後に" & _
                    Term("ＨＰ", u) & "を１にする。"
            Else
                msg = "クリティカル発生時に相手の" & Term("ＨＰ", u) & "を１にする。"
            End If
        Case "脱"
            If alevel = DEFAULT_LEVEL Then
                msg = "相手の" & Term("気力", u) & "を10低下させる。"
            ElseIf alevel >= 0 Then
                msg = "相手の" & Term("気力", u) & "を" & Format$(CInt(5 * alevel)) & "低下させる。"
            Else
                msg = "相手の" & Term("気力", u) & "を" & Format$(CInt(-5 * alevel)) & "増加させる。"
            End If
        Case "Ｄ"
            If alevel = DEFAULT_LEVEL Then
                msg = "相手の" & Term("気力", u) & "を10低下させ、その半分を吸収する。"
            ElseIf alevel >= 0 Then
                msg = "相手の" & Term("気力", u) & "を" & Format$(CInt(5 * alevel)) & "低下させ、その半分を吸収する。"
            Else
                msg = "相手の" & Term("気力", u) & "を" & Format$(CInt(-5 * alevel)) & "増加させ、その半分を与える。"
            End If
        Case "低攻"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手の攻撃力を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "低下させる。"
        Case "低防"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手の" & Term("装甲", u) & "を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "低下させる。"
        Case "低運"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手の" & Term("運動性", u) & "を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "低下させる。"
        Case "低移"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手の" & Term("移動力", u) & "を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "低下させる。。"
        Case "先"
            msg = "反撃時でも相手より先に攻撃する。"
        Case "後"
            msg = "反撃時ではない場合も相手より後に攻撃する。"
        Case "吹"
            If alevel > 0 Then
                msg = "相手ユニットを" & StrConv(Format$(CInt(alevel)), vbWide) & _
                    "マス吹き飛ばす。;" & _
                    "クリティカル発生時は吹き飛ばし距離＋１。"
            Else
                msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。"
            End If
        Case "Ｋ"
            If alevel > 0 Then
                msg = "相手ユニットを" & StrConv(Format$(CInt(alevel)), vbWide) & _
                    "マス吹き飛ばす。;" & _
                    "クリティカル発生時は吹き飛ばし距離＋１。" & _
                    Term("サイズ", u) & "制限あり。"
            Else
                msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。" & _
                    Term("サイズ", u) & "制限あり。"
            End If
        Case "引"
            msg = "クリティカル発生時に相手ユニットを隣接するマスまで引き寄せる。"
        Case "転"
            msg = "クリティカル発生時に相手ユニットを" & StrConv(Format$(CInt(alevel)), vbWide) & _
                "マス強制テレポートさせる。テレポート先はランダムに選ばれる。"
        Case "連"
            msg = Format$(alevel) & "回連続して攻撃を行う。;" & _
                "攻撃によって与えるダメージは下記の式で計算される。;" & _
                "  通常のダメージ量 × 命中回数 ／ 攻撃回数"
        Case "再"
            msg = Format$(100 * alevel \ 16) & "%の確率で再攻撃。"
        Case "精"
            msg = "精神に働きかける攻撃。性格が「機械」のユニットには効かない。" & _
                "シールドを無効化。"
        Case "援"
            msg = "自分以外のユニットに対してのみ使用可能。"
        Case "難"
            msg = Format$(10 * alevel) & "%の確率で失敗する。"
        Case "忍"
            msg = "物音を立てずに攻撃し、" & _
                "ステルス状態の際に" & Term("ＣＴ率", u) & "に+10のボーナス。" & _
                "一撃で相手を倒した場合は自分から攻撃をかけてもステルス状態が維持される。"
        Case "盗"
            msg = "クリティカル発生時に敵から持ち物を盗む。;" & _
                "盗めるものは通常は" & Term("資金", u) & "(普通に倒した時の半分の額)だが、" & _
                "相手によってはアイテムを入手することもある。"
        Case "Ｈ"
            msg = "レーダー等でターゲットを追尾する攻撃。;"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際も命中率が低下しないが、"
            End If
            msg = msg & "ＥＣＭによる影響を強く受ける。"
            msg = msg & "攻撃側が撹乱等の状態に陥っても命中率が低下しない。"
        Case "追"
            msg = "自己判断能力を持ち、ターゲットを追尾する攻撃。;"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際も命中率が低下しない。また、"
            End If
            msg = msg & "攻撃側が撹乱等の状態に陥っても命中率が低下しない。"
        Case "有"
            msg = "有線による誘導でターゲットを追尾する攻撃。;"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際も命中率が低下しない。また、"
            End If
            msg = msg & "ＥＣＭによる影響を受けない。"
            msg = msg & "しかし、スペシャルパワーや" & _
                "アイテムの効果によって射程が増加しない。"
        Case "誘"
            msg = "電波妨害を受けない特殊な手段による誘導でターゲットを追尾する攻撃。;"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際も命中率が低下しない。また、"
            End If
            msg = msg & "ＥＣＭによる影響を受けない。"
        Case "爆"
            msg = "爆発によりダメージを与える攻撃。;"
            If IsOptionDefined("距離修正") Then
                msg = msg & "長距離の敵を攻撃する際もダメージが低下しない。"
            End If
        Case "空"
            msg = "空中にいるターゲットを攻撃することを目的とした攻撃。"
            If IsOptionDefined("高度修正") Then
                msg = msg & "地上から空中にいる敵を攻撃する際に命中率が低下しない。"
            End If
        Case "固"
            msg = "パイロットの" & Term("気力", u) & "や攻撃力、防御側の" & _
                Term("装甲", u) & "にかかわらず" & _
                "武器の攻撃力と同じダメージを与える攻撃。" & _
                "ただし、ユニットランクが上がっても攻撃力は増えない。" & _
                Term("スペシャルパワー", u) & "や" & Term("地形適応", u) & _
                "によるダメージ修正は有効。"
        Case "衰"
            msg = "クリティカル発生時に敵の" & Term("ＨＰ", u) & "を現在値の "
            Select Case CInt(alevel)
                Case 1
                    msg = msg & "3/4"
                Case 2
                    msg = msg & "1/2"
                Case 3
                    msg = msg & "1/4"
            End Select
            msg = msg & " まで減少させる。"
        Case "滅"
            msg = "クリティカル発生時に敵の" & Term("ＥＮ", u) & "を現在値の "
            Select Case CInt(alevel)
                Case 1
                    msg = msg & "3/4"
                Case 2
                    msg = msg & "1/2"
                Case 3
                    msg = msg & "1/4"
            End Select
            msg = msg & " まで減少させる。"
        Case "踊"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "踊らせる。"
        Case "狂"
            If alevel = DEFAULT_LEVEL Then
                alevel = 3
            End If
            msg = "クリティカル発生時に相手を"
            If alevel > 0 Then
                msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
            Else
                msg = msg & "その戦闘中のみ"
            End If
            msg = msg & "狂戦士状態にする。"
        Case "ゾ"
            If alevel = DEFAULT_LEVEL Then
                msg = "クリティカル発生時に相手をゾンビ状態にする。"
            Else
                msg = "クリティカル発生時に相手を"
                If alevel > 0 Then
                    msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                Else
                    msg = msg & "その戦闘中のみ"
                End If
                msg = msg & "ゾンビ状態にする。"
            End If
        Case "害"
            If alevel = DEFAULT_LEVEL Then
                msg = "クリティカル発生時に相手の自己回復能力を破壊する。"
            Else
                msg = "クリティカル発生時に相手を"
                If alevel > 0 Then
                    msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                Else
                    msg = msg & "その戦闘中のみ"
                End If
                msg = msg & "自己回復不能状態にする。"
            End If
        Case "習"
            msg = "クリティカル発生時に相手の持つ技を習得出来る。;" & _
                "ただし、習得可能な技を相手が持っていなければ無効。"
        Case "写"
            msg = "クリティカル発生時に相手ユニットに変身する。;" & _
                "ただし、既に変身している場合は使用できない。" & _
                "また、相手と２段階以上" & Term("サイズ", u) & "が異なる場合は無効。"
        Case "化"
            msg = "クリティカル発生時に相手ユニットに変身する。;" & _
                "ただし、既に変身している場合は使用できない。"
        Case "痛"
            msg = "クリティカル発生時に通常の "
            If IsOptionDefined("ダメージ倍率低下") Then
                msg = msg & Format$(100 + 10 * (alevel + 2))
            Else
                msg = msg & Format$(100 + 25 * (alevel + 2))
            End If
            msg = msg & "% のダメージを与える。"
        Case "地", "水", "火", "風", "冷", "雷", "光", "闇", "聖", "死", "木"
            Select Case atype
                Case "水", "火", "風"
                    msg = atype & "を使った"
                Case "光", "闇", "死"
                    msg = atype & "の力を使った"
                Case "地"
                    msg = "大地の力を借りた"
                Case "冷"
                    msg = "冷気による"
                Case "雷"
                    msg = "電撃による"
                Case "聖"
                    msg = "聖なる力を借りた"
                Case "木"
                    msg = "樹木の力を借りた"
            End Select
            msg = msg & whatsthis & "。"
        Case "魔"
            If Not is_ability Then
                msg = "魔力を帯びた攻撃。"
            Else
                msg = "魔法による" & Term("アビリティ", u) & "。"
            End If
        Case "時"
            msg = "時の流れを操る" & whatsthis & "。"
        Case "重"
            msg = "重力を使った攻撃。"
        Case "銃", "剣", "刀", "槍", "斧", "弓"
            msg = atype & "を使った攻撃。"
        Case "機"
            msg = "機械(ロボット、アンドロイド)に対し特に有効な攻撃。"
        Case "感"
            msg = "エスパー(超能力者)に対し特に有効な攻撃。"
        Case "竜"
            msg = "竜族(ドラゴン)に対し特に有効な武器。"
        Case "瀕"
            msg = "瀕死時にのみ使用可能な" & whatsthis & "。"
        Case "禁"
            msg = "現在の状況下では使用することが出来ません。"
        Case "対"
            If Not is_ability Then
                whatsthis = "攻撃"
            End If
            msg = "相手のメインパイロットのレベルが" & StrConv(Format$(CInt(alevel)), vbWide) & _
                "の倍数の場合にのみ有効な" & whatsthis & "。"
        Case "ラ"
            If Not is_ability Then
                whatsthis = "攻撃"
            End If
            msg = "ラーニングが可能な" & whatsthis & "。"
        Case "小"
            msg = "最小射程が" & StrConv(Format$(CInt(alevel)), vbWide) & "になる。"
        Case "散"
            msg = "相手から２マス以上離れていると命中率が上昇し、与えるダメージが減少する。"
        Case Else
            '弱、効、剋属性
            Select Case Left$(atype, 1)
                Case "弱"
                    If alevel = DEFAULT_LEVEL Then
                        alevel = 3
                    End If
                    msg = "クリティカル発生時に相手に" & Mid$(atype, 2) & "属性に対する弱点を"
                    If alevel > 0 Then
                        msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                    Else
                        msg = msg & "その戦闘中のみ"
                    End If
                    msg = msg & "付加する。"
                Case "効"
                    If alevel = DEFAULT_LEVEL Then
                        alevel = 3
                    End If
                    msg = "クリティカル発生時に相手に" & Mid$(atype, 2) & "属性に対する有効を"
                    If alevel > 0 Then
                        msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                    Else
                        msg = msg & "その戦闘中のみ"
                    End If
                    msg = msg & "付加する。"
                Case "剋"
                    If alevel = DEFAULT_LEVEL Then
                        alevel = 3
                    End If
                    msg = "クリティカル発生時に相手の"
                    Select Case Mid$(atype, 2)
                        Case "オ"
                            msg = msg & "オーラ"
                        Case "超"
                            msg = msg & "超能力"
                        Case "シ"
                            msg = msg & "同調率"
                        Case "サ"
                            msg = msg & "超感覚、知覚強化"
                        Case "霊"
                            msg = msg & "霊力"
                        Case "術"
                            msg = msg & "術"
                        Case "技"
                            msg = msg & "技"
                        Case Else
                            msg = msg & Mid$(atype, 2) & "属性の武器、アビリティ"
                    End Select
                    msg = msg & "を"
                    If alevel > 0 Then
                        msg = msg & StrConv(Format$(CInt(alevel)), vbWide) & "ターン"
                    Else
                        msg = msg & "その戦闘中のみ"
                    End If
                    msg = msg & "使用不能にする。"
            End Select
    End Select
    
    fdata = u.FeatureData(atype)
    If ListIndex(fdata, 1) = "解説" Then
        '解説を定義している場合
        msg = ListTail(fdata, 3)
        If Left$(msg, 1) = """" Then
            msg = Mid$(msg, 2, Len(msg) - 2)
        End If
    End If
    
    '等身大基準の際は「パイロット」という語を使わないようにする
    If IsOptionDefined("等身大基準") Then
        ReplaceString msg, "メインパイロット", "ユニット"
        ReplaceString msg, "パイロット", "ユニット"
        ReplaceString msg, "相手のユニット", "相手ユニット"
    End If
    
    AttributeHelpMessage = msg
End Function

