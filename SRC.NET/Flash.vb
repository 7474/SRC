Option Strict Off
Option Explicit On
Module Flash
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Flashãƒ•ã‚¡ã‚¤ãƒ«ã®å†ç”Ÿ
	Public Sub PlayFlash(ByRef fname As String, ByRef fx As Short, ByRef fy As Short, ByRef fw As Short, ByRef fh As Short, ByRef opt As String)
		Dim i As Short
		Dim is_VisibleEnd As Boolean
		
		'Invalid_string_refer_to_original_code
		If Not IsFlashAvailable Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
		End If
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Enable ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If Not frmMain.FlashObject.Enable Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
			'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Exit Sub
		End If
		
		is_VisibleEnd = False
		
		For i = 1 To LLength(opt)
			Select Case LIndex(opt, i)
				Case "ä¿æŒ"
					is_VisibleEnd = True
			End Select
		Next 
		
		With frmMain.FlashObject
			'ä¸€æ—¦éè¡¨ç¤º
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Visible ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Visible = False
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Left ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Left = fx
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Top ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Top = fy
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Width ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Width = fw
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Height ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Height = fh
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Visible ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Visible = True
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.ZOrder ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.ZOrder()
			
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.LoadMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.LoadMovie(ScenarioPath & fname)
			
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Playing ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Do While .Playing And Not IsRButtonPressed(True)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.StopMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			frmMain.FlashObject.StopMovie()
			
			If Not is_VisibleEnd Then
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.ClearMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.ClearMovie()
				'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Visible ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.Visible = False
			End If
		End With
	End Sub
	
	'è¡¨ç¤ºã—ãŸã¾ã¾ã®Flashã‚’æ¶ˆå»ã™ã‚‹
	Public Sub ClearFlash()
		If Not IsFlashAvailable Then Exit Sub
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Enable ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If Not frmMain.FlashObject.Enable Then Exit Sub
		
		With frmMain.FlashObject
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.ClearMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.ClearMovie()
			'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.Visible ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Visible = False
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	' Flashã®ã‚¢ã‚¯ã‚·ãƒ§ãƒ³ã®ã€ŒGetURLã€ã§
	'ã€€1.ã€ŒURLã€ã«"FSCommand:"
	'Invalid_string_refer_to_original_code
	'ã‚’æŒ‡å®šã™ã‚‹ã¨ã€ãã®ã‚¢ã‚¯ã‚·ãƒ§ãƒ³ãŒå®Ÿè¡Œã•ã‚ŒãŸã¨ãã«
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub GetEvent(ByVal fpara As String)
		Dim buf As String
		Dim i, j As Short
		Dim funcname, funcpara As String
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		
		'å†ç”Ÿã‚’ä¸€æ™‚åœæ­¢
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.StopMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		frmMain.FlashObject.StopMovie()
		
		funcname = ""
		funcpara = ""
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		If funcname = "" Then
			funcname = ListIndex(fpara, 1)
			buf = ListTail(fpara, 2)
		End If
		'ã‚µãƒ–ãƒ«ãƒ¼ãƒãƒ³ã®å¼•æ•°ã‚’è¨˜éŒ²
		For j = 1 To ListLength(buf)
			funcpara = funcpara & ", " & ListIndex(buf, j)
		Next 
		
		'Invalid_string_refer_to_original_code
		buf = "Call(" & funcname & funcpara & ")"
		'Invalid_string_refer_to_original_code
		CallFunction(buf, etype, str_result, num_result)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg frmMain.FlashObject.PlayMovie ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		frmMain.FlashObject.PlayMovie()
	End Sub
End Module