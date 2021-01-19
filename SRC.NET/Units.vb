Option Strict Off
Option Explicit On
Friend Class Units
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public IDCount As Integer
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆä¸€è¦§
	Private colUnits As New Collection
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize ‚Í Class_Initialize_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Private Sub Class_Initialize_Renamed()
		IDCount = 0
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'ã‚¯ãƒ©ã‚¹ã®è§£æ”¾
	'UPGRADE_NOTE: Class_Terminate ‚Í Class_Terminate_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg colUnits ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		colUnits = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEachç”¨é–¢æ•°
	'UPGRADE_NOTE: NewEnum ƒvƒƒpƒeƒB‚ªƒRƒƒ“ƒg ƒAƒEƒg‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colUnits.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: ƒRƒŒƒNƒVƒ‡ƒ“—ñ‹“q‚ğ•Ô‚·‚É‚ÍAƒRƒƒ“ƒg‚ğ‰ğœ‚µ‚ÄˆÈ‰º‚Ìs‚ğ•ÏX‚µ‚Ä‚­‚¾‚³‚¢B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'GetEnumerator = colUnits.GetEnumerator
	End Function
	
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆãƒªã‚¹ãƒˆã«æ–°ã—ã„ãƒ¦ãƒ‹ãƒƒãƒˆã‚’è¿½åŠ 
	Public Function Add(ByRef uname As String, ByVal urank As Short, ByRef uparty As String) As Unit
		Dim new_unit As Unit
		Dim new_form As Unit
		Dim ud As UnitData
		Dim uname2 As String
		Dim other_forms() As String
		Dim i, j As Short
		Dim list As String
		
		'Invalid_string_refer_to_original_code
		If Not UDList.IsDefined(uname) Then
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Add = Nothing
			Exit Function
		End If
		
		ud = UDList.Item(uname)
		
		new_unit = New Unit
		Add = new_unit
		With new_unit
			.Name = ud.Name
			.Rank = urank
			.Party = uparty
			.ID = CreateID((ud.Name))
			.FullRecover()
		End With
		colUnits.Add(new_unit, new_unit.ID)
		
		'Invalid_string_refer_to_original_code
		ReDim other_forms(0)
		
		'Invalid_string_refer_to_original_code
		list = ud.FeatureData("å¤‰å½¢")
		For i = 2 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		uname2 = LIndex(list, 2)
		If Not UDList.IsDefined(uname2) Then
			If uname = "" Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Else
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Add = Nothing
			Exit Function
		End If
		ReDim Preserve other_forms(UBound(other_forms) + 1)
		other_forms(UBound(other_forms)) = uname2
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		uname2 = LIndex(list, 1)
		If Not UDList.IsDefined(uname2) Then
			If uname2 = "" Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Else
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Add = Nothing
			Exit Function
		End If
		ReDim Preserve other_forms(UBound(other_forms) + 1)
		other_forms(UBound(other_forms)) = uname2
		'End If
		
		'Invalid_string_refer_to_original_code
		If ud.IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
			uname2 = LIndex(ud.FeatureData("Invalid_string_refer_to_original_code"), 2)
			If Not UDList.IsDefined(uname2) Then
				If uname2 = "" Then
					ErrorMessage("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Else
					ErrorMessage("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code_
					'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				End If
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		If Not UDList.IsDefined(uname2) Then
			If uname = "" Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Else
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			End If
			'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			Add = Nothing
			Exit Function
		End If
		ReDim Preserve other_forms(UBound(other_forms) + 1)
		other_forms(UBound(other_forms)) = uname2
		'End If
		
		'Invalid_string_refer_to_original_code
		With ud
			If .IsFeatureAvailable("å¤‰å½¢æŠ€") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "å¤‰å½¢æŠ€" Then
						uname2 = LIndex(.FeatureData(i), 2)
						If Not UDList.IsDefined(uname2) Then
							If uname2 = "" Then
								ErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							Else
								ErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code_
								'uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							End If
							'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							Add = Nothing
							Exit Function
						End If
						ReDim Preserve other_forms(UBound(other_forms) + 1)
						other_forms(UBound(other_forms)) = uname2
					End If
				Next 
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code_
				'& uname2 & "ã€ãŒè¦‹ã¤ã‹ã‚Šã¾ã›ã‚“"
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Add ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'å½¢æ…‹ã‚’è¿½åŠ 
		For i = 1 To UBound(other_forms)
			If Not new_unit.IsOtherFormDefined(other_forms(i)) Then
				new_form = New Unit
				With new_form
					.Name = other_forms(i)
					.Rank = urank
					.Party = uparty
					.ID = CreateID((ud.Name))
					.FullRecover()
					.Status_Renamed = "Invalid_string_refer_to_original_code"
				End With
				colUnits.Add(new_form, new_form.ID)
				new_unit.AddOtherForm(new_form)
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		For i = 1 To new_unit.CountOtherForm
			new_unit.OtherForm(i).AddOtherForm(new_unit)
			For j = 1 To new_unit.CountOtherForm
				If Not i = j Then
					new_unit.OtherForm(i).AddOtherForm(new_unit.OtherForm(j))
				End If
			Next 
		Next 
		
		'Invalid_string_refer_to_original_code
		With ud
			For i = 1 To .CountFeature
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
					new_unit.Status_Renamed = "Invalid_string_refer_to_original_code"
					Exit Function
				End If
				'End If
				If .Feature(i) = "Invalid_string_refer_to_original_code" Then
					For j = 2 To LLength(.FeatureData(i))
						If UList.IsDefined(LIndex(.FeatureData(i), j)) Then
							new_unit.Status_Renamed = "Invalid_string_refer_to_original_code"
							Exit Function
						End If
					Next 
				End If
			Next 
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Add2(ByRef u As Unit)
		colUnits.Add(u, u.ID)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function CreateID(ByRef uname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined(uname & ":" & VB6.Format(IDCount))
		CreateID = uname & ":" & VB6.Format(IDCount)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colUnits.Count()
	End Function
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆãƒªã‚¹ãƒˆã‹ã‚‰ãƒ¦ãƒ‹ãƒƒãƒˆã‚’å‰Šé™¤
	Public Sub Delete(ByRef Index As Object)
		colUnits.Remove(Index)
	End Sub
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆãƒªã‚¹ãƒˆã‹ã‚‰æŒ‡å®šã•ã‚ŒãŸãƒ¦ãƒ‹ãƒƒãƒˆã‚’è¿”ã™
	Public Function Item(ByRef Index As Object) As Unit
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		Item = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'IDã§è¦‹ã¤ã‹ã‚‰ãªã‘ã‚Œã°ãƒ¦ãƒ‹ãƒƒãƒˆåã§æ¤œç´¢
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Index ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					Item = u
					Exit Function
				End If
				'End If
			End With
		Next u
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Item ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Item = Nothing
	End Function
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆãƒªã‚¹ãƒˆã‹ã‚‰ãƒ¦ãƒ‹ãƒƒãƒˆã‚’æ¤œç´¢ (IDã®ã¿)
	Public Function Item2(ByRef Index As Object) As Unit
		On Error GoTo ErrorHandler
		Item2 = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: ƒIƒuƒWƒFƒNƒg Item2 ‚ğƒKƒx[ƒW ƒRƒŒƒNƒg‚·‚é‚Ü‚Å‚±‚ÌƒIƒuƒWƒFƒNƒg‚ğ”jŠü‚·‚é‚±‚Æ‚Í‚Å‚«‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Item2 = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'IDã§è¦‹ã¤ã‹ã‚‰ãªã‘ã‚Œã°ãƒ¦ãƒ‹ãƒƒãƒˆåã§æ¤œç´¢
		'UPGRADE_WARNING: ƒIƒuƒWƒFƒNƒg Index ‚ÌŠù’èƒvƒƒpƒeƒB‚ğ‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					IsDefined = True
					Exit Function
				End If
				'End If
			End With
		Next u
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Update()
		Dim u As Unit
		Dim k, i, j, n As Short
		Dim prev_money As Integer
		Dim flag As Boolean
		Dim pname, uname, uname2, buf As String
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				For i = 1 To .CountUnitOnBoard
					.UnloadUnit(1)
				Next 
			End With
		Next u
		
		'ç ´å£Šã•ã‚ŒãŸå‘³æ–¹ãƒ¦ãƒ‹ãƒƒãƒˆãŒã‚ã‚‹ã‹æ¤œç´¢
		For	Each u In colUnits
			With u
				If .Party0 = "å‘³æ–¹" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					flag = True
					Exit For
				End If
				'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If Not .Summoner Is Nothing Then
					If .Summoner.Party0 = "å‘³æ–¹" Then
						flag = True
						Exit For
					End If
				End If
				'End If
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		If flag Then
			OpenMessageForm()
			prev_money = Money
			For	Each u In colUnits
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextDestroyedUnit
				End With
			Next u
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo NextDestroyedUnit
		'End If
		'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		
		'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
		'End With
NextDestroyedUnit: 
		'Next
		DisplayMessage("Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code" & VB6.Format(prev_money - Money))
		CloseMessageForm()
		'End If
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				Select Case .Status_Renamed
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						.Status_Renamed = "Invalid_string_refer_to_original_code"
				End Select
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		For i = 1 To 3
			'Invalid_string_refer_to_original_code
			For	Each u In colUnits
				With u
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					'End If
					'End If
					'End If
				End With
			Next u
			
			'Invalid_string_refer_to_original_code
			For	Each u In colUnits
				With u
					If Not .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						GoTo NextLoop1
					End If
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextLoop1
					'End If
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					If .CountPilot = 0 Then
						GoTo NextLoop1
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextLoop1
					'End If
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextLoop1
					'End If
					
					'Invalid_string_refer_to_original_code
					n = 0
					For j = 2 To LLength(.FeatureData("Invalid_string_refer_to_original_code"))
						uname = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), j)
						If UDList.IsDefined(uname) Then
							With UDList.Item(uname)
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
								n = n + .PilotNum
							End With
						End If
					Next 
				End With
				'End If
			Next u
			'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: Update ‚É•ÏŠ·‚³‚ê‚Ä‚¢‚È‚¢ƒXƒe[ƒgƒƒ“ƒg‚ª‚ ‚è‚Ü‚·Bƒ\[ƒX ƒR[ƒh‚ğŠm”F‚µ‚Ä‚­‚¾‚³‚¢B
			'End With
NextLoop1: 
		Next 
		
		'åˆä½“ã‚’è¡Œã†
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				For j = 1 To .CountFeature
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					GoTo NextLoop2
					'End If
					
					'Invalid_string_refer_to_original_code
					uname = LIndex(.FeatureData(j), 2)
					If Not UList.IsDefined(uname) Then
						GoTo NextLoop2
					End If
					With UList.Item(uname)
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						GoTo NextLoop2
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						GoTo NextLoop2
						'End If
						'Invalid_string_refer_to_original_code_
						'And LLength(u.FeatureData(j)) = 3 _
						'Then
						'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						GoTo NextLoop2
						'End If
					End With
					
					'Invalid_string_refer_to_original_code
					For k = 3 To LLength(.FeatureData(j))
						uname = LIndex(.FeatureData(j), k)
						If Not UList.IsDefined(uname) Then
							GoTo NextLoop2
						End If
						With UList.Item(uname)
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
							GoTo NextLoop2
							'End If
							If .CurrentForm.Status_Renamed <> "é›¢è„±" Then
								GoTo NextLoop2
							End If
							'End If
						End With
					Next 
					
					'åˆä½“ã‚’å®Ÿæ–½
					.Combine(LIndex(.FeatureData(j), 2))
					Exit For
NextLoop2: 
				Next 
				'End If
				'End If
			End With
		Next u
		
		'æ¨™æº–å½¢æ…‹ã«å¤‰å½¢
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				If .IsFeatureAvailable("å¤‰å½¢") Then
					uname = .Name
					buf = .FeatureData("å¤‰å½¢")
					For j = 2 To LLength(buf)
						uname2 = LIndex(buf, j)
						If UDList.IsDefined(uname2) Then
							If UDList.Item(uname2).ID < UDList.Item(uname).ID Then
								uname = uname2
							End If
						Else
							ErrorMessage(uname & "Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
						End If
					Next 
					
					If uname <> .Name Then
						.Transform(uname)
					End If
				End If
				'End If
			End With
		Next u
		'Next
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					If PList.IsDefined(.FeatureData("Invalid_string_refer_to_original_code")) Then
						PList.Delete(.FeatureData("Invalid_string_refer_to_original_code"))
					End If
				End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				If .CountPilot > 0 Then
					If .Pilot(1).Nickname0 = "Invalid_string_refer_to_original_code" Then
						.DeletePilot(1)
					End If
				End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.DeleteTemporaryOtherForm()
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.Status_Renamed = "Invalid_string_refer_to_original_code"
				'End If
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				.Status_Renamed = "Invalid_string_refer_to_original_code"
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountItem
					.Item(i).Exist = False
				Next 
				Delete(.ID)
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			u.Reset_Renamed()
		Next u
		
		'Invalid_string_refer_to_original_code
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Save()
		Dim i As Short
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, IDCount)
		WriteLine(SaveDataFileNumber, Count)
		For	Each u In colUnits
			With u
				WriteLine(SaveDataFileNumber, .Name)
				WriteLine(SaveDataFileNumber, .ID, .Rank, .Status_Renamed)
				
				WriteLine(SaveDataFileNumber, .CountOtherForm)
				For i = 1 To .CountOtherForm
					WriteLine(SaveDataFileNumber, .OtherForm(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountPilot)
				For i = 1 To .CountPilot
					WriteLine(SaveDataFileNumber, .Pilot(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountSupport)
				For i = 1 To .CountSupport
					WriteLine(SaveDataFileNumber, .Support(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountItem)
				For i = 1 To .CountItem
					WriteLine(SaveDataFileNumber, .Item(i).ID)
				Next 
			End With
		Next u
	End Sub
	
	'Invalid_string_refer_to_original_code
	'(ãƒªãƒ³ã‚¯ã¯å¾Œã§è¡Œã†)
	Public Sub Load()
		Dim num, num2 As Short
		Dim new_unit As Unit
		Dim Name As String
		'UPGRADE_NOTE: Status ‚Í Status_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dim ID, Status_Renamed As String
		Dim Rank As Short
		Dim i, j As Short
		Dim dummy As String
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			new_unit = New Unit
			With new_unit
				'Name
				Input(SaveDataFileNumber, Name)
				
				If Not UDList.IsDefined(Name) Then
					ErrorMessage(Name & "Invalid_string_refer_to_original_code")
					TerminateSRC()
					End
				End If
				
				'ID, Rank, Status
				Input(SaveDataFileNumber, ID)
				Input(SaveDataFileNumber, Rank)
				Input(SaveDataFileNumber, Status_Renamed)
				
				'Invalid_string_refer_to_original_code
				If SaveDataVersion < 10700 Then
					ConvertUnitID(ID)
				End If
				
				.Name = Name
				.ID = ID
				.Rank = Rank
				.Party = "å‘³æ–¹"
				.Status_Renamed = Status_Renamed
				.FullRecover()
			End With
			colUnits.Add(new_unit, new_unit.ID)
			
			'OtherForm
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Pilot
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Support
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Item
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadLinkInfo()
		Dim num, num2 As Short
		Dim ID, ID2 As String
		Dim i, j As Short
		Dim int_dummy As Short
		Dim str_dummy As String
		Dim u As Unit
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'Name
			str_dummy = LineInput(SaveDataFileNumber)
			'ID, Rank, Status
			Input(SaveDataFileNumber, ID)
			Input(SaveDataFileNumber, int_dummy)
			Input(SaveDataFileNumber, str_dummy)
			
			'Invalid_string_refer_to_original_code
			If SaveDataVersion < 10700 Then
				ConvertUnitID(ID)
			End If
			
			With Item(ID)
				'OtherForm
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					ConvertUnitID(ID2)
					If IsDefined(ID2) Then
						.AddOtherForm(Item(ID2))
					End If
				Next 
				
				'Pilot
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddPilot(PList.Item(ID2))
						If .Status_Renamed = "é›¢è„±" Then
							PList.Item(ID2).Away = True
						End If
					Else
						ID2 = Left(ID2, InStr(ID2, "(") - 1)
						If PList.IsDefined(ID2) Then
							.AddPilot(PList.Item(ID2))
							If .Status_Renamed = "é›¢è„±" Then
								PList.Item(ID2).Away = True
							End If
						End If
					End If
				Next 
				
				'Support
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddSupport(PList.Item(ID2))
						If .Status_Renamed = "é›¢è„±" Then
							PList.Item(ID2).Away = True
						End If
					End If
				Next 
				
				'Unit
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If IList.IsDefined(ID2) Then
						If IList.Item(ID2).Unit Is Nothing Then
							.CurrentForm.AddItem0(IList.Item(ID2))
						End If
					ElseIf IDList.IsDefined(ID2) Then 
						.CurrentForm.AddItem0(IList.Add(ID2))
					End If
				Next 
			End With
		Next 
		
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Dump()
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, Count)
		
		For	Each u In colUnits
			u.Dump()
		Next u
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Restore()
		Dim i, num As Short
		Dim u As Unit
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			u = New Unit
			With u
				.Restore()
				colUnits.Add(u, .ID)
			End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreLinkInfo()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreLinkInfo()
		Next u
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreParameter()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreParameter()
		Next u
	End Sub
	
	
	'ãƒ¦ãƒ‹ãƒƒãƒˆãƒªã‚¹ãƒˆã‚’ã‚¯ãƒªã‚¢
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ClearUnitBitmap()
		Dim u As Unit
		
		'UPGRADE_ISSUE: Control picUnitBitmap ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		With MainForm.picUnitBitmap
			'UPGRADE_ISSUE: Control picUnitBitmap ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			If .Width = 32 Then
				'Invalid_string_refer_to_original_code
				Exit Sub
			End If
			
			'ç”»åƒã‚’ã‚¯ãƒªã‚¢
			'UPGRADE_ISSUE: Control picUnitBitmap ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picUnitBitmap ‚ÍA”Ä—p–¼‘O‹óŠÔ Form “à‚É‚ ‚é‚½‚ßA‰ğŒˆ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
			.Move(0, 0, 32, 96)
		End With
		
		'BitmapIDã‚’ã‚¯ãƒªã‚¢
		For	Each u In colUnits
			u.BitmapID = 0
		Next u
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub CheckAutoHyperMode()
		Dim u As Unit
		
		For	Each u In colUnits
			u.CheckAutoHyperMode()
		Next u
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CheckAutoNormalMode()
		Dim u As Unit
		Dim is_redraw_necessary As Boolean
		
		For	Each u In colUnits
			If u.CheckAutoNormalMode(True) Then
				is_redraw_necessary = True
			End If
		Next u
		
		'Invalid_string_refer_to_original_code
		If is_redraw_necessary Then
			RedrawScreen()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Clean()
		Dim u As Unit
		Dim i As Short
		
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code
				If .Party0 <> "å‘³æ–¹" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
					.Status_Renamed = "Invalid_string_refer_to_original_code"
					For i = 1 To .CountOtherForm
						.OtherForm(i).Status_Renamed = "Invalid_string_refer_to_original_code"
					Next 
				End If
				'End If
			End With
		Next u
		
		For	Each u In colUnits
			With u
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountPilot
					.Pilot(i).Alive = False
				Next 
				For i = 1 To .CountSupport
					.Support(i).Alive = False
				Next 
				
				'Invalid_string_refer_to_original_code
				For i = 1 To .CountItem
					.Item(i).Exist = False
				Next 
				
				Delete(.ID)
				'End If
			End With
		Next u
	End Sub
End Class