Attribute VB_Name = "Status"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ステータスウィンドウへのステータス表示を行うモジュール
'ppicとupicに分かれているが、ppicにはアイコンと同じ行のデータが書き込まれる。
'決してパイロットステータスを書くのがppicではないことを留意しておく

'ステータス画面に表示されているユニット
Public DisplayedUnit As Unit
Public DisplayedPilotInd As Integer

'ステータス画面の更新を一時停止するかどうか
Public IsStatusWindowDisabled As Boolean
'ADD START 240a
'ステータス画面の背景色
Public StatusWindowBackBolor As Long
'ステータス画面の枠色
Public StatusWindowFrameColor As Long
'ステータス画面の枠幅
Public StatusWindowFrameWidth As Long
'ステータス画面 能力名のフォントカラー
Public StatusFontColorAbilityName As Long
'ステータス画面 有効な能力のフォントカラー
Public StatusFontColorAbilityEnable As Long
'ステータス画面 無効な能力のフォントカラー
Public StatusFontColorAbilityDisable As Long
'ステータス画面 その他通常描画のフォントカラー
Public StatusFontColorNormalString As Long
'ADD  END

'現在の状況をステータスウィンドウに表示
Public Sub DisplayGlobalStatus()
Dim X As Integer, Y As Integer
Dim pic As PictureBox
Dim td As TerrainData
'ADD START 240a
Dim fname As String
Dim wHeight As Integer
Dim ret As Long, color As Long, lineStart As Long, lineEnd As Long
'ADD  END  240a
    
    'ステータスウィンドウを消去
    ClearUnitStatus
    
    Set pic = MainForm.picUnitStatus
    
    pic.FontSize = 12

'ADD START 240a
    'マウスカーソルの位置は？
    X = PixelToMapX(MouseX)
    Y = PixelToMapY(MouseY)
    
    If NewGUIMode Then
        'Global変数が宣言されていれば、ステータス画面用変数の同期を取る
        GlobalVariableLoad
        pic.BackColor = StatusWindowBackBolor
        pic.DrawWidth = StatusWindowFrameWidth
        color = StatusWindowFrameColor
        lineStart = (StatusWindowFrameWidth - 1) / 2
        lineEnd = (StatusWindowFrameWidth + 1) / 2
        pic.FillStyle = vbFSTransparent
        '一旦高さを最大にする
        pic.width = 235
        pic.Height = MapPHeight - 20
        wHeight = GetGlobalStatusSize(X, Y)
        '枠線を引く
        pic.Line (lineStart, lineStart)-(235 - lineEnd, wHeight - lineEnd), color, B
        pic.FillStyle = ObjFillStyle
        '高さを設定する
        pic.Height = wHeight
        pic.CurrentX = 5
        pic.CurrentY = 5
        '文字色をリセット
        pic.ForeColor = StatusFontColorNormalString
    End If
'ADD  END  240a
    pic.Print "ターン数 " & Format$(Turn)
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    pic.Print Term("資金", Nothing, 8) & " " & Format$(Money)
    
'MOV START 240a ↑に移動
'    'マウスカーソルの位置は？
'    X = PixelToMapX(MouseX)
'    Y = PixelToMapY(MouseY)
'MOV  END  240a
    
    'マップ外をクリックした時はここで終了
    If X < 1 Or MapWidth < X _
        Or Y < 1 Or MapHeight < Y _
    Then
        pic.FontSize = 9
        If NewGUIMode Then
            '高さを設定する
            pic.Height = wHeight
        End If
        Exit Sub
    End If
    
    '地形情報の表示
    pic.Print
    
    '地形名称
'ADD START 240a
    'マップ画像表示
    If NewGUIMode Then
        ret = BitBlt(pic.hDC, 5, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
    Else
        ret = BitBlt(pic.hDC, 0, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
    End If
    pic.CurrentX = 37
    pic.CurrentY = 65
'ADD  END  240a
    If InStr(TerrainName(X, Y), "(") > 0 Then
        pic.Print "(" & Format$(X) & "," & Format$(Y) & ") " _
            & Left$(TerrainName(X, Y), InStr(TerrainName(X, Y), "(") - 1)
    Else
        pic.Print "(" & Format$(X) & "," & Format$(Y) & ") " & TerrainName(X, Y)
    End If
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    '命中修正
    If TerrainEffectForHit(X, Y) >= 0 Then
        pic.Print "回避 +" & Format$(TerrainEffectForHit(X, Y)) & "%";
    Else
        pic.Print "回避 " & Format$(TerrainEffectForHit(X, Y)) & "%";
    End If
    
    'ダメージ修正
    If TerrainEffectForDamage(X, Y) >= 0 Then
        pic.Print "  防御 +" & Format$(TerrainEffectForDamage(X, Y)) & "%"
    Else
        pic.Print "  防御 " & Format$(TerrainEffectForDamage(X, Y)) & "%"
    End If
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    'ＨＰ回復率
    If TerrainEffectForHPRecover(X, Y) > 0 Then
        pic.Print Term("ＨＰ") & " +" & Format$(TerrainEffectForHPRecover(X, Y)) & "%  ";
    End If
    
    'ＥＮ回復率
    If TerrainEffectForENRecover(X, Y) > 0 Then
        pic.Print Term("ＥＮ") & " +" & Format$(TerrainEffectForENRecover(X, Y)) & "%";
    End If
    
    If TerrainEffectForHPRecover(X, Y) > 0 _
        Or TerrainEffectForENRecover(X, Y) > 0 _
    Then
        pic.Print
    End If
    
'MOD START 240a
'    Set td = TDList.Item(MapData(X, Y, 0))
    'マスのタイプに応じて参照先を変更
    Select Case MapData(X, Y, MapDataIndex.BoxType)
    Case BoxTypes.Under, BoxTypes.UpperBmpOnly
        Set td = TDList.Item(MapData(X, Y, MapDataIndex.TerrainType))
    Case Else
        Set td = TDList.Item(MapData(X, Y, MapDataIndex.LayerType))
    End Select
'MOD  END
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    'ＨＰ＆ＥＮ減少
    If td.IsFeatureAvailable("ＨＰ減少") Then
        pic.Print Term("ＨＰ") & " -" & Format$(10 * td.FeatureLevel("ＨＰ減少")) & "% (" _
            & td.FeatureData("ＨＰ減少") & ")  ";
    End If
    If td.IsFeatureAvailable("ＥＮ減少") Then
        pic.Print Term("ＥＮ") & " -" & Format$(10 * td.FeatureLevel("ＥＮ減少")) & "% (" _
            & td.FeatureData("ＥＮ減少") & ")  ";
    End If
    If td.IsFeatureAvailable("ＨＰ減少") Or td.IsFeatureAvailable("ＥＮ減少") Then
        pic.Print
    End If
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    'ＨＰ＆ＥＮ増加
    If td.IsFeatureAvailable("ＨＰ増加") Then
        pic.Print Term("ＨＰ") & " +" _
            & Format$(1000 * td.FeatureLevel("ＨＰ増加")) & "  ";
    End If
    If td.IsFeatureAvailable("ＥＮ増加") Then
        pic.Print Term("ＥＮ") & " +" _
            & Format$(10 * td.FeatureLevel("ＥＮ増加")) & "  ";
    End If
    If td.IsFeatureAvailable("ＨＰ増加") Or td.IsFeatureAvailable("ＥＮ増加") Then
        pic.Print
    End If
'MOD  END
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    'ＨＰ＆ＥＮ低下
    If td.IsFeatureAvailable("ＨＰ低下") Then
        pic.Print Term("ＨＰ") & " -" _
            & Format$(1000 * td.FeatureLevel("ＨＰ低下")) & "  ";
    End If
    If td.IsFeatureAvailable("ＥＮ低下") Then
        pic.Print Term("ＥＮ") & " -" _
            & Format$(10 * td.FeatureLevel("ＥＮ低下")) & "  ";
    End If
    If td.IsFeatureAvailable("ＨＰ低下") Or td.IsFeatureAvailable("ＥＮ低下") Then
        pic.Print
    End If
    
'ADD START 240a
    If NewGUIMode Then
        pic.CurrentX = 5
    End If
'ADD  END  240a
    '摩擦
    If td.IsFeatureAvailable("摩擦") Then
        pic.Print "摩擦Lv" _
            & Format$(td.FeatureLevel("摩擦"))
    End If
' ADD START MARGE
    '状態異常付加
    If td.IsFeatureAvailable("状態付加") Then
        pic.Print td.FeatureData("状態付加") & "状態付加"
    End If
' ADD END MARGE
    
    'フォントサイズを元に戻しておく
    pic.FontSize = 9
End Sub

'ユニットステータスを表示
'pindexはステータス表示に使うパイロットを指定
Public Sub DisplayUnitStatus(u As Unit, Optional ByVal pindex As Integer)
Dim p As Pilot
Dim i As Integer, j As Integer, k As Integer, n As Integer, ret As Long
Dim buf As String
Dim fname As String, fdata As String, opt As String
Dim sname As String, stype As String, slevel As String
Dim cx As Integer, cy As Integer
Dim warray() As Integer, wpower() As Long
Dim ppic As PictureBox, upic As PictureBox
Dim ecost As Integer, nmorale As Integer, pmorale As Integer
Dim flist() As String
Dim is_unknown As Boolean
Dim w As Integer, dmg As Long, prob As Integer, cprob As Integer
Dim def_mode As String
Dim name_list() As String
'ADD START 240a
Dim color As Long, lineStart As Long, lineEnd As Long
Dim isNoSp As Boolean
    isNoSp = False
'ADD  END  240a
    'ステータス画面の更新が一時停止されている場合はそのまま終了
    If IsStatusWindowDisabled Then
        Exit Sub
    End If
    
    '破壊、破棄されたユニットは表示しない
    If u.Status = "破壊" Or u.Status = "破棄" Then
        Exit Sub
    End If
    
    Set DisplayedUnit = u
    DisplayedPilotInd = pindex
    
'MOD START MARGE
'    If MainWidth = 15 Then
    If Not NewGUIMode Then
'MOD  END  MARGE
        Set ppic = MainForm.picPilotStatus
        Set upic = MainForm.picUnitStatus
        ppic.Cls
        upic.Cls
    Else
        Set ppic = MainForm.picUnitStatus
        Set upic = MainForm.picUnitStatus
        upic.Cls
'ADD START 240a
        'global変数とステータス描画用の変数を同期
        GlobalVariableLoad
        '新ＧＵＩでは地形表示したときにサイズを変えているので元に戻す
        upic.Move MainPWidth - 240, 10, 235, MainPHeight - 20
        upic.BackColor = StatusWindowBackBolor
        upic.DrawWidth = StatusWindowFrameWidth
        color = StatusWindowFrameColor
        lineStart = (StatusWindowFrameWidth - 1) / 2
        lineEnd = (StatusWindowFrameWidth + 1) / 2
        upic.FillStyle = vbFSTransparent
        '枠線を引く
        upic.Line (lineStart, lineStart)-(235 - lineEnd, MainPHeight - 20 - lineEnd), color, B
        upic.FillStyle = ObjFillStyle
        upic.CurrentX = 5
        upic.CurrentY = 5
        '文字色をリセット
        upic.ForeColor = StatusFontColorNormalString
'ADD  END
    End If
    
    With u
        '情報を更新
        .Update
        
        '未確認ユニットかどうか判定しておく
        If (IsOptionDefined("ユニット情報隠蔽") _
                And (Not .IsConditionSatisfied("識別済み") _
                    And (.Party0 = "敵" Or .Party0 = "中立"))) _
            Or .IsConditionSatisfied("ユニット情報隠蔽") _
        Then
            is_unknown = True
        End If
        
        'パイロットが乗っていない？
        If .CountPilot = 0 Then
            'キャラ画面をクリア
            If MainWidth = 15 Then
                MainForm.picFace = LoadPicture("")
            Else
                DrawPicture "white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "ステータス"
            End If
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 150)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            ppic.Print Term("レベル", u)
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            ppic.Print Term("気力", u)
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 0)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            upic.Print Term("格闘", u, 4) & "               " & Term("射撃", u)
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            upic.Print Term("命中", u, 4) & "               " & Term("回避", u)
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            upic.Print Term("技量", u, 4) & "               " & Term("反応", u)
            upic.Print
            upic.Print
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            
            GoTo UnitStatus
         End If
         
        '表示するパイロットを選択
        If pindex = 0 Then
            'メインパイロット
            Set p = .MainPilot
            If .MainPilot.Nickname = .Pilot(1).Nickname Or .Data.PilotNum = 1 Then
                DisplayedPilotInd = 1
            End If
        ElseIf pindex = 1 Then
            'メインパイロットまたは１番目のパイロット
            If .MainPilot.Nickname <> .Pilot(1).Nickname And .Data.PilotNum <> 1 Then
                Set p = .Pilot(1)
            Else
                Set p = .MainPilot
            End If
        ElseIf pindex <= .CountPilot Then
            'サブパイロット
            Set p = .Pilot(pindex)
        ElseIf pindex <= .CountPilot + .CountSupport Then
            'サポートパイロット
            Set p = .Support(pindex - .CountPilot)
        Else
            '追加サポート
            Set p = .AdditionalSupport
        End If
        
        With p
            '情報を更新
            .UpdateSupportMod
            
            'パイロット画像を表示
            fname = "\Bitmap\Pilot\" & .Bitmap
            If frmMultiSelectListBox.Visible Then
                'ザコ＆汎用パイロットが乗るユニットの出撃選択時はパイロット画像の
                '代わりにユニット画像を表示
                If InStr(.Name, "(ザコ)") > 0 Or InStr(.Name, "(汎用)") > 0 Then
                    fname = "\Bitmap\Unit\" & u.Bitmap
                End If
            End If
            
            '画像ファイルを検索
            If InStr(fname, "\-.bmp") > 0 Then
                fname = ""
            ElseIf FileExists(ScenarioPath & fname) Then
                fname = ScenarioPath & fname
            ElseIf FileExists(ExtDataPath & fname) Then
                fname = ExtDataPath & fname
            ElseIf FileExists(ExtDataPath2 & fname) Then
                fname = ExtDataPath2 & fname
            ElseIf FileExists(AppPath & fname) Then
                fname = AppPath & fname
            Else
                '画像が見つからなかったことを記録
                If InStr(fname, "\Pilot\") > 0 Then
                    If .Bitmap = .Data.Bitmap Then
                        .Data.IsBitmapMissing = True
                    End If
                End If
                fname = ""
            End If
            
            '画像ファイルを読み込んで表示
            If MainWidth = 15 Then
                If fname <> "" Then
                    On Error GoTo ErrorHandler
                    MainForm.picTmp = LoadPicture(fname)
                    On Error GoTo 0
                    MainForm.picFace.PaintPicture _
                        MainForm.picTmp.Picture, _
                        0, 0, 64, 64
                Else
                    '画像ファイルが見つからなかった場合はキャラ画面をクリア
                    MainForm.picFace = LoadPicture("")
                End If
            Else
                If fname <> "" Then
                    DrawPicture fname, 2, 2, 64, 64, 0, 0, 0, 0, "ステータス"
                Else
                    '画像ファイルが見つからなかった場合はキャラ画面をクリア
                    DrawPicture "white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "ステータス"
                End If
            End If
            
            'パイロット愛称
            ppic.FontSize = 10.5
            ppic.FontBold = False
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END
                ppic.CurrentX = 68
            End If
            ppic.Print .Nickname
            ppic.FontBold = False
            ppic.FontSize = 10
            
            'ダミーパイロット？
            If .Nickname0 = "パイロット不在" Then
'MOD START 240a
'                ppic.ForeColor = rgb(0, 0, 150)
                ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
'MOD START 240a
    '            If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                ppic.Print Term("レベル", u)
'MOD START 240a
    '            If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                ppic.Print Term("気力", u)
'MOD START 240a
'                ppic.ForeColor = rgb(0, 0, 0)
                ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
'MOD START 240a
    '            If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                upic.Print Term("格闘", u, 4) & "               " & Term("射撃", u)
'MOD START 240a
    '            If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                upic.Print Term("命中", u, 4) & "               " & Term("回避", u)
'MOD START 240a
    '            If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                upic.Print Term("技量", u, 4) & "               " & Term("反応", u)
                upic.Print
                upic.Print
'MOD START 240a
'               upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                
                GoTo UnitStatus
            End If
            'レベル、経験値、行動回数
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 150)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
'MOD START 240a
'            If MainWidth <> 15 Then
            If NewGUIMode Then
'MOD  END  240a
                ppic.CurrentX = 68
            End If
            ppic.Print Term("レベル", u) & " ";
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 0)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If .Party = "味方" Then
                ppic.Print Format$(.Level) & " (" & .Exp & ")";
                Select Case u.Action
                    Case 2
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 200)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        ppic.Print " Ｗ";
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 0)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    Case 3
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 200)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        ppic.Print " Ｔ";
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 0)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                End Select
            Else
                If Not is_unknown Then
                    ppic.Print Format$(.Level);
                    If u.Action = 2 Then
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 200)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        ppic.Print " Ｗ";
'MOD START 240a
'                        ppic.ForeColor = rgb(0, 0, 0)
                        ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    End If
                Else
                    ppic.Print "？";
                End If
            End If
            ppic.Print
            
            '気力
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 150)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            If MainWidth <> 15 Then
                ppic.CurrentX = 68
            End If
            ppic.Print Term("気力", u) & " ";
'MOD START 240a
'            ppic.ForeColor = rgb(0, 0, 0)
            ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If Not is_unknown Then
                If .MoraleMod > 0 Then
                    ppic.Print Format$(.Morale) & _
                        "+" & Format$(.MoraleMod) & " (" & .Personality & ")"
                Else
                    ppic.Print Format$(.Morale) & " (" & .Personality & ")"
                End If
            Else
                ppic.Print "？"
            End If
            
            'ＳＰ
            If .MaxSP > 0 Then
'MOD START 240a
'                ppic.ForeColor = rgb(0, 0, 150)
                ppic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                If MainWidth <> 15 Then
                    ppic.CurrentX = 68
                End If
                ppic.Print Term("ＳＰ", u) & " ";
'MOD START 240a
'                ppic.ForeColor = rgb(0, 0, 0)
                ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                If Not is_unknown Then
                    ppic.Print Format$(.SP) & "/" & Format$(.MaxSP)
                Else
                    ppic.Print "？"
                End If
            Else
                isNoSp = True
            End If
            
            '使用中のスペシャルパワー一覧
            If Not is_unknown Then
'MOD START 240a
'                ppic.ForeColor = rgb(0, 0, 0)
                ppic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
'MOD START 240a
'                If MainWidth <> 15 Then
                If NewGUIMode Then
'MOD  END
                    ppic.CurrentX = 68
                End If
                ppic.Print u.SpecialPowerInEffect
'ADD START 240a
            Else
                If NewGUIMode Then
                    ppic.Print " "
                End If
'ADD  END  240a
            End If
'ADD START 240a
            If isNoSp Then
                ppic.Print " "
            End If
            
            'upicを明示的に初期化
            upic.FontBold = False
            upic.FontSize = 9

'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '格闘
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("格闘", u, 4) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4) & Space$(10);
            ElseIf .Data.Infight > 1 Then
                Select Case .InfightMod + .InfightMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.InfightBase), 5) _
                            & RightPaddedString("+" & Format$(.InfightMod + .InfightMod2), 9);
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.InfightBase), 5) _
                            & RightPaddedString(Format$(.InfightMod + .InfightMod2), 9);
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Infight), 5) _
                            & Space$(9);
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(9);
            End If
            
            '射撃
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            If Not .HasMana() Then
                upic.Print Term("射撃", u, 4) & " ";
            Else
                upic.Print Term("魔力", u, 4) & " ";
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4)
            ElseIf .Data.Shooting > 1 Then
                Select Case .ShootingMod + .ShootingMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.ShootingBase), 5) _
                            & RightPaddedString("+" & Format$(.ShootingMod + .ShootingMod2), 5)
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.ShootingBase), 5) _
                            & RightPaddedString(Format$(.ShootingMod + .ShootingMod2), 5)
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Shooting), 5) _
                            & Space$(5)
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(5)
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '命中
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("命中", u, 4) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4) & Space$(10);
            ElseIf .Data.Hit > 1 Then
                Select Case .HitMod + .HitMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.HitBase), 5) _
                            & RightPaddedString("+" & Format$(.HitMod + .HitMod2), 9);
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.HitBase), 5) _
                            & RightPaddedString(Format$(.HitMod + .HitMod2), 9);
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Hit), 5) _
                            & Space$(9);
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(9);
            End If
            
            '回避
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("回避", u, 4) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4)
            ElseIf .Data.Dodge > 1 Then
                Select Case .DodgeMod + .DodgeMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.DodgeBase), 5) _
                            & RightPaddedString("+" & Format$(.DodgeMod + .DodgeMod2), 9)
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.DodgeBase), 5) _
                            & RightPaddedString(Format$(.DodgeMod + .DodgeMod2), 9)
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Dodge), 5) _
                            & Space$(9)
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(9)
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '技量
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("技量", u, 4) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4) & Space$(10);
            ElseIf .Data.Technique > 1 Then
                Select Case .TechniqueMod + .TechniqueMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.TechniqueBase), 5) _
                            & RightPaddedString("+" & Format$(.TechniqueMod + .TechniqueMod2), 9);
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.TechniqueBase), 5) _
                            & RightPaddedString(Format$(.TechniqueMod + .TechniqueMod2), 9);
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Technique), 5) _
                            & Space$(9);
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(9);
            End If
            
            '反応
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("反応", u, 4) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            If is_unknown Then
                upic.Print LeftPaddedString("？", 4)
            ElseIf .Data.Intuition > 1 Then
                Select Case .IntuitionMod + .IntuitionMod2
                    Case Is > 0
                        upic.Print LeftPaddedString(Format$(.IntuitionBase), 5) _
                            & RightPaddedString("+" & Format$(.IntuitionMod + .IntuitionMod2), 9)
                    Case Is < 0
                        upic.Print LeftPaddedString(Format$(.IntuitionBase), 5) _
                            & RightPaddedString(Format$(.IntuitionMod + .IntuitionMod2), 9)
                    Case 0
                        upic.Print LeftPaddedString(Format$(.Intuition), 5) _
                            & Space$(9)
                End Select
            Else
                upic.Print LeftPaddedString("--", 5) & Space$(9)
            End If
            
            If IsOptionDefined("防御力成長") _
                Or IsOptionDefined("防御力レベルアップ") _
            Then
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
                '防御
'MOD START 240a
'               upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print Term("防御", u) & " ";
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                If is_unknown Then
                    upic.Print LeftPaddedString("？", 4)
                ElseIf Not .IsSupport(u) Then
                    upic.Print LeftPaddedString(Format$(.Defense), 5)
                Else
                    upic.Print LeftPaddedString("--", 5)
                End If
            End If
        End With
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '所有するスペシャルパワー一覧
        With p
            If .CountSpecialPower > 0 Then
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print Term("スペシャルパワー", u, 18) & " ";
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                If Not is_unknown Then
                    For i = 1 To .CountSpecialPower
                        If .SP < .SpecialPowerCost(.SpecialPower(i)) Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        
                        End If
                        upic.Print SPDList.Item(.SpecialPower(i)).ShortName;
'MOD START 240a
'                        upic.ForeColor = rgb(0, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    Next
                Else
                    upic.Print "？";
                End If
                upic.Print
            End If
        End With
        
        '未識別のユニットはこれ以降の情報を表示しない
        If is_unknown Then
            upic.CurrentY = upic.CurrentY + 8
            GoTo UnitStatus
        End If
        
        'パイロット用特殊能力一覧
        With p
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '霊力
            If .MaxPlana > 0 Then
                If .IsSkillAvailable("霊力") Then
                    sname = .SkillName("霊力")
                Else
                    '追加パイロットは第１パイロットの霊力を代わりに使うので
                    sname = u.Pilot(1).SkillName("霊力")
                End If
                If InStr(sname, "非表示") = 0 Then
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 150)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                    upic.Print sname & " ";
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    If u.PlanaLevel() < .Plana Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                    upic.Print Format$(.Plana) & "/" & Format$(.MaxPlana)
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                End If
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '同調率
            If .SynchroRate() > 0 Then
                If InStr(.SkillName("同調率"), "非表示") = 0 Then
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 150)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                    upic.Print .SkillName("同調率") & " ";
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    If u.SyncLevel() < .SynchroRate Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                    upic.Print Format$(.SynchroRate) & "%"
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                End If
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '得意技＆不得手
            n = 0
            If .IsSkillAvailable("得意技") Then
                n = n + 1
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print "得意技 ";
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                upic.Print RightPaddedString(.SkillData("得意技"), 12);
            End If
            If .IsSkillAvailable("不得手") Then
                n = n + 1
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print "不得手 ";
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                upic.Print .SkillData("不得手");
            End If
            If n > 0 Then
                upic.Print
            End If
            
            '表示するパイロット能力のリストを作成
            ReDim name_list(.CountSkill)
            For i = 1 To .CountSkill
                name_list(i) = .Skill(i)
            Next
            '付加されたパイロット特殊能力
            For i = 1 To u.CountCondition
                If u.ConditionLifetime(i) <> 0 Then
                    Select Case Right$(u.Condition(i), 3)
                        Case "付加２", "強化２"
                            Select Case LIndex(u.ConditionData(i), 1)
                                Case "非表示", "解説"
                                    '非表示の能力
                                Case Else
                                    stype = Left$(u.Condition(i), Len(u.Condition(i)) - 3)
                                    Select Case stype
                                        Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
                                            '重複可能な能力
                                            ReDim Preserve name_list(UBound(name_list) + 1)
                                            name_list(UBound(name_list)) = stype
                                        Case Else
                                            '既に所有している能力であればスキップ
                                            For j = 1 To UBound(name_list)
                                                If stype = name_list(j) Then
                                                    Exit For
                                                End If
                                            Next
                                            If j > UBound(name_list) Then
                                                ReDim Preserve name_list(UBound(name_list) + 1)
                                                name_list(UBound(name_list)) = stype
                                            End If
                                    End Select
                            End Select
                    End Select
                End If
            Next
            
            'パイロット能力を表示
            n = 0
            For i = 1 To UBound(name_list)
'ADD START 240a
                '文字色をリセット
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'ADD  END  240a
                stype = name_list(i)
                If i <= .CountSkill Then
                    sname = .SkillName(i)
                    slevel = .SkillLevel(i)
                Else
                    sname = .SkillName(stype)
                    slevel = .SkillLevel(stype)
                End If
                
                If InStr(sname, "非表示") > 0 Then
                    GoTo NextSkill
                End If
                
                Select Case stype
                    Case "オーラ"
                        If DisplayedPilotInd = 1 Then
                            If u.AuraLevel(True) < u.AuraLevel() _
                                And MapFileName <> "" _
                            Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            If u.AuraLevel(True) > slevel Then
                                sname = sname & "+" & Format$(u.AuraLevel(True) - slevel)
                            End If
                        End If
                        
                    Case "超能力"
                        If DisplayedPilotInd = 1 Then
                            If u.PsychicLevel(True) < u.PsychicLevel() _
                                And MapFileName <> "" _
                            Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            If u.PsychicLevel(True) > slevel Then
                                sname = sname & "+" & Format$(u.PsychicLevel(True) - slevel)
                            End If
                        End If
                        
                    Case "底力", "超底力", "覚悟"
                        If u.HP <= u.MaxHP \ 4 Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        End If
                        
                    Case "不屈"
                        If u.HP <= u.MaxHP \ 2 Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        End If
                        
                    Case "潜在力開放"
                        If .Morale >= 130 Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        End If
                        
                    Case "スペシャルパワー自動発動"
                        If i <= .CountSkill Then
                            If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                            End If
                        Else
                            If .Morale >= StrToLng(LIndex(.SkillData(stype), 3)) Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                            End If
                        End If
                        
                    Case "Ｓ防御"
                        If Not u.IsFeatureAvailable("シールド") _
                            And Not u.IsFeatureAvailable("大型シールド") _
                            And Not u.IsFeatureAvailable("小型シールド") _
                            And Not u.IsFeatureAvailable("エネルギーシールド") _
                            And Not u.IsFeatureAvailable("アクティブシールド") _
                            And Not u.IsFeatureAvailable("盾") _
                            And Not u.IsFeatureAvailable("バリアシールド") _
                            And Not u.IsFeatureAvailable("アクティブフィールド") _
                            And Not u.IsFeatureAvailable("アクティブプロテクション") _
                            And InStr(u.FeatureData("阻止"), "Ｓ防御") = 0 _
                            And InStr(u.FeatureData("広域阻止"), "Ｓ防御") = 0 _
                            And InStr(u.FeatureData("反射"), "Ｓ防御") = 0 _
                            And InStr(u.FeatureData("当て身技"), "Ｓ防御") = 0 _
                            And InStr(u.FeatureData("自動反撃"), "Ｓ防御") = 0 _
                            And MapFileName <> "" _
                        Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                        
                    Case "切り払い"
                        For j = 1 To u.CountWeapon
                            If u.IsWeaponClassifiedAs(j, "武") Then
                                If Not u.IsDisabled(u.Weapon(j).Name) Then
                                    Exit For
                                End If
                            End If
                        Next
                        If u.IsFeatureAvailable("格闘武器") Then
                            j = 0
                        End If
                        If j > u.CountWeapon _
                            And InStr(u.FeatureData("阻止"), "切り払い") = 0 _
                            And InStr(u.FeatureData("広域阻止"), "切り払い") = 0 _
                            And InStr(u.FeatureData("反射"), "切り払い") = 0 _
                            And InStr(u.FeatureData("当て身技"), "切り払い") = 0 _
                            And InStr(u.FeatureData("自動反撃"), "切り払い") = 0 _
                            And MapFileName <> "" _
                        Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                        
                    Case "迎撃"
                        For j = 1 To u.CountWeapon
                            If u.IsWeaponAvailable(j, "移動後") _
                                And u.IsWeaponClassifiedAs(j, "射撃系") _
                                And (u.Weapon(j).Bullet >= 10 _
                                Or (u.Weapon(j).Bullet = 0 _
                                    And u.Weapon(j).ENConsumption <= 5)) _
                            Then
                                Exit For
                            End If
                        Next
                        If u.IsFeatureAvailable("迎撃武器") Then
                            j = 0
                        End If
                        If j > u.CountWeapon _
                            And InStr(u.FeatureData("阻止"), "迎撃") = 0 _
                            And InStr(u.FeatureData("広域阻止"), "迎撃") = 0 _
                            And InStr(u.FeatureData("反射"), "迎撃") = 0 _
                            And InStr(u.FeatureData("当て身技"), "迎撃") = 0 _
                            And InStr(u.FeatureData("自動反撃"), "迎撃") = 0 _
                            And MapFileName <> "" _
                        Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                        
                    Case "浄化"
                        For j = 1 To u.CountWeapon
                            If u.IsWeaponClassifiedAs(j, "浄") Then
                                If Not u.IsDisabled(u.Weapon(j).Name) Then
                                    Exit For
                                End If
                            End If
                        Next
                        If j > u.CountWeapon _
                            And MapFileName <> "" _
                        Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                        
                    Case "援護"
                        If MapFileName <> "" Then
                            If u.Party = Stage Then
                                ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
                            Else
                                If u.IsUnderSpecialPowerEffect("サポートガード不能") Then
'MOD START 240a
'                                    upic.ForeColor = rgb(150, 0, 0)
                                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                                End If
                                ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
                            End If
                            If ret = 0 Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            sname = sname & " (残り" & Format$(ret) & "回)"
                        End If
                        
                    Case "援護攻撃"
                        If MapFileName <> "" Then
                            ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
                            If ret = 0 Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            sname = sname & " (残り" & Format$(ret) & "回)"
                        End If
                        
                    Case "援護防御"
                        If MapFileName <> "" Then
                            ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
                            If ret = 0 Or u.IsUnderSpecialPowerEffect("サポートガード不能") Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            sname = sname & " (残り" & Format$(ret) & "回)"
                        End If
                        
                    Case "統率"
                        If MapFileName <> "" Then
                            ret = MaxLng(u.MaxSyncAttack - u.UsedSyncAttack, 0)
                            If ret = 0 Then
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                            End If
                            sname = sname & " (残り" & Format$(ret) & "回)"
                        End If
                        
                    Case "カウンター"
                        If MapFileName <> "" Then
                            ret = MaxLng(u.MaxCounterAttack - u.UsedCounterAttack, 0)
                            If ret > 100 Then
                                sname = sname & " (残り∞回)"
                            ElseIf ret > 0 Then
                                sname = sname & " (残り" & Format$(ret) & "回)"
                            Else
'MOD START 240a
'                                upic.ForeColor = rgb(150, 0, 0)
                                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                                sname = sname & " (残り0回)"
                            End If
                        End If
                        
                    Case "先手必勝"
                        If u.MaxCounterAttack > 100 Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        End If
                        
                    Case "耐久"
                        If IsOptionDefined("防御力成長") _
                            Or IsOptionDefined("防御力レベルアップ") _
                        Then
                            GoTo NextSkill
                        End If
                        
                    Case "霊力", "同調率", "得意技", "不得手"
                        GoTo NextSkill
                        
                End Select
                
                '特殊能力名を表示
                If LenB(StrConv(sname, vbFromUnicode)) > 19 Then
                    If n > 0 Then
                        upic.Print
'ADD START 240a
                        If NewGUIMode Then
                            upic.CurrentX = 5
                        End If
'ADD  END  240a
                    End If
                    upic.Print sname;
                    n = 2
                Else
                    upic.Print RightPaddedString(sname, 19);
                    n = n + 1
                End If
                upic.ForeColor = vbBlack
                
                '必要に応じて改行
                If n > 1 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                    n = 0
                End If
NextSkill:
            Next
        End With
        
        If n > 0 Then
            upic.Print
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
        End If
        
        upic.CurrentY = upic.CurrentY + 8
        
UnitStatus:
        
        'パイロットステータス表示用のダミーユニットの場合はここで表示を終了
        If .IsFeatureAvailable("ダミーユニット") Then
            GoTo UpdateStatusWindow
        End If
        
        'ここからはユニットに関する情報
        
        'ユニット愛称
        upic.FontSize = 10.5
        upic.FontBold = False
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
            '文字色をリセット
            upic.ForeColor = StatusFontColorNormalString
        End If
'ADD  END  240a
        upic.Print .Nickname0
        upic.FontBold = False
        upic.FontSize = 9
        
        If .Status = "出撃" And MapFileName <> "" Then
            Dim td As TerrainData
            
            '地形情報の表示
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            'ユニットの位置を地形名称
            If InStr(TerrainName(.X, .Y), "(") > 0 Then
                upic.Print .Area & " (" _
                    & Left$(TerrainName(.X, .Y), InStr(TerrainName(.X, .Y), "(") - 1);
            Else
                upic.Print .Area & " (" & TerrainName(.X, .Y);
            End If
            
            '回避＆防御修正
            If TerrainEffectForHit(.X, .Y) = TerrainEffectForDamage(.X, .Y) Then
                If TerrainEffectForHit(.X, .Y) >= 0 Then
                    upic.Print " 回＆防+" _
                        & Format$(TerrainEffectForHit(.X, .Y)) & "%";
                Else
                    upic.Print " 回＆防" _
                        & Format$(TerrainEffectForHit(.X, .Y)) & "%";
                End If
            Else
                If TerrainEffectForHit(.X, .Y) >= 0 Then
                    upic.Print " 回+" _
                        & Format$(TerrainEffectForHit(.X, .Y)) & "%";
                Else
                    upic.Print " 回" _
                        & Format$(TerrainEffectForHit(.X, .Y)) & "%";
                End If
                If TerrainEffectForDamage(.X, .Y) >= 0 Then
                    upic.Print " 防+" _
                        & Format$(TerrainEffectForDamage(.X, .Y)) & "%";
                Else
                    upic.Print " 防" _
                        & Format$(TerrainEffectForDamage(.X, .Y)) & "%";
                End If
            End If
            
            'ＨＰ＆ＥＮ回復
            If TerrainEffectForHPRecover(.X, .Y) > 0 Then
                upic.Print " " & Left$(Term("ＨＰ"), 1) & "+" _
                    & Format$(TerrainEffectForHPRecover(.X, .Y)) & "%";
            End If
            If TerrainEffectForENRecover(.X, .Y) > 0 Then
                upic.Print " " & Left$(Term("ＥＮ"), 1) & "+" _
                    & Format$(TerrainEffectForENRecover(.X, .Y)) & "%";
            End If
            
'MOD START 240a
'            Set td = TDList.Item(MapData(.X, .Y, 0))
            'マスのタイプに応じて参照先を変更
            Select Case MapData(.X, .Y, MapDataIndex.BoxType)
            Case BoxTypes.Under, BoxTypes.UpperBmpOnly
                Set td = TDList.Item(MapData(.X, .Y, MapDataIndex.TerrainType))
            Case Else
                Set td = TDList.Item(MapData(.X, .Y, MapDataIndex.LayerType))
            End Select
'MOD START 240a
            'ＨＰ＆ＥＮ減少
            If td.IsFeatureAvailable("ＨＰ減少") Then
                upic.Print " " & Left$(Term("ＨＰ"), 1) & "-" _
                    & Format$(10 * td.FeatureLevel("ＨＰ減少")) & "%";
            End If
            If td.IsFeatureAvailable("ＥＮ減少") Then
                upic.Print " " & Left$(Term("ＥＮ"), 1) & "-" _
                    & Format$(10 * td.FeatureLevel("ＥＮ減少")) & "%";
            End If
            
            'ＨＰ＆ＥＮ増加
            If td.IsFeatureAvailable("ＨＰ増加") Then
                upic.Print " " & Left$(Term("ＨＰ"), 1) & "+" _
                    & Format$(1000 * td.FeatureLevel("ＨＰ増加"));
            End If
            If td.IsFeatureAvailable("ＥＮ増加") Then
                upic.Print " " & Left$(Term("ＥＮ"), 1) & "+" _
                    & Format$(10 * td.FeatureLevel("ＥＮ増加"));
            End If
            
            'ＨＰ＆ＥＮ低下
            If td.IsFeatureAvailable("ＨＰ低下") Then
                upic.Print " " & Left$(Term("ＨＰ"), 1) & "-" _
                    & Format$(1000 * td.FeatureLevel("ＨＰ低下"));
            End If
            If td.IsFeatureAvailable("ＥＮ低下") Then
                upic.Print " " & Left$(Term("ＥＮ"), 1) & "-" _
                    & Format$(10 * td.FeatureLevel("ＥＮ低下"));
            End If
            
            '摩擦
            If td.IsFeatureAvailable("摩擦") Then
                upic.Print " 摩L" _
                    & Format$(td.FeatureLevel("摩擦"));
            End If
            
            upic.Print ")"
        Else
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "ランク ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print Format$(.Rank)
        End If
        
        '未確認ユニット？
        If is_unknown Then
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            'ＨＰ
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("ＨＰ", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print "?????/?????"
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            'ＥＮ
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("ＥＮ", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print "???/???"
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '装甲
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("装甲", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString("？", 12);
            
            '運動性
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("運動性", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print "？"
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '移動タイプ
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("タイプ", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString("？", 12);
            
            '移動力
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("移動力", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print "？"
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '地形適応
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "適応   ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString("？", 12);
            
            'ユニットサイズ
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print Term("サイズ", Nothing, 6) & " ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print "？"
            
            'サポートアタックを得られるかどうかのみ表示
            If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択") _
                And (SelectedCommand = "攻撃" Or SelectedCommand = "マップ攻撃") _
                And Not SelectedUnit Is Nothing _
            Then
                If .Party = "敵" _
                    Or .Party = "中立" _
                    Or .IsConditionSatisfied("暴走") _
                    Or .IsConditionSatisfied("魅了") _
                    Or .IsConditionSatisfied("憑依") _
                Then
                    upic.Print
                    
                    '攻撃手段
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 150)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                    upic.Print "攻撃     ";
'MOD START 240a
'                   upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                    upic.Print SelectedUnit.WeaponNickname(SelectedWeapon);
                    'サポートアタックを得られる？
                    If Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "合") _
                        And Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") _
                    Then
                        If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
                            upic.Print " [援]"
                        End If
                    End If
                End If
            End If
            
            GoTo UpdateStatusWindow
        End If
        
        '実行中の命令
        If .Party = "ＮＰＣ" _
            And Not .IsConditionSatisfied("混乱") _
            And Not .IsConditionSatisfied("恐怖") _
            And Not .IsConditionSatisfied("暴走") _
            And Not .IsConditionSatisfied("狂戦士") _
        Then
            '思考モードを見れば実行している命令が分かるので……
            buf = ""
            If .IsConditionSatisfied("魅了") Then
                If Not .Master Is Nothing Then
                    If .Master.Party = "味方" Then
                        buf = .Mode
                    End If
                End If
            End If
            If .IsFeatureAvailable("召喚ユニット") _
                And Not .IsConditionSatisfied("魅了") _
            Then
                If Not .Summoner Is Nothing Then
                    If .Summoner.Party = "味方" Then
                        buf = .Mode
                    End If
                End If
            End If
            
            If buf = "通常" Then
                upic.Print "自由行動中"
            ElseIf PList.IsDefined(buf) Then
                '思考モードにパイロット名が指定されている場合
                With PList.Item(buf)
                    If Not .Unit Is Nothing Then
                        With .Unit
                            If .Status = "出撃" Then
                                upic.Print .Nickname & "(" & Format$(.X) & "," & Format(.Y) & ")を";
                                If .Party = "味方" Then
                                    upic.Print "護衛中"
                                Else
                                    upic.Print "追跡中"
                                End If
                            End If
                        End With
                    End If
                End With
            ElseIf LLength(buf) = 2 Then
                '思考モードに座標が指定されている場合
                upic.Print "(" & LIndex(buf, 1) & "," & LIndex(buf, 2) & ")に移動中"
            End If
        End If
        
        'ユニットにかかっている特殊ステータス
        ReDim name_list(0)
        For i = 1 To .CountCondition
            '時間切れ？
            If .ConditionLifetime(i) = 0 Then
                GoTo NextCondition
            End If
            
            '非表示？
            If InStr(.ConditionData(i), "非表示") > 0 Then
                GoTo NextCondition
            End If
            
            '解説？
            If LIndex(.ConditionData(i), 1) = "解説" Then
                GoTo NextCondition
            End If
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            Select Case .Condition(i)
                Case "データ不明", "形態固定", "機体固定", "不死身", "無敵", _
                    "識別済み", "非操作", "破壊キャンセル", "盾ダメージ", _
                    "能力コピー", _
                    "メッセージ付加", "ノーマルモード付加", "追加パイロット付加", _
                    "追加サポート付加", "パイロット愛称付加", "パイロット画像付加", _
                    "性格変更付加", "性別付加", "ＢＧＭ付加", "愛称変更付加", _
                    "スペシャルパワー無効化", "精神コマンド無効化", _
                    "ユニット画像付加", "メッセージ付加"
                    '非表示
                Case "残り時間"
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print "残り時間" & Format$(.ConditionLifetime(i)) & "ターン"
                    End If
                Case "無効化付加", "耐性付加", "吸収付加", "弱点付加"
                    upic.Print .ConditionData(i) & .Condition(i);
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " " & Format$(.ConditionLifetime(i)) & "T";
                    End If
                    upic.Print ""
                Case "特殊効果無効化付加"
                    upic.Print .ConditionData(i) & "無効化付加";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "攻撃属性付加"
                    upic.Print LIndex(.ConditionData(i), 1) & "属性付加";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "武器強化付加"
                    upic.Print "武器強化Lv" & .ConditionLevel(i) & "付加";
                    If .ConditionData(i) <> "" Then
                        upic.Print "(" & .ConditionData(i) & ")";
                    End If
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "命中率強化付加"
                    upic.Print "命中率強化Lv" & .ConditionLevel(i) & "付加";
                    If .ConditionData(i) <> "" Then
                        upic.Print "(" & .ConditionData(i) & ")";
                    End If
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "ＣＴ率強化付加"
                    upic.Print "ＣＴ率強化Lv" & .ConditionLevel(i) & "付加";
                    If .ConditionData(i) <> "" Then
                        upic.Print "(" & .ConditionData(i) & ")";
                    End If
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "特殊効果発動率強化付加"
                    upic.Print "特殊効果発動率強化Lv" & .ConditionLevel(i) & "付加";
                    If .ConditionData(i) <> "" Then
                        upic.Print "(" & .ConditionData(i) & ")";
                    End If
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "地形適応変更付加"
                    upic.Print "地形適応変更付加";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "盾付加"
                    upic.Print LIndex(.ConditionData(i), 1) & "付加";
                    upic.Print "(" & Format$(.ConditionLevel(i)) & ")";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "ダミー破壊"
                    buf = .FeatureName("ダミー")
                    If InStr(buf, "Lv") > 0 Then
                        buf = Left$(buf, InStr(buf, "Lv") - 1)
                    End If
                    upic.Print buf & StrConv(Format$(.ConditionLevel(i)), vbWide) & "体破壊"
                Case "ダミー付加"
                    upic.Print .FeatureName("ダミー") & "残り" & _
                        StrConv(Format$(.ConditionLevel(i)), vbWide) & "体";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "バリア発動"
                    If .ConditionData(i) <> "" Then
                        upic.Print .ConditionData(i);
                    Else
                        upic.Print "バリア発動";
                    End If
                    upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン"
                Case "フィールド発動"
                    If .ConditionData(i) <> "" Then
                        upic.Print .ConditionData(i);
                    Else
                        upic.Print "フィールド発動";
                    End If
                    upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン"
                Case "装甲劣化"
                    upic.Print Term("装甲", u) & "劣化";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "運動性ＵＰ"
                    upic.Print Term("運動性", u) & "ＵＰ";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "運動性ＤＯＷＮ"
                    upic.Print Term("運動性", u) & "ＤＯＷＮ";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "移動力ＵＰ"
                    upic.Print Term("移動力", u) & "ＵＰ";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case "移動力ＤＯＷＮ"
                    upic.Print Term("移動力", u) & "ＤＯＷＮ";
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
                Case Else
                    '充填中？
                    If Right$(.Condition(i), 3) = "充填中" Then
                        If .IsHero() Then
                            upic.Print Left$(.Condition(i), Len(.Condition(i)) - 3);
                            upic.Print "準備中";
                        Else
                            upic.Print .Condition(i);
                        End If
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン"
                        GoTo NextCondition
                    End If
                    
                    'パイロット特殊能力付加＆強化による状態は表示しない
                    If Right$(.Condition(i), 3) = "付加２" _
                        Or Right$(.Condition(i), 3) = "強化２" _
                    Then
                        GoTo NextCondition
                    End If
                    
                    If Right$(.Condition(i), 2) = "付加" _
                        And .ConditionData(i) <> "" _
                    Then
                        buf = LIndex(.ConditionData(i), 1) & "付加"
                    ElseIf Right$(.Condition(i), 2) = "強化" _
                        And .ConditionData(i) <> "" _
                    Then
                        '強化アビリティ
                        buf = LIndex(.ConditionData(i), 1) & "強化Lv" & .ConditionLevel(i)
                    ElseIf .ConditionLevel(i) > 0 Then
                        '付加アビリティ(レベル指定あり)
                        buf = Left$(.Condition(i), Len(.Condition(i)) - 2) _
                            & "Lv" & Format$(.ConditionLevel(i)) & "付加"
                    Else
                        '付加アビリティ(レベル指定なし)
                        buf = .Condition(i)
                    End If
                    
                    'エリアスされた特殊能力の付加表示がたぶらないように
                    For j = 1 To UBound(name_list)
                        If buf = name_list(j) Then
                            Exit For
                        End If
                    Next
                    If j <= UBound(name_list) Then
                        GoTo NextCondition
                    End If
                    ReDim Preserve name_list(UBound(name_list) + 1)
                    name_list(UBound(name_list)) = buf
                    
                    upic.Print buf;
                    
                    If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
                        upic.Print " 残り" & Format$(.ConditionLifetime(i)) & "ターン";
                    End If
                    upic.Print ""
            End Select
NextCondition:
        Next
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        'ＨＰ
        cx = upic.CurrentX
        cy = upic.CurrentY
        upic.Line _
            (116, cy + 2)-(118 + GauageWidth, cy + 2), rgb(100, 100, 100)
        upic.Line _
            (116, cy + 2)-(116, cy + 9), rgb(100, 100, 100)
        upic.Line _
            (117, cy + 8)-(118 + GauageWidth, cy + 8), rgb(220, 220, 220)
        upic.Line _
            (118 + GauageWidth, cy + 3)-(118 + GauageWidth, cy + 9), rgb(220, 220, 220)
        upic.Line _
            (117, cy + 3)-(117 + GauageWidth, cy + 7), rgb(200, 0, 0), BF
        If .HP > 0 Then
            upic.Line _
                (117, cy + 3)-(117 + GauageWidth * .HP \ .MaxHP, cy + 7), rgb(0, 210, 0), BF
        End If
        upic.CurrentX = cx
        upic.CurrentY = cy
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("ＨＰ", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        If .IsConditionSatisfied("データ不明") Then
            upic.Print "?????/?????"
        Else
            If .HP < 100000 Then
                upic.Print Format$(.HP);
            Else
                upic.Print "?????";
            End If
            upic.Print "/";
            If .MaxHP < 100000 Then
                upic.Print Format$(.MaxHP)
            Else
                upic.Print "?????"
            End If
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        'ＥＮ
        cx = upic.CurrentX
        cy = upic.CurrentY
        upic.Line _
            (116, cy + 2)-(118 + GauageWidth, cy + 2), rgb(100, 100, 100)
        upic.Line _
            (116, cy + 2)-(116, cy + 9), rgb(100, 100, 100)
        upic.Line _
            (117, cy + 8)-(118 + GauageWidth, cy + 8), rgb(220, 220, 220)
        upic.Line _
            (118 + GauageWidth, cy + 3)-(118 + GauageWidth, cy + 9), rgb(220, 220, 220)
        upic.Line _
            (117, cy + 3)-(117 + GauageWidth, cy + 7), rgb(200, 0, 0), BF
        If .EN > 0 Then
            upic.Line _
                (117, cy + 3)-(117 + GauageWidth * .EN \ .MaxEN, cy + 7), rgb(0, 210, 0), BF
        End If
        upic.CurrentX = cx
        upic.CurrentY = cy
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("ＥＮ", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        If .IsConditionSatisfied("データ不明") Then
            upic.Print "???/???"
        Else
            If .EN < 1000 Then
                upic.Print Format$(.EN);
            Else
                upic.Print "???";
            End If
            upic.Print "/";
            If .MaxEN < 1000 Then
                upic.Print Format$(.MaxEN)
            Else
                upic.Print "???"
            End If
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '装甲
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("装甲", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        Select Case .Armor("修正値")
            Case Is > 0
                upic.Print RightPaddedString _
                    (Format$(.Armor("基本値")) & "+" & Format$(.Armor("修正値")), 12);
            Case Is < 0
                upic.Print RightPaddedString _
                    (Format$(.Armor("基本値")) & Format$(.Armor("修正値")), 12);
            Case 0
                upic.Print RightPaddedString(Format$(.Armor), 12);
        End Select
        
        '運動性
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("運動性", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        Select Case .Mobility("修正値")
            Case Is > 0
                upic.Print Format$(.Mobility("基本値")) & "+" & Format$(.Mobility("修正値"))
            Case Is < 0
                upic.Print Format$(.Mobility("基本値")) & Format$(.Mobility("修正値"))
            Case 0
                upic.Print Format$(.Mobility)
        End Select
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '移動タイプ
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("タイプ", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        upic.Print _
            RightPaddedString(.Transportation, 12);
        
        '移動力
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("移動力", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        If .IsFeatureAvailable("テレポート") _
            And (.Data.Speed = 0 Or LIndex(.FeatureData("テレポート"), 2) = "0") _
        Then
            upic.Print Format$(.Speed + .FeatureLevel("テレポート"))
        Else
            upic.Print Format$(.Speed)
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '地形適応
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print "適応   ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        For i = 1 To 4
            Select Case .Adaption(i)
                Case 5
                    upic.Print "S";
                Case 4
                    upic.Print "A";
                Case 3
                    upic.Print "B";
                Case 2
                    upic.Print "C";
                Case 1
                    upic.Print "D";
                Case Else
                    upic.Print "E";
            End Select
        Next
        upic.Print Space$(8);
        
        'ユニットサイズ
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print Term("サイズ", u, 6) & " ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        upic.Print StrConv(.Size, vbWide)
        
        '防御属性の表示
        n = 0
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '吸収
        If Len(.strAbsorb) > 0 _
            And InStr(.strAbsorb, "非表示") = 0 _
        Then
            If Len(.strAbsorb) > 5 Then
                If n > 0 Then
                    upic.Print
                End If
                n = 2
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "吸収   ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strAbsorb, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '無効化
        If Len(.strImmune) > 0 _
            And InStr(.strImmune, "非表示") = 0 _
        Then
            If Len(.strImmune) > 5 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                n = 2
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "無効化 ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strImmune, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '耐性
        If Len(.strResist) > 0 _
            And InStr(.strResist, "非表示") = 0 _
        Then
            If Len(.strResist) > 5 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                n = 2
            End If
            If n = 0 And NewGUIMode Then
                upic.CurrentX = 5
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "耐性   ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strResist, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '弱点
        If Len(.strWeakness) > 0 _
            And InStr(.strWeakness, "非表示") = 0 _
        Then
            If Len(.strWeakness) > 5 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                n = 2
            End If
            If n = 0 And NewGUIMode Then
                upic.CurrentX = 5
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "弱点   ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strWeakness, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '有効
        If Len(.strEffective) > 0 _
            And InStr(.strEffective, "非表示") = 0 _
        Then
            If Len(.strEffective) > 5 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                n = 2
            End If
            If n = 0 And NewGUIMode Then
                upic.CurrentX = 5
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "有効   ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strEffective, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '特殊効果無効化
        If Len(.strSpecialEffectImmune) > 0 _
            And InStr(.strSpecialEffectImmune, "非表示") = 0 _
        Then
            If Len(.strSpecialEffectImmune) > 5 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                n = 2
            End If
            If n = 0 And NewGUIMode Then
                upic.CurrentX = 5
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "特無効 ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print RightPaddedString(.strSpecialEffectImmune, 12);
            n = n + 1
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
        End If
        
        '必要に応じて改行
        If n > 0 Then
            upic.Print
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
        End If
        n = 0
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '武器・防具クラス
        ReDim flist(0)
        If IsOptionDefined("アイテム交換") Then
            If .IsFeatureAvailable("武器クラス") _
                Or .IsFeatureAvailable("防具クラス") _
            Then
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
                upic.Print RightPaddedString("武器・防具クラス", 19);
                ReDim Preserve flist(1)
                flist(1) = "武器・防具クラス"
                n = n + 1
            End If
        End If
        
        '特殊能力一覧を表示する前に必要気力判定のためメインパイロットの気力を参照
        If .CountPilot > 0 Then
            pmorale = .MainPilot.Morale
        Else
            pmorale = 150
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '特殊能力一覧
        For i = .AdditionalFeaturesNum + 1 To .CountAllFeature
            fname = .AllFeatureName(i)
            
            'ユニットステータスコマンド時は通常は非表示のパーツ合体、
            'ノーマルモード、換装も表示
            If fname = "" Then
                If MapFileName = "" Then
                    Select Case .AllFeature(i)
                        Case "パーツ合体", "ノーマルモード"
                            upic.Print RightPaddedString(.AllFeature(i), 19);
                            n = n + 1
                            If n > 1 Then
                                upic.Print
'ADD START 240a
                                If NewGUIMode Then
                                    upic.CurrentX = 5
                                End If
'ADD  END  240a
                                n = 0
                            End If
                        Case "換装"
                            fname = "換装"
                            
                            'エリアスで換装の名称が変更されている？
                            With ALDList
                                For j = 1 To .Count
                                    With .Item(j)
                                        If .AliasType(1) = "換装" Then
                                            fname = .Name
                                            Exit For
                                        End If
                                    End With
                                Next
                            End With
                            
                            upic.Print RightPaddedString(fname, 19);
                            n = n + 1
                            If n > 1 Then
                                upic.Print
'ADD START 240a
                                If NewGUIMode Then
                                    upic.CurrentX = 5
                                End If
'ADD  END  240a
                                n = 0
                            End If
                    End Select
                End If
                GoTo NextFeature
            End If
            
            '既に表示しているかを判定
            For j = 1 To UBound(flist)
                If fname = flist(j) Then
                    GoTo NextFeature
                End If
            Next
            ReDim Preserve flist(UBound(flist) + 1)
            flist(UBound(flist)) = fname
            
            '使用可否によって表示色を変える
            fdata = .AllFeatureData(i)
            Select Case .AllFeature(i)
                Case "合体"
                    If Not UList.IsDefined(LIndex(fdata, 2)) Then
                       GoTo NextFeature
                    End If
                    If UList.Item(LIndex(fdata, 2)).IsConditionSatisfied("行動不能") Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "分離"
                    k = 0
                    For j = 2 To LLength(fdata)
                        If Not UList.IsDefined(LIndex(fdata, j)) Then
                            GoTo NextFeature
                        End If
                        With UList.Item(LIndex(fdata, j)).Data
                            If .IsFeatureAvailable("召喚ユニット") Then
                                k = k + Abs(.PilotNum)
                            End If
                        End With
                    Next
                    If .CountPilot < k Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "ハイパーモード"
                    If pmorale < CInt(10# * .FeatureLevel(i)) + 100 _
                        And .HP > .MaxHP \ 4 _
                    Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf .IsConditionSatisfied("ノーマルモード付加") Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "修理装置", "補給装置"
                    If IsNumeric(LIndex(fdata, 2)) Then
                        If .EN < CInt(LIndex(fdata, 2)) Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                    End If
                Case "テレポート"
                    If IsNumeric(LIndex(fdata, 2)) Then
                        If .EN < CInt(LIndex(fdata, 2)) Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                    Else
                        If .EN < 40 Then
'MOD START 240a
'                            upic.ForeColor = rgb(150, 0, 0)
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                        End If
                    End If
                Case "分身"
                    If pmorale < 130 Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "超回避"
                    If IsNumeric(LIndex(fdata, 2)) Then
                        ecost = CInt(LIndex(fdata, 2))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 3)) Then
                        nmorale = CInt(LIndex(fdata, 3))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "緊急テレポート"
                    If IsNumeric(LIndex(fdata, 3)) Then
                        ecost = CInt(LIndex(fdata, 3))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 4)) Then
                        nmorale = CInt(LIndex(fdata, 4))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "エネルギーシールド"
                    If .EN < 5 Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "バリア", "バリアシールド", "プロテクション", "アクティブプロテクション"
                    If IsNumeric(LIndex(fdata, 3)) Then
                        ecost = CInt(LIndex(fdata, 3))
                    Else
                        ecost = 10
                    End If
                    If IsNumeric(LIndex(fdata, 4)) Then
                        nmorale = CInt(LIndex(fdata, 4))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale _
                        Or (.IsConditionSatisfied("バリア無効化") _
                            And InStr(fdata, "バリア無効化無効") = 0) _
                    Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf InStr(fdata, "能力必要") > 0 Then
                        For j = 5 To LLength(fdata)
                            opt = LIndex(fdata, j)
                            If InStr(opt, "*") > 0 Then
                                opt = Left$(opt, InStr(opt, "*") - 1)
                            End If
                            Select Case opt
                                Case "相殺", "中和", "近接無効", "手動", "能力必要"
                                    'スキップ
                                Case "同調率"
                                    If .SyncLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case "霊力"
                                    If .PlanaLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case Else
                                    If .SkillLevel(opt) = 0 Then
                                        GoTo NextFeature
                                    End If
                            End Select
                        Next
                    End If
                Case "フィールド", "アクティブフィールド"
                    If IsNumeric(LIndex(fdata, 3)) Then
                        ecost = CInt(LIndex(fdata, 3))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 4)) Then
                        nmorale = CInt(LIndex(fdata, 4))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale _
                        Or (.IsConditionSatisfied("バリア無効化") _
                            And InStr(fdata, "バリア無効化無効") = 0) _
                    Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf InStr(fdata, "能力必要") > 0 Then
                        For j = 5 To LLength(fdata)
                            opt = LIndex(fdata, j)
                            If InStr(opt, "*") > 0 Then
                                opt = Left$(opt, InStr(opt, "*") - 1)
                            End If
                            Select Case opt
                                Case "相殺", "中和", "近接無効", "手動", "能力必要"
                                    'スキップ
                                Case "同調率"
                                    If .SyncLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case "霊力"
                                    If .PlanaLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case Else
                                    If .SkillLevel(opt) = 0 Then
                                        GoTo NextFeature
                                    End If
                            End Select
                        Next
                    End If
                Case "広域バリア", "広域フィールド", "広域プロテクション"
                    If IsNumeric(LIndex(fdata, 4)) Then
                        ecost = CInt(LIndex(fdata, 4))
                    ElseIf IsNumeric(LIndex(fdata, 2)) Then
                        ecost = 20 * CInt(LIndex(fdata, 2))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 5)) Then
                        nmorale = CInt(LIndex(fdata, 5))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale _
                        Or (.IsConditionSatisfied("バリア無効化") _
                            And InStr(fdata, "バリア無効化無効") = 0) _
                    Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                    fname = fname & "(範囲" & LIndex(fdata, 2) & "マス)"
                Case "アーマー", "レジスト"
                    If IsNumeric(LIndex(fdata, 3)) Then
                        nmorale = CInt(LIndex(fdata, 3))
                    Else
                        nmorale = 0
                    End If
                    If pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf InStr(fdata, "能力必要") > 0 Then
                        For j = 4 To LLength(fdata)
                            opt = LIndex(fdata, j)
                            If InStr(opt, "*") > 0 Then
                                opt = Left$(opt, InStr(opt, "*") - 1)
                            End If
                            Select Case opt
                                Case "同調率"
                                    If .SyncLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case "霊力"
                                    If .PlanaLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case Else
                                    If .SkillLevel(opt) = 0 Then
                                        GoTo NextFeature
                                    End If
                            End Select
                        Next
                    End If
                Case "攻撃回避"
                    If IsNumeric(LIndex(fdata, 3)) Then
                        nmorale = CInt(LIndex(fdata, 3))
                    Else
                        nmorale = 0
                    End If
                    If pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "反射", "阻止"
                    If IsNumeric(LIndex(fdata, 4)) Then
                        ecost = CInt(LIndex(fdata, 4))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 5)) Then
                        nmorale = CInt(LIndex(fdata, 5))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf InStr(fdata, "能力必要") > 0 Then
                        For j = 6 To LLength(fdata)
                            opt = LIndex(fdata, j)
                            If InStr(opt, "*") > 0 Then
                                opt = Left$(opt, InStr(opt, "*") - 1)
                            End If
                            Select Case opt
                                Case "相殺", "中和", "近接無効", "手動", "能力必要"
                                    'スキップ
                                Case "同調率"
                                    If .SyncLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case "霊力"
                                    If .PlanaLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case Else
                                    If .SkillLevel(opt) = 0 Then
                                        GoTo NextFeature
                                    End If
                            End Select
                        Next
                    End If
                Case "広域阻止"
                    If IsNumeric(LIndex(fdata, 5)) Then
                        ecost = CInt(LIndex(fdata, 5))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 6)) Then
                        nmorale = CInt(LIndex(fdata, 6))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                    fname = fname & "(範囲" & LIndex(fdata, 2) & "マス)"
                Case "当て身技", "自動反撃"
                    If IsNumeric(LIndex(fdata, 5)) Then
                        ecost = CInt(LIndex(fdata, 5))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 6)) Then
                        nmorale = CInt(LIndex(fdata, 6))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    ElseIf InStr(fdata, "能力必要") > 0 Then
                        For j = 7 To LLength(fdata)
                            opt = LIndex(fdata, j)
                            If InStr(opt, "*") > 0 Then
                                opt = Left$(opt, InStr(opt, "*") - 1)
                            End If
                            Select Case opt
                                Case "相殺", "中和", "近接無効", "手動", "能力必要"
                                    'スキップ
                                Case "同調率"
                                    If .SyncLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case "霊力"
                                    If .PlanaLevel = 0 Then
                                        GoTo NextFeature
                                    End If
                                Case Else
                                    If .SkillLevel(opt) = 0 Then
                                        GoTo NextFeature
                                    End If
                            End Select
                        Next
                    End If
                Case "ブースト"
                    If pmorale >= 130 Then
'MOD START 240a
'                        upic.ForeColor = vbBlue
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                    End If
                Case "盾"
                    If .ConditionLevel("盾ダメージ") >= .AllFeatureLevel("盾") Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                    fname = fname & "(" & _
                        Format$(MaxLng(.AllFeatureLevel("盾") - .ConditionLevel("盾ダメージ"), 0)) & _
                        "/" & Format$(.AllFeatureLevel("盾")) & ")"
                Case "ＨＰ回復", "ＥＮ回復"
' MOD START MARGE
'                    If .IsConditionSatisfied("回復不能") Then
                    If .IsConditionSatisfied("回復不能") _
                        Or .IsSpecialPowerInEffect("回復不能") _
                    Then
' MOD END MARGE
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
                Case "格闘強化", "射撃強化", "命中強化", "回避強化", "技量強化", "反応強化", _
                    "ＨＰ強化", "ＥＮ強化", "装甲強化", "運動性強化", "移動力強化", _
                    "ＨＰ割合強化", "ＥＮ割合強化", "装甲割合強化", "運動性割合強化"
                    If IsNumeric(LIndex(fdata, 2)) Then
                        If pmorale >= StrToLng(LIndex(fdata, 2)) Then
'MOD START 240a
'                            upic.ForeColor = vbBlue
                            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityEnable, vbBlue)
'MOD  END  240a
                        End If
                    End If
                Case "ＺＯＣ"
                    If LLength(fdata) < 2 Then
                        j = 1
                    Else
                        j = CInt(LIndex(fdata, 2))
                    End If
                    If j >= 1 Then
                        ReplaceString fdata, vbTab, " "
                        If InStr(fdata, " 直線") > 0 _
                            Or (InStr(fdata, " 垂直") > 0 _
                                And InStr(fdata, " 水平") > 0) _
                        Then
                            buf = "直線"
                        ElseIf InStr(fdata, " 垂直") > 0 Then
                            buf = "上下"
                        ElseIf InStr(fdata, " 水平") > 0 Then
                            buf = "左右"
                        Else
                            buf = "範囲"
                        End If
                        fname = fname & "(" & buf & Format$(j) & "マス)"
                    End If
                Case "広域ＺＯＣ無効化"
                    fname = fname & "(範囲" & LIndex(fdata, 2) & "マス)"
                Case "追加攻撃"
                    If IsNumeric(LIndex(fdata, 5)) Then
                        ecost = CInt(LIndex(fdata, 5))
                    Else
                        ecost = 0
                    End If
                    If IsNumeric(LIndex(fdata, 6)) Then
                        nmorale = CInt(LIndex(fdata, 6))
                    Else
                        nmorale = 0
                    End If
                    If .EN < ecost Or pmorale < nmorale Then
'MOD START 240a
'                        upic.ForeColor = rgb(150, 0, 0)
                        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
                    End If
            End Select
            
            '必要条件を満たさない特殊能力は赤色で表示
            If Not .IsFeatureActivated(i) Then
'MOD START 240a
'                upic.ForeColor = rgb(150, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
            End If
            
            '特殊能力名を表示
            If LenB(StrConv(fname, vbFromUnicode)) > 19 Then
                If n > 0 Then
                    upic.Print
'ADD START 240a
                    If NewGUIMode Then
                        upic.CurrentX = 5
                    End If
'ADD  END  240a
                End If
                upic.Print fname;
                n = 2
            Else
                upic.Print RightPaddedString(fname, 19);
                n = n + 1
            End If
            
            '必要に応じて改行
            If n > 1 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
                n = 0
            End If
            
            '表示色を戻しておく
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
NextFeature:
        Next
        If n > 0 Then
            upic.Print
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        'アイテム一覧
        If .CountItem > 0 Then
            j = 0
            For i = 1 To .CountItem
                With .Item(i)
                    '表示指定を持つアイテムのみ表示する
                    If Not .IsFeatureAvailable("表示") Then
                        GoTo NextItem
                    End If
                    
                    'アイテム名を表示
                    If Len(.Nickname) > 9 Then
                        If j = 1 Then
                            upic.Print
'ADD START 240a
                            If NewGUIMode Then
                                upic.CurrentX = 5
                            End If
'ADD  END  240a
                        End If
                        upic.Print .Nickname;
                        j = 2
                    Else
                        upic.Print _
                            RightPaddedString(.Nickname, 19);
                        j = j + 1
                    End If
                    If j = 2 Then
                        upic.Print
'ADD START 240a
                        If NewGUIMode Then
                            upic.CurrentX = 5
                        End If
'ADD  END  240a
                        j = 0
                    End If
                End With
NextItem:
            Next
            If j > 0 Then
                upic.Print
'ADD START 240a
                If NewGUIMode Then
                    upic.CurrentX = 5
                End If
'ADD  END  240a
            End If
        End If
        
        'ターゲット選択時の攻撃結果予想表示
        
        '攻撃時にのみ表示
        If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択") _
            And (SelectedCommand = "攻撃" Or SelectedCommand = "マップ攻撃") _
            And Not SelectedUnit Is Nothing _
            And SelectedWeapon > 0 _
            And Stage <> "プロローグ" And Stage <> "エピローグ" _
        Then
            '攻撃時と判定
        Else
            GoTo SkipAttackExpResult
        End If
        
        '相手が敵の場合にのみ表示
        If .Party <> "敵" _
            And .Party <> "中立" _
            And Not .IsConditionSatisfied("暴走") _
            And Not .IsConditionSatisfied("魅了") _
            And Not .IsConditionSatisfied("憑依") _
            And Not .IsConditionSatisfied("混乱") _
            And Not .IsConditionSatisfied("睡眠") _
        Then
            GoTo SkipAttackExpResult
        End If
        
        upic.Print
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '攻撃手段
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print "攻撃     ";
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        upic.Print SelectedUnit.WeaponNickname(SelectedWeapon);
        'サポートアタックを得られる？
        If Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "合") _
            And Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") _
            And UseSupportAttack _
        Then
            If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
                upic.Print " [援]"
            Else
                upic.Print
            End If
        Else
            upic.Print
        End If
        
        '反撃を受ける？
        If .MaxAction = 0 _
            Or SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") _
            Or SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "間") _
        Then
            w = 0
        Else
            w = SelectWeapon(u, SelectedUnit, "反撃")
        End If
        
        '敵の防御行動を設定
        def_mode = SelectDefense(SelectedUnit, SelectedWeapon, u, w)
        If def_mode <> "" Then
            w = 0
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '予測ダメージ
        If Not IsOptionDefined("予測ダメージ非表示") Then
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "ダメージ ";
            dmg = SelectedUnit.Damage(SelectedWeapon, u, True)
            If def_mode = "防御" Then
                dmg = dmg \ 2
            End If
            If dmg >= .HP And Not .IsConditionSatisfied("データ不明") Then
                upic.ForeColor = rgb(190, 0, 0)
            Else
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            End If
            upic.Print Format$(dmg)
        End If
        
'ADD START 240a
        If NewGUIMode Then
            upic.CurrentX = 5
        End If
'ADD  END  240a
        '予測命中率
        If Not IsOptionDefined("予測命中率非表示") Then
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "命中率   ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            prob = SelectedUnit.HitProbability(SelectedWeapon, u, True)
            If def_mode = "回避" Then
                prob = prob \ 2
            End If
            cprob = SelectedUnit.CriticalProbability(SelectedWeapon, u, def_mode)
            upic.Print MinLng(prob, 100) & "％（" & cprob & "％）"
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        End If
        
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
        If w > 0 Then
            '反撃手段
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            upic.Print "反撃     ";
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            upic.Print .WeaponNickname(w);
            'サポートガードを受けられる？
            If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
                upic.Print " [援]"
            Else
                upic.Print
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '予測ダメージ
            If Not IsOptionDefined("予測ダメージ非表示") Then
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print "ダメージ ";
                dmg = .Damage(w, SelectedUnit, True)
                If dmg >= SelectedUnit.HP Then
                    upic.ForeColor = rgb(190, 0, 0)
                Else
'MOD START 240a
'                    upic.ForeColor = rgb(0, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                End If
                upic.Print Format$(dmg)
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            '予測命中率
            If Not IsOptionDefined("予測命中率非表示") Then
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 150)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
                upic.Print "命中率   ";
'MOD START 240a
'                upic.ForeColor = rgb(0, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
                prob = .HitProbability(w, SelectedUnit, True)
                cprob = .CriticalProbability(w, SelectedUnit)
                upic.Print Format$(MinLng(prob, 100)) & "％（" & cprob & "％）"
            End If
        Else
            '相手は反撃できない
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 150)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
            If def_mode <> "" Then
                upic.Print def_mode;
            Else
                upic.Print "反撃不能";
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
            'サポートガードを受けられる？
            If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
                upic.Print " [援]"
            Else
                upic.Print
            End If
        End If
        
SkipAttackExpResult:
        
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
        '武器一覧
        upic.CurrentY = upic.CurrentY + 8
        upic.Print Space$(25);
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 150)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityName, rgb(0, 0, 150))
'MOD  END  240a
        upic.Print "攻撃 射程"
'MOD START 240a
'        upic.ForeColor = rgb(0, 0, 0)
        upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
        
        ReDim warray(.CountWeapon)
        ReDim wpower(.CountWeapon)
        For i = 1 To .CountWeapon
            wpower(i) = .WeaponPower(i, "")
        Next
        
        '攻撃力でソート
        For i = 1 To .CountWeapon
            For j = 1 To i - 1
                If wpower(i) > wpower(warray(i - j)) Then
                    Exit For
                ElseIf wpower(i) = wpower(warray(i - j)) Then
                    If .Weapon(i).ENConsumption > 0 Then
                        If .Weapon(i).ENConsumption >= .Weapon(warray(i - j)).ENConsumption Then
                            Exit For
                        End If
                    ElseIf .Weapon(i).Bullet > 0 Then
                        If .Weapon(i).Bullet <= .Weapon(warray(i - j)).Bullet Then
                            Exit For
                        End If
                    Else
                        If .Weapon(i - j).ENConsumption = 0 _
                            And .Weapon(warray(i - j)).Bullet = 0 _
                        Then
                            Exit For
                        End If
                    End If
                End If
            Next
            For k = 1 To j - 1
                warray(i - k + 1) = warray(i - k)
            Next
            warray(i - j + 1) = i
        Next
        
        '個々の武器を表示
        For i = 1 To .CountWeapon
            If upic.CurrentY > 420 Then
                Exit For
            End If
            w = warray(i)
            If Not .IsWeaponAvailable(w, "ステータス") Then
                '習得していない技は表示しない
                If Not .IsWeaponMastered(w) Then
                    GoTo NextWeapon
                End If
                'Disableコマンドで使用不可になった武器も同様
                If .IsDisabled(.Weapon(w).Name) Then
                    GoTo NextWeapon
                End If
                'フォーメーションを満たしていない合体技も
                If .IsWeaponClassifiedAs(w, "合") Then
                    If Not .IsCombinationAttackAvailable(w, True) Then
                        GoTo NextWeapon
                    End If
                End If
'MOD START 240a
'                    upic.ForeColor = rgb(150, 0, 0)
                    upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
            End If
            
            '武器の表示
            If .WeaponPower(w, "") < 10000 Then
                buf = RightPaddedString(Format$(.WeaponNickname(w)), 25)
                buf = buf & LeftPaddedString(Format$(.WeaponPower(w, "")), 4)
            Else
                buf = RightPaddedString(Format$(.WeaponNickname(w)), 24)
                buf = buf & LeftPaddedString(Format$(.WeaponPower(w, "")), 5)
            End If
            
            '武器が特殊効果を持つ場合は略称で表記
            If .WeaponMaxRange(w) > 1 Then
                Dim wclass As String
                buf = buf & _
                    LeftPaddedString(Format$(.Weapon(w).MinRange) & "-" & Format$(.WeaponMaxRange(w)), _
                        34 - LenB(StrConv(buf, vbFromUnicode)))
                '移動後攻撃可能
                If .IsWeaponClassifiedAs(w, "Ｐ") Then
                    buf = buf & "P"
                End If
            Else
                buf = buf & LeftPaddedString("1", 34 - LenB(StrConv(buf, vbFromUnicode)))
' ADD START MARGE
                '移動後攻撃不可
                If .IsWeaponClassifiedAs(w, "Ｑ") Then
                    buf = buf & "Q"
                End If
' ADD END MARGE
            End If
            'マップ攻撃
            If .IsWeaponClassifiedAs(w, "Ｍ") Then
                buf = buf & "M"
            End If
            '特殊効果
            wclass = .Weapon(w).Class
            For j = 1 To .CountWeaponEffect(w)
                buf = buf & "+"
            Next
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            upic.Print buf
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
NextWeapon:
        Next
        
        'アビリティ一覧
        For i = 1 To .CountAbility
            If upic.CurrentY > 420 Then
                Exit For
            End If
            If Not .IsAbilityAvailable(i, "ステータス") Then
                '習得していない技は表示しない
                If Not .IsAbilityMastered(i) Then
                    GoTo NextAbility
                End If
                'Disableコマンドで使用不可になった武器も同様
                If .IsDisabled(.Ability(i).Name) Then
                    GoTo NextAbility
                End If
                'フォーメーションを満たしていない合体技も
                If .IsAbilityClassifiedAs(i, "合") Then
                    If Not .IsCombinationAbilityAvailable(i, True) Then
                        GoTo NextAbility
                    End If
                End If
'MOD START 240a
'                upic.ForeColor = rgb(150, 0, 0)
                upic.ForeColor = IIf(NewGUIMode, StatusFontColorAbilityDisable, rgb(150, 0, 0))
'MOD  END  240a
            End If
            
'ADD START 240a
            If NewGUIMode Then
                upic.CurrentX = 5
            End If
'ADD  END  240a
            'アビリティの表示
            upic.Print RightPaddedString(Format$(.AbilityNickname(i)), 29);
            If .AbilityMaxRange(i) > 1 Then
                upic.Print _
                    LeftPaddedString(Format$(.AbilityMinRange(i)) & _
                        "-" & Format$(.AbilityMaxRange(i)), 5);
                If .IsAbilityClassifiedAs(i, "Ｐ") Then
                    upic.Print "P";
                End If
                If .IsAbilityClassifiedAs(i, "Ｍ") Then
                    upic.Print "M";
                End If
                upic.Print
            ElseIf .AbilityMaxRange(i) = 1 Then
                upic.Print "    1";
' ADD START MARGE
                If .IsAbilityClassifiedAs(i, "Ｑ") Then
                    upic.Print "Q";
                End If
' ADD END MARGE
                If .IsAbilityClassifiedAs(i, "Ｍ") Then
                    upic.Print "M";
                End If
                upic.Print
            Else
                upic.Print "    -"
            End If
'MOD START 240a
'            upic.ForeColor = rgb(0, 0, 0)
            upic.ForeColor = IIf(NewGUIMode, StatusFontColorNormalString, rgb(0, 0, 0))
'MOD  END  240a
NextAbility:
        Next
    End With
    
UpdateStatusWindow:
    
'MOD START 240a
'    If MainWidth = 15 Then
    If Not NewGUIMode Then
'MOD  END
        'ステータスウィンドウをリフレッシュ
        MainForm.picFace.Refresh
        ppic.Refresh
        upic.Refresh
    Else
        If MouseX < MainPWidth \ 2 Then
'MOD START 240a
'            upic.Move MainPWidth - 230 - 5, 10
            '画面左側にカーソルがある場合
            upic.Move MainPWidth - 240, 10
'MOD  END
        Else
            upic.Move 5, 10
        End If
        If upic.Visible Then
            upic.Refresh
        Else
            upic.Visible = True
        End If
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "パイロット用画像ファイル" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
        & "画像ファイルが壊れていないか確認して下さい。"
End Sub

'指定されたパイロットのステータスをステータスウィンドウに表示
Public Sub DisplayPilotStatus(ByVal p As Pilot)
Dim i As Integer
    
    Set DisplayedUnit = p.Unit
    
    With DisplayedUnit
        If p Is .MainPilot Then
            'メインパイロット
            DisplayUnitStatus DisplayedUnit, 0
        Else
            'サブパイロット
            For i = 1 To .CountPilot
                If p Is .Pilot(i) Then
                    DisplayUnitStatus DisplayedUnit, i
                    Exit Sub
                End If
            Next
            
            'サポートパイロット
            For i = 1 To .CountSupport
                If p Is .Support(i) Then
                    DisplayUnitStatus DisplayedUnit, i + .CountPilot
                    Exit Sub
                End If
            Next
            
            '追加サポート
            If .IsFeatureAvailable("追加サポート") Then
                DisplayUnitStatus DisplayedUnit, .CountPilot + .CountSupport + 1
            End If
        End If
    End With
End Sub

'指定したマップ座標にいるユニットのステータスをステータスウィンドウに表示
Public Sub InstantUnitStatusDisplay(ByVal X As Integer, ByVal Y As Integer)
Dim u As Unit
    
    '指定された座標にいるユニットを収得
    Set u = MapDataForUnit(X, Y)
    
    '発進コマンドの場合は母艦ではなく発進するユニットを使う
    If CommandState = "ターゲット選択" And SelectedCommand = "発進" Then
        If u Is SelectedUnit Then
            Set u = SelectedTarget
            If u Is Nothing Then
                Exit Sub
            End If
        End If
    End If
    
    If DisplayedUnit Is Nothing Then
        'ステータスウィンドウに何も表示されていなければ無条件で表示
    Else
        '同じユニットが表示されていればスキップ
        If u Is DisplayedUnit Then
            Exit Sub
        End If
    End If
    
    DisplayUnitStatus u
End Sub

'ステータスウィンドウをクリア
Public Sub ClearUnitStatus()
    If MainWidth = 15 Then
        MainForm.picFace = LoadPicture("")
        MainForm.picPilotStatus.Cls
        MainForm.picUnitStatus.Cls
        Set DisplayedUnit = Nothing
    Else
        MainForm.picUnitStatus.Visible = False
        MainForm.picUnitStatus.Cls
        IsStatusWindowDisabled = True
        DoEvents
        IsStatusWindowDisabled = False
'ADD
        Set DisplayedUnit = Nothing
    End If
End Sub

'ADD START 240a
'新ＧＵＩ時のグローバルステータスウインドウのサイズを取得する
Private Function GetGlobalStatusSize(X As Integer, Y As Integer) As Long
Dim ret As Long
    ret = 42
    If Not (X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y) Then
        '地形情報の表示が確定
        ret = 106
        'ＨＰ・ＥＮ回復が記述される場合
        If TerrainEffectForHPRecover(X, Y) > 0 Or TerrainEffectForENRecover(X, Y) > 0 Then
            ret = ret + 16
        End If
        'ＨＰ・ＥＮ減少が記述される場合
        If TerrainHasFeature(X, Y, "ＨＰ減少") Or TerrainHasFeature(X, Y, "ＥＮ減少") Then
            ret = ret + 16
        End If
        'ＨＰ・ＥＮ増加が記述される場合
        If TerrainHasFeature(X, Y, "ＨＰ増加") Or TerrainHasFeature(X, Y, "ＥＮ増加") Then
            ret = ret + 16
        End If
        'ＨＰ・ＥＮ低下が記述される場合
        If TerrainHasFeature(X, Y, "ＨＰ低下") Or TerrainHasFeature(X, Y, "ＥＮ低下") Then
            ret = ret + 16
        End If
        '摩擦・状態付加が記述される場合
        If TerrainHasFeature(X, Y, "摩擦") Or TerrainHasFeature(X, Y, "状態付加") Then
            ret = ret + 16
        End If
    End If
    GetGlobalStatusSize = ret
End Function

'Global変数とステータス描画系変数の同期。
Private Sub GlobalVariableLoad()
    '背景色
    If IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
        If Not StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)") Then
            StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)")
        End If
    End If
    '枠の色
    If IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
        If Not StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)") Then
            StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)")
        End If
    End If
    '枠の太さ
    If IsGlobalVariableDefined("StatusWindow(FrameWidth)") Then
        If Not StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)") Then
            StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)")
        End If
    End If
    '能力名の色
    If IsGlobalVariableDefined("StatusWindow(ANameColor)") Then
        If Not StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)") Then
            StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)")
        End If
    End If
    '有効な能力の色
    If IsGlobalVariableDefined("StatusWindow(EnableColor)") Then
        If Not StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)") Then
            StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)")
        End If
    End If
    '無効な能力の色
    If IsGlobalVariableDefined("StatusWindow(DisableColor)") Then
        If Not StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)") Then
            StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)")
        End If
    End If
    '通常文字の色
    If IsGlobalVariableDefined("StatusWindow(StringColor)") Then
        If Not StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)") Then
            StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)")
        End If
    End If
End Sub
'ADD  END  240a

