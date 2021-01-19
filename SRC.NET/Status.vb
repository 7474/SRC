Option Strict Off
Option Explicit On
Module Status
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public DisplayedUnit As Unit
	Public DisplayedPilotInd As Short
	
	'Invalid_string_refer_to_original_code
	Public IsStatusWindowDisabled As Boolean
	'ADD START 240a
	'Invalid_string_refer_to_original_code
	Public StatusWindowBackBolor As Integer
	'Invalid_string_refer_to_original_code
	Public StatusWindowFrameColor As Integer
	'Invalid_string_refer_to_original_code
	Public StatusWindowFrameWidth As Integer
	'Invalid_string_refer_to_original_code
	Public StatusFontColorAbilityName As Integer
	'Invalid_string_refer_to_original_code
	Public StatusFontColorAbilityEnable As Integer
	'Invalid_string_refer_to_original_code
	Public StatusFontColorAbilityDisable As Integer
	'Invalid_string_refer_to_original_code
	Public StatusFontColorNormalString As Integer
	'ADD  END
	
	'Invalid_string_refer_to_original_code
	Public Sub DisplayGlobalStatus()
		Dim X, Y As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim td As TerrainData
		'ADD START 240a
		Dim fname As String
		Dim wHeight As Short
		Dim lineStart, ret, color, lineEnd As Integer
		'ADD  END  240a
		
		'Invalid_string_refer_to_original_code
		ClearUnitStatus()
		
		'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		pic = MainForm.picUnitStatus
		
		pic.Font = VB6.FontChangeSize(pic.Font, 12)
		
		'ADD START 240a
		'Invalid_string_refer_to_original_code
		X = PixelToMapX(MouseX)
		Y = PixelToMapY(MouseY)
		
		If NewGUIMode Then
			'Invalid_string_refer_to_original_code
			GlobalVariableLoad()
			pic.BackColor = System.Drawing.ColorTranslator.FromOle(StatusWindowBackBolor)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.DrawWidth = StatusWindowFrameWidth
			color = StatusWindowFrameColor
			lineStart = (StatusWindowFrameWidth - 1) / 2
			lineEnd = (StatusWindowFrameWidth + 1) / 2
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.FillStyle = vbFSTransparent
			'荳�譌ｦ鬮倥＆繧呈怙螟ｧ縺ｫ縺吶ｋ
			pic.Width = VB6.TwipsToPixelsX(235)
			pic.Height = VB6.TwipsToPixelsY(MapPHeight - 20)
			wHeight = GetGlobalStatusSize(X, Y)
			'譫�邱壹ｒ蠑輔￥
			'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Line (lineStart, lineStart) - (235 - lineEnd, wHeight - lineEnd), color, B
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.FillStyle = ObjFillStyle
			'Invalid_string_refer_to_original_code
			pic.Height = VB6.TwipsToPixelsY(wHeight)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentY = 5
			'Invalid_string_refer_to_original_code
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
		End If
		'ADD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Print("繧ｿ繝ｼ繝ｳ謨ｰ " & VB6.Format(Turn))
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Print(Term("Invalid_string_refer_to_original_code", Nothing, 8) & " " & VB6.Format(Money))
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'    X = PixelToMapX(MouseX)
		'    Y = PixelToMapY(MouseY)
		'MOV  END  240a
		
		'Invalid_string_refer_to_original_code
		If X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y Then
			pic.Font = VB6.FontChangeSize(pic.Font, 9)
			If NewGUIMode Then
				'Invalid_string_refer_to_original_code
				pic.Height = VB6.TwipsToPixelsY(wHeight)
			End If
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Print()
		
		'蝨ｰ蠖｢蜷咲ｧｰ
		'ADD START 240a
		'繝槭ャ繝礼判蜒剰｡ｨ遉ｺ
		If NewGUIMode Then
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = GUI.BitBlt(pic.hDC, 5, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
		Else
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = GUI.BitBlt(pic.hDC, 0, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
		End If
		'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.CurrentX = 37
		'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.CurrentY = 65
		'ADD  END  240a
		If InStr(TerrainName(X, Y), "(") > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("(" & VB6.Format(X) & "," & VB6.Format(Y) & ") " & Left(TerrainName(X, Y), InStr(TerrainName(X, Y), "(") - 1))
		Else
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("(" & VB6.Format(X) & "," & VB6.Format(Y) & ") " & TerrainName(X, Y))
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'蜻ｽ荳ｭ菫ｮ豁｣
		If TerrainEffectForHit(X, Y) >= 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("蝗樣∩ +" & VB6.Format(TerrainEffectForHit(X, Y)) & "%")
		Else
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("蝗樣∩ " & VB6.Format(TerrainEffectForHit(X, Y)) & "%")
		End If
		
		'繝�繝｡繝ｼ繧ｸ菫ｮ豁｣
		If TerrainEffectForDamage(X, Y) >= 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("  髦ｲ蠕｡ +" & VB6.Format(TerrainEffectForDamage(X, Y)) & "%")
		Else
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("  髦ｲ蠕｡ " & VB6.Format(TerrainEffectForDamage(X, Y)) & "%")
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		If TerrainEffectForHPRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print(Term("Invalid_string_refer_to_original_code") & " +" & VB6.Format(TerrainEffectForHPRecover(X, Y)) & "%  ")
		End If
		
		'Invalid_string_refer_to_original_code
		If TerrainEffectForENRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print(Term("Invalid_string_refer_to_original_code") & " +" & VB6.Format(TerrainEffectForENRecover(X, Y)) & "%")
		End If
		
		If TerrainEffectForHPRecover(X, Y) > 0 Or TerrainEffectForENRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print()
		End If
		
		'MOD START 240a
		'    Set td = TDList.Item(MapData(X, Y, 0))
		'Invalid_string_refer_to_original_code
		Select Case MapData(X, Y, Map.MapDataIndex.BoxType)
			Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
				td = TDList.Item(MapData(X, Y, Map.MapDataIndex.TerrainType))
			Case Else
				td = TDList.Item(MapData(X, Y, Map.MapDataIndex.LayerType))
		End Select
		'MOD  END
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Print()
		'End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		If td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print(Term("Invalid_string_refer_to_original_code") & " +" & VB6.Format(1000 * td.FeatureLevel("Invalid_string_refer_to_original_code")) & "  ")
		End If
		If td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print(Term("Invalid_string_refer_to_original_code") & " +" & VB6.Format(10 * td.FeatureLevel("Invalid_string_refer_to_original_code")) & "  ")
		End If
		If td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Or td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print()
		End If
		'MOD  END
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Print()
		'End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'鞫ｩ謫ｦ
		If td.IsFeatureAvailable("鞫ｩ謫ｦ") Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print("鞫ｩ謫ｦLv" & VB6.Format(td.FeatureLevel("鞫ｩ謫ｦ")))
		End If
		' ADD START MARGE
		'迥ｶ諷狗焚蟶ｸ莉伜刈
		If td.IsFeatureAvailable("迥ｶ諷倶ｻ伜刈") Then
			'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Print(td.FeatureData("迥ｶ諷倶ｻ伜刈") & "迥ｶ諷倶ｻ伜刈")
		End If
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		pic.Font = VB6.FontChangeSize(pic.Font, 9)
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub DisplayUnitStatus(ByRef u As Unit, Optional ByVal pindex As Short = 0)
		Dim Invalid_string_refer_to_original_code As Object
		Dim p As Pilot
		Dim k, i, j, n As Short
		Dim ret As Integer
		Dim buf As String
		Dim fdata, fname, opt As String
		Dim stype, sname, slevel As String
		Dim cx, cy As Short
		Dim warray() As Short
		Dim wpower() As Integer
		Dim ppic, upic As System.Windows.Forms.PictureBox
		Dim nmorale, ecost, pmorale As Short
		Dim flist() As String
		Dim is_unknown As Boolean
		Dim prob, w, cprob As Short
		Dim dmg As Integer
		Dim def_mode As String
		Dim name_list() As String
		'ADD START 240a
		Dim lineStart, color, lineEnd As Integer
		Dim isNoSp As Boolean
		isNoSp = False
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		If IsStatusWindowDisabled Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		Exit Sub
		'End If
		
		DisplayedUnit = u
		DisplayedPilotInd = pindex
		
		'MOD START MARGE
		'    If MainWidth = 15 Then
		If Not NewGUIMode Then
			'MOD  END  MARGE
			'UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ppic = MainForm.picPilotStatus
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			upic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: PictureBox メソッド ppic.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ppic.Cls()
			'UPGRADE_ISSUE: PictureBox メソッド upic.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Cls()
		Else
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ppic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			upic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: PictureBox メソッド upic.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Cls()
			'ADD START 240a
			'Invalid_string_refer_to_original_code
			GlobalVariableLoad()
			'Invalid_string_refer_to_original_code
			upic.SetBounds(VB6.TwipsToPixelsX(MainPWidth - 240), VB6.TwipsToPixelsY(10), VB6.TwipsToPixelsX(235), VB6.TwipsToPixelsY(MainPHeight - 20))
			upic.BackColor = System.Drawing.ColorTranslator.FromOle(StatusWindowBackBolor)
			'UPGRADE_ISSUE: PictureBox プロパティ upic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.DrawWidth = StatusWindowFrameWidth
			color = StatusWindowFrameColor
			lineStart = (StatusWindowFrameWidth - 1) / 2
			lineEnd = (StatusWindowFrameWidth + 1) / 2
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ upic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.FillStyle = vbFSTransparent
			'譫�邱壹ｒ蠑輔￥
			'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Line (lineStart, lineStart) - (235 - lineEnd, MainPHeight - 20 - lineEnd), color, B
			'UPGRADE_ISSUE: PictureBox プロパティ upic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.FillStyle = ObjFillStyle
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentY = 5
			'Invalid_string_refer_to_original_code
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
			'ADD  END
		End If
		
		Dim td As TerrainData
		With u
			'Invalid_string_refer_to_original_code
			.Update()
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Or .IsConditionSatisfied("繝ｦ繝九ャ繝域ュ蝣ｱ髫�阡ｽ") _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_unknown = True
			'End If
			
			'Invalid_string_refer_to_original_code
			If .CountPilot = 0 Then
				'繧ｭ繝｣繝ｩ逕ｻ髱｢繧偵け繝ｪ繧｢
				If MainWidth = 15 Then
					'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picFace = System.Drawing.Image.FromFile("")
				Else
					DrawPicture("white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code")
				End If
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print(Term("繝ｬ繝吶Ν", u))
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print(Term("豌怜鴨", u))
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("蜻ｽ荳ｭ", u, 4) & "               " & Term("蝗樣∩", u))
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				
				GoTo UnitStatus
			End If
			
			'Invalid_string_refer_to_original_code
			If pindex = 0 Then
				'Invalid_string_refer_to_original_code
				p = .MainPilot
				If .MainPilot.Nickname = .Pilot(1).Nickname Or .Data.PilotNum = 1 Then
					DisplayedPilotInd = 1
				End If
			ElseIf pindex = 1 Then 
				'Invalid_string_refer_to_original_code
				If .MainPilot.Nickname <> .Pilot(1).Nickname And .Data.PilotNum <> 1 Then
					p = .Pilot(1)
				Else
					p = .MainPilot
				End If
			ElseIf pindex <= .CountPilot Then 
				'Invalid_string_refer_to_original_code
				p = .Pilot(pindex)
			ElseIf pindex <= .CountPilot + .CountSupport Then 
				'Invalid_string_refer_to_original_code
				p = .Support(pindex - .CountPilot)
			Else
				'Invalid_string_refer_to_original_code
				p = .AdditionalSupport
			End If
			
			With p
				'Invalid_string_refer_to_original_code
				.UpdateSupportMod()
				
				'Invalid_string_refer_to_original_code
				fname = "\Bitmap\Pilot\" & .Bitmap
				If frmMultiSelectListBox.Visible Then
					'Invalid_string_refer_to_original_code
					'莉｣繧上ｊ縺ｫ繝ｦ繝九ャ繝育判蜒上ｒ陦ｨ遉ｺ
					If InStr(.Name, "(繧ｶ繧ｳ)") > 0 Or InStr(.Name, "(豎守畑)") > 0 Then
						fname = "\Bitmap\Unit\" & u.Bitmap
					End If
				End If
				
				'逕ｻ蜒上ヵ繧｡繧､繝ｫ繧呈､懃ｴ｢
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
					'逕ｻ蜒上′隕九▽縺九ｉ縺ｪ縺九▲縺溘％縺ｨ繧定ｨ倬鹸
					If InStr(fname, "\Pilot\") > 0 Then
						If .Bitmap = .Data.Bitmap Then
							.Data.IsBitmapMissing = True
						End If
					End If
					fname = ""
				End If
				
				'逕ｻ蜒上ヵ繧｡繧､繝ｫ繧定ｪｭ縺ｿ霎ｼ繧薙〒陦ｨ遉ｺ
				If MainWidth = 15 Then
					If fname <> "" Then
						On Error GoTo ErrorHandler
						'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.picTmp = System.Drawing.Image.FromFile(fname)
						On Error GoTo 0
						'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.picFace.PaintPicture(MainForm.picTmp.Picture, 0, 0, 64, 64)
					Else
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.picFace = System.Drawing.Image.FromFile("")
					End If
				Else
					If fname <> "" Then
						DrawPicture(fname, 2, 2, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code")
					Else
						'Invalid_string_refer_to_original_code
						DrawPicture("white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code")
					End If
				End If
				
				'Invalid_string_refer_to_original_code
				ppic.Font = VB6.FontChangeSize(ppic.Font, 10.5)
				ppic.Font = VB6.FontChangeBold(ppic.Font, False)
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print(.Nickname)
				ppic.Font = VB6.FontChangeBold(ppic.Font, False)
				ppic.Font = VB6.FontChangeSize(ppic.Font, 10)
				
				'Invalid_string_refer_to_original_code
				If .Nickname0 = "Invalid_string_refer_to_original_code" Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 150)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(Term("繝ｬ繝吶Ν", u))
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(Term("豌怜鴨", u))
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(Term("蜻ｽ荳ｭ", u, 4) & "               " & Term("蝗樣∩", u))
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print()
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print()
					'MOD START 240a
					'               upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					
					GoTo UnitStatus
				End If
				'繝ｬ繝吶Ν縲∫ｵ碁ｨ灘�､縲∬｡悟虚蝗樊焚
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print(Term("繝ｬ繝吶Ν", u) & " ")
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If .Party = "蜻ｳ譁ｹ" Then
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(VB6.Format(.Level) & " (" & .Exp & ")")
					Select Case u.Action
						Case 2
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ppic.Print("Invalid_string_refer_to_original_code")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						Case 3
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ppic.Print("Invalid_string_refer_to_original_code")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
					End Select
				Else
					If Not is_unknown Then
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print(VB6.Format(.Level))
						If u.Action = 2 Then
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ppic.Print("Invalid_string_refer_to_original_code")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						End If
					Else
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print("Invalid_string_refer_to_original_code")
					End If
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print()
				
				'豌怜鴨
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				If MainWidth <> 15 Then
					'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ppic.Print(Term("豌怜鴨", u) & " ")
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If Not is_unknown Then
					If .MoraleMod > 0 Then
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print(VB6.Format(.Morale) & "+" & VB6.Format(.MoraleMod) & " (" & .Personality & ")")
					Else
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print(VB6.Format(.Morale) & " (" & .Personality & ")")
					End If
				Else
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				If .MaxSP > 0 Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 150)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					If MainWidth <> 15 Then
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(Term("Invalid_string_refer_to_original_code", u) & " ")
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If Not is_unknown Then
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP))
					Else
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print("Invalid_string_refer_to_original_code")
					End If
				Else
					isNoSp = True
				End If
				
				'菴ｿ逕ｨ荳ｭ縺ｮ繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ荳�隕ｧ
				If Not is_unknown Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'MOD START 240a
					'                If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox プロパティ ppic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(u.SpecialPowerInEffect)
					'ADD START 240a
				Else
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ppic.Print(" ")
					End If
					'ADD  END  240a
				End If
				'ADD START 240a
				If isNoSp Then
					'UPGRADE_ISSUE: PictureBox メソッド ppic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ppic.Print(" ")
				End If
				
				'Invalid_string_refer_to_original_code
				upic.Font = VB6.FontChangeBold(upic.Font, False)
				upic.Font = VB6.FontChangeSize(upic.Font, 9)
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Infight > 1 Then 
					Select Case .InfightMod + .InfightMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.InfightBase), 5) & RightPaddedString("+" & VB6.Format(.InfightMod + .InfightMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.InfightBase), 5) & RightPaddedString(VB6.Format(.InfightMod + .InfightMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Infight), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				If Not .HasMana() Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(Term("Invalid_string_refer_to_original_code", u, 4) & " ")
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(Term("鬲泌鴨", u, 4) & " ")
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Shooting > 1 Then 
					Select Case .ShootingMod + .ShootingMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.ShootingBase), 5) & RightPaddedString("+" & VB6.Format(.ShootingMod + .ShootingMod2), 5))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.ShootingBase), 5) & RightPaddedString(VB6.Format(.ShootingMod + .ShootingMod2), 5))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Shooting), 5) & Space(5))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(5))
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'蜻ｽ荳ｭ
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("蜻ｽ荳ｭ", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Hit > 1 Then 
					Select Case .HitMod + .HitMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.HitBase), 5) & RightPaddedString("+" & VB6.Format(.HitMod + .HitMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.HitBase), 5) & RightPaddedString(VB6.Format(.HitMod + .HitMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Hit), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'蝗樣∩
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("蝗樣∩", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Dodge > 1 Then 
					Select Case .DodgeMod + .DodgeMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.DodgeBase), 5) & RightPaddedString("+" & VB6.Format(.DodgeMod + .DodgeMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.DodgeBase), 5) & RightPaddedString(VB6.Format(.DodgeMod + .DodgeMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Dodge), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Technique > 1 Then 
					Select Case .TechniqueMod + .TechniqueMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.TechniqueBase), 5) & RightPaddedString("+" & VB6.Format(.TechniqueMod + .TechniqueMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.TechniqueBase), 5) & RightPaddedString(VB6.Format(.TechniqueMod + .TechniqueMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Technique), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ElseIf .Data.Intuition > 1 Then 
					Select Case .IntuitionMod + .IntuitionMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.IntuitionBase), 5) & RightPaddedString("+" & VB6.Format(.IntuitionMod + .IntuitionMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.IntuitionBase), 5) & RightPaddedString(VB6.Format(.IntuitionMod + .IntuitionMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(LeftPaddedString(VB6.Format(.Intuition), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.CurrentX = 5
					End If
					'髦ｲ蠕｡
					'MOD START 240a
					'               upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(Term("髦ｲ蠕｡", u) & " ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If is_unknown Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					ElseIf Not .IsSupport(u) Then 
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(LeftPaddedString(VB6.Format(.Defense), 5))
					Else
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(LeftPaddedString("--", 5))
					End If
				End If
			End With
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'謇�譛峨☆繧九せ繝壹す繝｣繝ｫ繝代Ρ繝ｼ荳�隕ｧ
			With p
				If .CountSpecialPower > 0 Then
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ", u, 18) & " ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If Not is_unknown Then
						For i = 1 To .CountSpecialPower
							If .SP < .SpecialPowerCost(.SpecialPower(i)) Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
								
							End If
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(SPDList.Item(.SpecialPower(i)).ShortName)
							'MOD START 240a
							'                        upic.ForeColor = rgb(0, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						Next 
					Else
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print("Invalid_string_refer_to_original_code")
					End If
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print()
				End If
			End With
			
			'Invalid_string_refer_to_original_code
			If is_unknown Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentY = upic.CurrentY + 8
				GoTo UnitStatus
			End If
			
			'Invalid_string_refer_to_original_code
			With p
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'髴雁鴨
				If .MaxPlana > 0 Then
					If .IsSkillAvailable("髴雁鴨") Then
						sname = .SkillName("髴雁鴨")
					Else
						'Invalid_string_refer_to_original_code
						sname = u.Pilot(1).SkillName("髴雁鴨")
					End If
					If InStr(sname, "髱櫁｡ｨ遉ｺ") = 0 Then
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 150)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
						'MOD  END  240a
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(sname & " ")
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
						If u.PlanaLevel() < .Plana Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(VB6.Format(.Plana) & "/" & VB6.Format(.MaxPlana))
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
					End If
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				If .SynchroRate() > 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'MOD START 240a
					'                    upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'MOD START 240a
					'                    upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If u.SyncLevel() < .SynchroRate Then
						'MOD START 240a
						'                        upic.ForeColor = rgb(150, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
						'MOD  END  240a
					End If
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(VB6.Format(.SynchroRate) & "%")
					'MOD START 240a
					'                    upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
				End If
				'End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				n = 0
				If .IsSkillAvailable("蠕玲э謚�") Then
					n = n + 1
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("蠕玲э謚� ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(RightPaddedString(.SkillData("蠕玲э謚�"), 12))
				End If
				If .IsSkillAvailable("荳榊ｾ玲焔") Then
					n = n + 1
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("荳榊ｾ玲焔 ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(.SkillData("荳榊ｾ玲焔"))
				End If
				If n > 0 Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print()
				End If
				
				'Invalid_string_refer_to_original_code
				ReDim name_list(.CountSkill)
				For i = 1 To .CountSkill
					name_list(i) = .Skill(i)
				Next 
				'Invalid_string_refer_to_original_code
				For i = 1 To u.CountCondition
					If u.ConditionLifetime(i) <> 0 Then
						Select Case Right(u.Condition(i), 3)
							Case "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								Select Case LIndex(u.ConditionData(i), 1)
									Case "髱櫁｡ｨ遉ｺ", "隗｣隱ｬ"
										'Invalid_string_refer_to_original_code
									Case Else
										stype = Left(u.Condition(i), Len(u.Condition(i)) - 3)
										Select Case stype
											Case "繝上Φ繧ｿ繝ｼ", "Invalid_string_refer_to_original_code"
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
												'Invalid_string_refer_to_original_code
												ReDim Preserve name_list(UBound(name_list) + 1)
												name_list(UBound(name_list)) = stype
											Case Else
												'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				n = 0
				For i = 1 To UBound(name_list)
					'ADD START 240a
					'Invalid_string_refer_to_original_code
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'ADD  END  240a
					stype = name_list(i)
					If i <= .CountSkill Then
						sname = .SkillName(i)
						slevel = CStr(.SkillLevel(i))
					Else
						sname = .SkillName(stype)
						slevel = CStr(.SkillLevel(stype))
					End If
					
					If InStr(sname, "髱櫁｡ｨ遉ｺ") > 0 Then
						GoTo NextSkill
					End If
					
					Select Case stype
						Case "繧ｪ繝ｼ繝ｩ"
							If DisplayedPilotInd = 1 Then
								If u.AuraLevel(True) < u.AuraLevel() And MapFileName <> "" Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								If u.AuraLevel(True) > CDbl(slevel) Then
									sname = sname & "+" & VB6.Format(u.AuraLevel(True) - CDbl(slevel))
								End If
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If DisplayedPilotInd = 1 Then
								If u.PsychicLevel(True) < u.PsychicLevel() And MapFileName <> "" Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								If u.PsychicLevel(True) > CDbl(slevel) Then
									sname = sname & "+" & VB6.Format(u.PsychicLevel(True) - CDbl(slevel))
								End If
							End If
							
						Case "蠎募鴨", "Invalid_string_refer_to_original_code", "隕壽ぁ"
							If u.HP <= u.MaxHP \ 4 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If u.HP <= u.MaxHP \ 2 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "貎懷惠蜉幃幕謾ｾ"
							If .Morale >= 130 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If i <= .CountSkill Then
								If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) Then
									'MOD START 240a
									'                            upic.ForeColor = vbBlue
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
									'MOD  END  240a
								End If
							Else
								If .Morale >= StrToLng(LIndex(.SkillData(stype), 3)) Then
									'MOD START 240a
									'                            upic.ForeColor = vbBlue
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
									'MOD  END  240a
								End If
							End If
							
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'And Not u.IsFeatureAvailable("逶ｾ") _
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'And MapFileName <> "" _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'MOD START 240a
							'                            upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
							'End If
							
						Case "Invalid_string_refer_to_original_code"
							For j = 1 To u.CountWeapon
								If u.IsWeaponClassifiedAs(j, "豁ｦ") Then
									If Not u.IsDisabled((u.Weapon(j).Name)) Then
										Exit For
									End If
								End If
							Next 
							If u.IsFeatureAvailable("譬ｼ髣俶ｭｦ蝎ｨ") Then
								j = 0
							End If
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'And MapFileName <> "" _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'MOD START 240a
							'                            upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
							'End If
							
						Case "霑取茶"
							For j = 1 To u.CountWeapon
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'And (u.Weapon(j).Bullet >= 10 _
								'Or (u.Weapon(j).Bullet = 0 _
								'And u.Weapon(j).ENConsumption <= 5)) _
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								Exit For
								'End If
							Next 
							If u.IsFeatureAvailable("霑取茶豁ｦ蝎ｨ") Then
								j = 0
							End If
							'Invalid_string_refer_to_original_code_
							'And InStr(u.FeatureData("蠖薙※霄ｫ謚�"), "霑取茶") = 0 _
							'Invalid_string_refer_to_original_code_
							'And MapFileName <> "" _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'MOD START 240a
							'                            upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
							'End If
							
						Case "Invalid_string_refer_to_original_code"
							For j = 1 To u.CountWeapon
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								If Not u.IsDisabled((u.Weapon(j).Name)) Then
									Exit For
								End If
								'End If
							Next 
							If j > u.CountWeapon And MapFileName <> "" Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
							
						Case "謠ｴ隴ｷ"
							If MapFileName <> "" Then
								If u.Party = Stage Then
									ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
								Else
									If u.IsUnderSpecialPowerEffect("Invalid_string_refer_to_original_code") Then
										'MOD START 240a
										'                                    upic.ForeColor = rgb(150, 0, 0)
										upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
										'MOD  END  240a
									End If
									ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
								End If
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (谿九ｊ" & VB6.Format(ret) & "Invalid_string_refer_to_original_code"
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (谿九ｊ" & VB6.Format(ret) & "Invalid_string_refer_to_original_code"
							End If
							
						Case "謠ｴ隴ｷ髦ｲ蠕｡"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
								If ret = 0 Or u.IsUnderSpecialPowerEffect("Invalid_string_refer_to_original_code") Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (谿九ｊ" & VB6.Format(ret) & "Invalid_string_refer_to_original_code"
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSyncAttack - u.UsedSyncAttack, 0)
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (谿九ｊ" & VB6.Format(ret) & "Invalid_string_refer_to_original_code"
							End If
							
						Case "繧ｫ繧ｦ繝ｳ繧ｿ繝ｼ"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxCounterAttack - u.UsedCounterAttack, 0)
								If ret > 100 Then
									sname = sname & " (谿九ｊ竏槫屓)"
								ElseIf ret > 0 Then 
									sname = sname & " (谿九ｊ" & VB6.Format(ret) & "Invalid_string_refer_to_original_code"
								Else
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
									sname = sname & "Invalid_string_refer_to_original_code"
								End If
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If u.MaxCounterAttack > 100 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "Invalid_string_refer_to_original_code"
							If IsOptionDefined("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
								GoTo NextSkill
							End If
							
						Case "髴雁鴨", "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							GoTo NextSkill
							
					End Select
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					If LenB(StrConv(sname, vbFromUnicode)) > 19 Then
						If n > 0 Then
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print()
							'ADD START 240a
							If NewGUIMode Then
								'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								upic.CurrentX = 5
							End If
							'ADD  END  240a
						End If
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(sname)
						n = 2
					Else
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print(RightPaddedString(sname, 19))
						n = n + 1
					End If
					upic.ForeColor = System.Drawing.Color.Black
					
					'Invalid_string_refer_to_original_code
					If n > 1 Then
						'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.CurrentX = 5
						End If
						'ADD  END  240a
						n = 0
					End If
NextSkill: 
				Next 
			End With
			
			If n > 0 Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
			End If
			
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentY = upic.CurrentY + 8
			
UnitStatus: 
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			GoTo UpdateStatusWindow
			'End If
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			upic.Font = VB6.FontChangeSize(upic.Font, 10.5)
			upic.Font = VB6.FontChangeBold(upic.Font, False)
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
				'Invalid_string_refer_to_original_code
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
			End If
			'ADD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(.Nickname0)
			upic.Font = VB6.FontChangeBold(upic.Font, False)
			upic.Font = VB6.FontChangeSize(upic.Font, 9)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			
			'Invalid_string_refer_to_original_code
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'Invalid_string_refer_to_original_code
			If InStr(TerrainName(.X, .Y), "(") > 0 Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(.Area & " (" & Left(TerrainName(.X, .Y), InStr(TerrainName(.X, .Y), "(") - 1))
			Else
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(.Area & " (" & TerrainName(.X, .Y))
			End If
			
			'Invalid_string_refer_to_original_code
			If TerrainEffectForHit(.X, .Y) = TerrainEffectForDamage(.X, .Y) Then
				If TerrainEffectForHit(.X, .Y) >= 0 Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("Invalid_string_refer_to_original_code" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("Invalid_string_refer_to_original_code" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
				End If
			Else
				If TerrainEffectForHit(.X, .Y) >= 0 Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("Invalid_string_refer_to_original_code" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print("Invalid_string_refer_to_original_code")
					& Format$(TerrainEffectForHit(.X, .Y)) & "%";
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				End If
				If TerrainEffectForDamage(.X, .Y) >= 0 Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(" 髦ｲ+" & VB6.Format(TerrainEffectForDamage(.X, .Y)) & "%")
				Else
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(" 髦ｲ" & VB6.Format(TerrainEffectForDamage(.X, .Y)) & "%")
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If TerrainEffectForHPRecover(.X, .Y) > 0 Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" " & Left(Term("Invalid_string_refer_to_original_code"), 1) & "+" & VB6.Format(TerrainEffectForHPRecover(.X, .Y)) & "%")
			End If
			If TerrainEffectForENRecover(.X, .Y) > 0 Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" " & Left(Term("Invalid_string_refer_to_original_code"), 1) & "+" & VB6.Format(TerrainEffectForENRecover(.X, .Y)) & "%")
			End If
			
			'MOD START 240a
			'            Set td = TDList.Item(MapData(.X, .Y, 0))
			'Invalid_string_refer_to_original_code
			Select Case MapData(.X, .Y, Map.MapDataIndex.BoxType)
				Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
					td = TDList.Item(MapData(.X, .Y, Map.MapDataIndex.TerrainType))
				Case Else
					td = TDList.Item(MapData(.X, .Y, Map.MapDataIndex.LayerType))
			End Select
			'MOD START 240a
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			
			'Invalid_string_refer_to_original_code
			If td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" " & Left(Term("Invalid_string_refer_to_original_code"), 1) & "+" & VB6.Format(1000 * td.FeatureLevel("Invalid_string_refer_to_original_code")))
			End If
			If td.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" " & Left(Term("Invalid_string_refer_to_original_code"), 1) & "+" & VB6.Format(10 * td.FeatureLevel("Invalid_string_refer_to_original_code")))
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			
			'鞫ｩ謫ｦ
			If td.IsFeatureAvailable("鞫ｩ謫ｦ") Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" 鞫ｩL" & VB6.Format(td.FeatureLevel("鞫ｩ謫ｦ")))
			End If
			
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(")")
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print("繝ｩ繝ｳ繧ｯ ")
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(VB6.Format(.Rank))
			'End If
			
			'Invalid_string_refer_to_original_code
			If is_unknown Then
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("Invalid_string_refer_to_original_code", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("?????/?????")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("Invalid_string_refer_to_original_code", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("???/???")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("Invalid_string_refer_to_original_code", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				'驕句虚諤ｧ
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("驕句虚諤ｧ", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				'遘ｻ蜍募鴨
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("遘ｻ蜍募鴨", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				'繝ｦ繝九ャ繝医し繧､繧ｺ
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(Term("繧ｵ繧､繧ｺ", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'And Not SelectedUnit Is Nothing _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code_
				'Or .IsConditionSatisfied("證ｴ襍ｰ") _
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
				
				'Invalid_string_refer_to_original_code
				'MOD START 240a
				'                    upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				'MOD START 240a
				'                   upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(SelectedUnit.WeaponNickname(SelectedWeapon))
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
					'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.Print(" [謠ｴ]")
				End If
			End If
			'End If
			'End If
			
			GoTo UpdateStatusWindow
			'End If
			
			'螳溯｡御ｸｭ縺ｮ蜻ｽ莉､
			'Invalid_string_refer_to_original_code_
			'And Not .IsConditionSatisfied("證ｴ襍ｰ") _
			'And Not .IsConditionSatisfied("迢よ姶螢ｫ") _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			buf = ""
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If Not .Master Is Nothing Then
				If .Master.Party = "蜻ｳ譁ｹ" Then
					buf = .Mode
				End If
			End If
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If Not .Summoner Is Nothing Then
				If .Summoner.Party = "蜻ｳ譁ｹ" Then
					buf = .Mode
				End If
			End If
			'End If
			
			If buf = "騾壼ｸｸ" Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("閾ｪ逕ｱ陦悟虚荳ｭ")
			ElseIf PList.IsDefined(buf) Then 
				'Invalid_string_refer_to_original_code
				With PList.Item(buf)
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							upic.Print(.Nickname & "(" & VB6.Format(.X) & "," & VB6.Format(.Y) & "Invalid_string_refer_to_original_code")
							If .Party = "蜻ｳ譁ｹ" Then
								'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								upic.Print("隴ｷ陦帑ｸｭ")
							Else
								'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								upic.Print("霑ｽ霍｡荳ｭ")
							End If
						End With
					End If
				End With
			End If
		End With
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print("(" & LIndex(buf, 1) & "," & LIndex(buf, 2) & ")縺ｫ遘ｻ蜍穂ｸｭ")
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code
		ReDim name_list(0)
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		cx = upic.CurrentX
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		cy = upic.CurrentY
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (116, cy + 2) - (118 + GauageWidth, cy + 2), RGB(100, 100, 100)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (116, cy + 2) - (116, cy + 9), RGB(100, 100, 100)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (117, cy + 8) - (118 + GauageWidth, cy + 8), RGB(220, 220, 220)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (118 + GauageWidth, cy + 3) - (118 + GauageWidth, cy + 9), RGB(220, 220, 220)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (117, cy + 3) - (117 + GauageWidth, cy + 7), RGB(200, 0, 0), BF
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.CurrentX = cx
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.CurrentY = cy
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("Invalid_string_refer_to_original_code", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		cx = upic.CurrentX
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		cy = upic.CurrentY
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (116, cy + 2) - (118 + GauageWidth, cy + 2), RGB(100, 100, 100)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (116, cy + 2) - (116, cy + 9), RGB(100, 100, 100)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (117, cy + 8) - (118 + GauageWidth, cy + 8), RGB(220, 220, 220)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (118 + GauageWidth, cy + 3) - (118 + GauageWidth, cy + 9), RGB(220, 220, 220)
		'UPGRADE_ISSUE: PictureBox メソッド upic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Line (117, cy + 3) - (117 + GauageWidth, cy + 7), RGB(200, 0, 0), BF
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.CurrentX = cx
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.CurrentY = cy
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("Invalid_string_refer_to_original_code", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("Invalid_string_refer_to_original_code", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'驕句虚諤ｧ
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("驕句虚諤ｧ", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'遘ｻ蜍募鴨
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("遘ｻ蜍募鴨", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print("Invalid_string_refer_to_original_code")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		For i = 1 To 4
			'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		Next 
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Space(8))
		
		'繝ｦ繝九ャ繝医し繧､繧ｺ
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Term("繧ｵ繧､繧ｺ", u, 6) & " ")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'髦ｲ蠕｡螻樊�ｧ縺ｮ陦ｨ遉ｺ
		n = 0
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'閠先�ｧ
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'蠑ｱ轤ｹ
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'譛牙柑
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		If n > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print()
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
		End If
		n = 0
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ
		ReDim flist(0)
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(RightPaddedString(fname, 19))
		n = n + 1
		If n > 1 Then
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print()
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			n = 0
		End If
		'End Select
		'End If
		GoTo NextFeature
		'End If
		
		'Invalid_string_refer_to_original_code
		For j = 1 To UBound(flist)
			If fname = flist(j) Then
				GoTo NextFeature
			End If
		Next 
		ReDim Preserve flist(UBound(flist) + 1)
		flist(UBound(flist)) = fname
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
		If LenB(StrConv(fname, vbFromUnicode)) > 19 Then
			If n > 0 Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					upic.CurrentX = 5
				End If
				'ADD  END  240a
			End If
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(fname)
			n = 2
		Else
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(RightPaddedString(fname, 19))
			n = n + 1
		End If
		
		'Invalid_string_refer_to_original_code
		If n > 1 Then
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print()
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			n = 0
		End If
		
		'陦ｨ遉ｺ濶ｲ繧呈綾縺励※縺翫￥
		'MOD START 240a
		'            upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
NextFeature: 
		'Next
		If n > 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print()
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'And Not SelectedUnit Is Nothing _
		'And SelectedWeapon > 0 _
		'And Stage <> "繝励Ο繝ｭ繝ｼ繧ｰ" And Stage <> "繧ｨ繝斐Ο繝ｼ繧ｰ" _
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		GoTo SkipAttackExpResult
		'End If
		
		'逶ｸ謇九′謨ｵ縺ｮ蝣ｴ蜷医↓縺ｮ縺ｿ陦ｨ遉ｺ
		'Invalid_string_refer_to_original_code_
		'And Not .IsConditionSatisfied("證ｴ襍ｰ") _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'And Not .IsConditionSatisfied("豺ｷ荵ｱ") _
		'And Not .IsConditionSatisfied("逹｡逵�") _
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		GoTo SkipAttackExpResult
		'End If
		
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print()
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print("Invalid_string_refer_to_original_code")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(SelectedUnit.WeaponNickname(SelectedWeapon))
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'And UseSupportAttack _
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(" [謠ｴ]")
		Else
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print()
		End If
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print()
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		w = 0
		w = SelectWeapon(u, SelectedUnit, "蜿肴茶")
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		def_mode = SelectDefense(SelectedUnit, SelectedWeapon, u, w)
		If def_mode <> "" Then
			w = 0
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'莠域ｸｬ繝�繝｡繝ｼ繧ｸ
		If Not IsOptionDefined("莠域ｸｬ繝�繝｡繝ｼ繧ｸ髱櫁｡ｨ遉ｺ") Then
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print("繝�繝｡繝ｼ繧ｸ ")
			dmg = SelectedUnit.Damage(SelectedWeapon, u, True)
			If def_mode = "髦ｲ蠕｡" Then
				dmg = dmg \ 2
			End If
			'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(VB6.Format(dmg))
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print("Invalid_string_refer_to_original_code")
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			prob = SelectedUnit.HitProbability(SelectedWeapon, u, True)
			If def_mode = "蝗樣∩" Then
				prob = prob \ 2
			End If
			cprob = SelectedUnit.CriticalProbability(SelectedWeapon, u, def_mode)
			'UPGRADE_WARNING: オブジェクト Invalid_string_refer_to_original_code の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print(MinLng(prob, 100) & "Invalid_string_refer_to_original_code" & Invalid_string_refer_to_original_code)
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		If w > 0 Then
			'蜿肴茶謇区ｮｵ
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.Print("蜿肴茶     ")
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
			'Invalid_string_refer_to_original_code
			If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" [謠ｴ]")
			Else
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'莠域ｸｬ繝�繝｡繝ｼ繧ｸ
			If Not IsOptionDefined("莠域ｸｬ繝�繝｡繝ｼ繧ｸ髱櫁｡ｨ遉ｺ") Then
				'MOD START 240a
				'                upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("繝�繝｡繝ｼ繧ｸ ")
				'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
				If dmg >= SelectedUnit.HP Then
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(190, 0, 0))
				Else
					'MOD START 240a
					'                    upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
				End If
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(VB6.Format(dmg))
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'Invalid_string_refer_to_original_code
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				'MOD START 240a
				'                upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
				'MOD START 240a
				'                upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: オブジェクト Invalid_string_refer_to_original_code の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(VB6.Format(MinLng(prob, 100)) & "Invalid_string_refer_to_original_code" & Invalid_string_refer_to_original_code)
			End If
		Else
			'Invalid_string_refer_to_original_code
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			If def_mode <> "" Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(def_mode)
			Else
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print("Invalid_string_refer_to_original_code")
			End If
			'MOD START 240a
			'            upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'Invalid_string_refer_to_original_code
			If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print(" [謠ｴ]")
			Else
				'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				upic.Print()
			End If
		End If
		
SkipAttackExpResult: 
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			upic.CurrentX = 5
		End If
		'ADD  END  240a
		'豁ｦ蝎ｨ荳�隕ｧ
		'UPGRADE_ISSUE: PictureBox プロパティ upic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.CurrentY = upic.CurrentY + 8
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print(Space(25))
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 150)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
		'MOD  END  240a
		'UPGRADE_ISSUE: PictureBox メソッド upic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		upic.Print("Invalid_string_refer_to_original_code")
		'MOD START 240a
		'        upic.ForeColor = rgb(0, 0, 0)
		upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
		'MOD  END  240a
		
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: DisplayUnitStatus に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		
UpdateStatusWindow: 
		
		'MOD START 240a
		'    If MainWidth = 15 Then
		If Not NewGUIMode Then
			'MOD  END
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picFace.Refresh()
			ppic.Refresh()
			upic.Refresh()
		Else
			If MouseX < MainPWidth \ 2 Then
				'MOD START 240a
				'            upic.Move MainPWidth - 230 - 5, 10
				'Invalid_string_refer_to_original_code
				upic.SetBounds(VB6.TwipsToPixelsX(MainPWidth - 240), VB6.TwipsToPixelsY(10), 0, 0, Windows.Forms.BoundsSpecified.X Or Windows.Forms.BoundsSpecified.Y)
				'MOD  END
			Else
				upic.SetBounds(VB6.TwipsToPixelsX(5), VB6.TwipsToPixelsY(10), 0, 0, Windows.Forms.BoundsSpecified.X Or Windows.Forms.BoundsSpecified.Y)
			End If
			If upic.Visible Then
				upic.Refresh()
			Else
				upic.Visible = True
			End If
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("Invalid_string_refer_to_original_code" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DisplayPilotStatus(ByVal p As Pilot)
		Dim i As Short
		
		DisplayedUnit = p.Unit_Renamed
		
		With DisplayedUnit
			If p Is .MainPilot Then
				'Invalid_string_refer_to_original_code
				DisplayUnitStatus(DisplayedUnit, 0)
			Else
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountPilot
					If p Is .Pilot(i) Then
						DisplayUnitStatus(DisplayedUnit, i)
						Exit Sub
					End If
				Next 
				
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountSupport
					If p Is .Support(i) Then
						DisplayUnitStatus(DisplayedUnit, i + .CountPilot)
						Exit Sub
					End If
				Next 
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				DisplayUnitStatus(DisplayedUnit, .CountPilot + .CountSupport + 1)
			End If
			'End If
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub InstantUnitStatusDisplay(ByVal X As Short, ByVal Y As Short)
		Dim u As Unit
		
		'Invalid_string_refer_to_original_code
		u = MapDataForUnit(X, Y)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If u Is SelectedUnit Then
			u = SelectedTarget
			If u Is Nothing Then
				Exit Sub
			End If
		End If
		'End If
		
		If DisplayedUnit Is Nothing Then
			'Invalid_string_refer_to_original_code
		Else
			'Invalid_string_refer_to_original_code
			If u Is DisplayedUnit Then
				Exit Sub
			End If
		End If
		
		DisplayUnitStatus(u)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ClearUnitStatus()
		If MainWidth = 15 Then
			'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picFace = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picPilotStatus.Cls()
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picUnitStatus.Cls()
			'UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DisplayedUnit = Nothing
		Else
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picUnitStatus.Visible = False
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picUnitStatus.Cls()
			IsStatusWindowDisabled = True
			System.Windows.Forms.Application.DoEvents()
			IsStatusWindowDisabled = False
			'ADD
			'UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			DisplayedUnit = Nothing
		End If
	End Sub
	
	'ADD START 240a
	'Invalid_string_refer_to_original_code
	Private Function GetGlobalStatusSize(ByRef X As Short, ByRef Y As Short) As Integer
		Dim ret As Integer
		ret = 42
		If Not (X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y) Then
			'Invalid_string_refer_to_original_code
			ret = 106
			'Invalid_string_refer_to_original_code
			If TerrainEffectForHPRecover(X, Y) > 0 Or TerrainEffectForENRecover(X, Y) > 0 Then
				ret = ret + 16
			End If
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ret = ret + 16
		End If
		'Invalid_string_refer_to_original_code
		If TerrainHasFeature(X, Y, "Invalid_string_refer_to_original_code") Or TerrainHasFeature(X, Y, "Invalid_string_refer_to_original_code") Then
			ret = ret + 16
		End If
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		ret = ret + 16
		'End If
		'Invalid_string_refer_to_original_code
		If TerrainHasFeature(X, Y, "鞫ｩ謫ｦ") Or TerrainHasFeature(X, Y, "迥ｶ諷倶ｻ伜刈") Then
			ret = ret + 16
		End If
		'End If
		GetGlobalStatusSize = ret
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Sub GlobalVariableLoad()
		'閭梧勹濶ｲ
		If IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
			If Not StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)") Then
				StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)")
			End If
		End If
		'譫�縺ｮ濶ｲ
		If IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
			If Not StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)") Then
				StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)")
			End If
		End If
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined("StatusWindow(FrameWidth)") Then
			If Not StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)") Then
				StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)")
			End If
		End If
		'閭ｽ蜉帛錐縺ｮ濶ｲ
		If IsGlobalVariableDefined("StatusWindow(ANameColor)") Then
			If Not StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)") Then
				StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)")
			End If
		End If
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined("StatusWindow(EnableColor)") Then
			If Not StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)") Then
				StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)")
			End If
		End If
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined("StatusWindow(DisableColor)") Then
			If Not StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)") Then
				StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)")
			End If
		End If
		'Invalid_string_refer_to_original_code
		If IsGlobalVariableDefined("StatusWindow(StringColor)") Then
			If Not StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)") Then
				StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)")
			End If
		End If
	End Sub
	'ADD  END  240a
End Module