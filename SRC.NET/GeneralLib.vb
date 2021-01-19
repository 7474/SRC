Option Strict Off
Option Explicit On
Module GeneralLib
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function GetPrivateProfileString Lib "kernel32"  Alias "GetPrivateProfileStringA"(ByVal lpApplicationName As String, ByVal lpKeyName As Any, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
	
	'iniãƒ•ã‚¡ã‚¤ãƒ«ã¸ã®æ›¸ãè¾¼ã¿
	'UPGRADE_ISSUE: ƒpƒ‰ƒ[ƒ^ 'As Any' ‚ÌéŒ¾‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function WritePrivateProfileString Lib "kernel32"  Alias "WritePrivateProfileStringA"(ByVal lpApplicationName As String, ByVal lpKeyName As Any, ByVal lpString As String, ByVal lpFileName As String) As Integer
	
	'Invalid_string_refer_to_original_code
	Declare Function timeGetTime Lib "winmm.dll" () As Integer
	
	'Invalid_string_refer_to_original_code
	Declare Function timeBeginPeriod Lib "winmm.dll" (ByVal uPeriod As Integer) As Integer
	Declare Function timeEndPeriod Lib "winmm.dll" (ByVal uPeriod As Integer) As Integer
	
	'ãƒ•ã‚¡ã‚¤ãƒ«å±æ€§ã‚’è¿”ã™
	Declare Function GetFileAttributes Lib "kernel32"  Alias "GetFileAttributesA"(ByVal lpFileName As String) As Integer
	
	'Invalid_string_refer_to_original_code
	Structure OSVERSIONINFO
		Dim dwOSVersionInfoSize As Integer
		Dim dwMajorVersion As Integer
		Dim dwMinorVersion As Integer
		Dim dwBuildNumber As Integer
		Dim dwPlatformId As Integer
		'UPGRADE_WARNING: ŒÅ’è’·•¶š—ñ‚ÌƒTƒCƒY‚Íƒoƒbƒtƒ@‚É‡‚í‚¹‚é•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		<VBFixedString(128),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=128)> Public szCSDVersion() As Char
	End Structure
	
	'UPGRADE_WARNING: \‘¢‘Ì OSVERSIONINFO ‚ÉA‚±‚Ì Declare ƒXƒe[ƒgƒƒ“ƒg‚Ìˆø”‚Æ‚µ‚Äƒ}[ƒVƒƒƒŠƒ“ƒO‘®«‚ğ“n‚·•K—v‚ª‚ ‚è‚Ü‚·B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Declare Function GetVersionEx Lib "kernel32"  Alias "GetVersionExA"(ByRef lpVersionInformation As OSVERSIONINFO) As Integer
	Const VER_PLATFORM_WIN32_NT As Short = 2
	
	
	'ä¹±æ•°ç™ºç”Ÿç”¨ã‚·ãƒ¼ãƒ‰å€¤
	Public RndSeed As Integer
	
	'Invalid_string_refer_to_original_code
	Private RndHistory(4096) As Single
	
	'Invalid_string_refer_to_original_code
	Public RndIndex As Short
	
	'Invalid_string_refer_to_original_code
	Public Sub RndReset()
		Dim i As Short
		
		Randomize(RndSeed)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		For i = 1 To UBound(RndHistory)
			RndHistory(i) = Rnd()
		Next 
		
		RndIndex = 0
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Dice(ByVal max As Integer) As Integer
		If max <= 1 Then
			Dice = max
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		Dice = Int((max * Rnd()) + 1)
		Exit Function
		'End If
		
		RndIndex = RndIndex + 1
		If RndIndex > UBound(RndHistory) Then
			RndIndex = 1
		End If
		
		Dice = Int((max * RndHistory(RndIndex)) + 1)
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function LIndex(ByRef list As String, ByVal idx As Short) As String
		Dim i, n As Short
		Dim list_len As Short
		Dim begin As Short
		
		'Invalid_string_refer_to_original_code
		If idx < 1 Then
			Exit Function
		End If
		
		list_len = Len(list)
		
		'Invalid_string_refer_to_original_code
		n = 0
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				If i > list_len Then
					Exit Function
				End If
			Loop While Mid(list, i, 1) = " "
			
			'Invalid_string_refer_to_original_code
			n = n + 1
			
			'Invalid_string_refer_to_original_code
			If n = idx Then
				Exit Do
			End If
			
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				If i > list_len Then
					Exit Function
				End If
			Loop Until Mid(list, i, 1) = " "
		Loop 
		
		'æ±‚ã‚ã‚‹è¦ç´ ã‚’èª­ã¿è¾¼ã‚€
		begin = i
		Do 
			i = i + 1
			If i > list_len Then
				LIndex = Mid(list, begin)
				Exit Function
			End If
		Loop Until Mid(list, i, 1) = " "
		
		LIndex = Mid(list, begin, i - begin)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function LLength(ByRef list As String) As Short
		Dim i As Short
		Dim list_len As Short
		
		LLength = 0
		list_len = Len(list)
		
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				If i > list_len Then
					Exit Function
				End If
			Loop While Mid(list, i, 1) = " "
			
			'Invalid_string_refer_to_original_code
			LLength = LLength + 1
			
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				If i > list_len Then
					Exit Function
				End If
			Loop Until Mid(list, i, 1) = " "
		Loop 
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function LSplit(ByRef list As String, ByRef larray() As String) As Short
		Dim i As Short
		Dim list_len As Short
		Dim begin As Short
		
		LSplit = 0
		list_len = Len(list)
		
		ReDim larray(0)
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do 
				i = i + 1
				If i > list_len Then
					Exit Function
				End If
			Loop While Mid(list, i, 1) = " "
			
			'Invalid_string_refer_to_original_code
			LSplit = LSplit + 1
			
			'è¦ç´ ã‚’èª­ã¿è¾¼ã‚€
			ReDim Preserve larray(LSplit)
			begin = i
			Do 
				i = i + 1
				If i > list_len Then
					larray(LSplit) = Mid(list, begin)
					Exit Function
				End If
			Loop Until Mid(list, i, 1) = " "
			larray(LSplit) = Mid(list, begin, i - begin)
		Loop 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsSpace(ByRef ch As String) As Boolean
		If Len(ch) = 0 Then
			IsSpace = True
			Exit Function
		End If
		
		Select Case Asc(ch)
			Case 9, 13, 32, 160
				IsSpace = True
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Sub LAppend(ByRef list As String, ByRef str_Renamed As String)
		list = Trim(list)
		str_Renamed = Trim(str_Renamed)
		If list <> "" Then
			If str_Renamed <> "" Then
				list = list & " " & str_Renamed
			End If
		Else
			If str_Renamed <> "" Then
				list = str_Renamed
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Function SearchList(ByRef list As String, ByRef str_Renamed As String) As Short
		Dim i As Short
		
		For i = 1 To LLength(list)
			If LIndex(list, i) = str_Renamed Then
				SearchList = i
				Exit Function
			End If
		Next 
		
		SearchList = 0
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function ListIndex(ByRef list As String, ByVal idx As Short) As String
		Dim n, i, ch As Short
		Dim paren, list_len, begin As Short
		Dim in_single_quote, in_double_quote As Boolean
		
		'Invalid_string_refer_to_original_code
		If idx < 1 Then
			Exit Function
		End If
		
		list_len = Len(list)
		
		'Invalid_string_refer_to_original_code
		n = 0
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do While True
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				Select Case ch
					Case 9, 32
						'Invalid_string_refer_to_original_code
					Case Else
						Exit Do
				End Select
			Loop 
			
			'Invalid_string_refer_to_original_code
			n = n + 1
			
			'Invalid_string_refer_to_original_code
			If n = idx Then
				Exit Do
			End If
			
			'Invalid_string_refer_to_original_code
			Do While True
				If in_single_quote Then
					If ch = 96 Then ' "`"
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If ch = 34 Then ' """"
						in_double_quote = False
					End If
				Else
					Select Case ch
						Case 40, 91 ' "(", "["
							paren = paren + 1
						Case 41, 93 ' ")", "]"
							paren = paren - 1
							If paren < 0 Then
								'Invalid_string_refer_to_original_code
								Exit Function
							End If
						Case 96 ' "`"
							in_single_quote = True
						Case 34 ' """"
							in_double_quote = True
					End Select
				End If
				
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				If Not in_single_quote And Not in_double_quote And paren = 0 Then
					'Invalid_string_refer_to_original_code
					Select Case ch
						Case 9, 32
							Exit Do
					End Select
				End If
			Loop 
		Loop 
		
		'æ±‚ã‚ã‚‹è¦ç´ ã‚’èª­ã¿è¾¼ã‚€
		begin = i
		Do While True
			If in_single_quote Then
				If ch = 96 Then ' "`"
					in_single_quote = False
				End If
			ElseIf in_double_quote Then 
				If ch = 34 Then ' """"
					in_double_quote = False
				End If
			Else
				Select Case ch
					Case 40, 91 ' "(", "["
						paren = paren + 1
					Case 41, 93 ' ")", "]"
						paren = paren - 1
						If paren < 0 Then
							'Invalid_string_refer_to_original_code
							Exit Function
						End If
					Case 96 ' "`"
						in_single_quote = True
					Case 34 ' """"
						in_double_quote = True
				End Select
			End If
			
			i = i + 1
			
			'Invalid_string_refer_to_original_code
			If i > list_len Then
				ListIndex = Mid(list, begin)
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			ch = Asc(Mid(list, i, 1))
			
			'Invalid_string_refer_to_original_code
			If Not in_single_quote And Not in_double_quote And paren = 0 Then
				'Invalid_string_refer_to_original_code
				Select Case ch
					Case 9, 32
						Exit Do
				End Select
			End If
		Loop 
		
		ListIndex = Mid(list, begin, i - begin)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function ListLength(ByRef list As String) As Short
		Dim i, ch As Short
		Dim list_len, paren As Short
		Dim in_single_quote, in_double_quote As Boolean
		
		ListLength = 0
		list_len = Len(list)
		
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do While True
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				Select Case ch
					Case 9, 32
						'Invalid_string_refer_to_original_code
					Case Else
						Exit Do
				End Select
			Loop 
			
			'Invalid_string_refer_to_original_code
			ListLength = ListLength + 1
			
			'Invalid_string_refer_to_original_code
			Do While True
				If in_single_quote Then
					If ch = 96 Then ' "`"
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If ch = 34 Then ' """"
						in_double_quote = False
					End If
				Else
					Select Case ch
						Case 40, 91 ' "(", "["
							paren = paren + 1
						Case 41, 93 ' ")", "]"
							paren = paren - 1
							If paren < 0 Then
								'Invalid_string_refer_to_original_code
								ListLength = -1
								Exit Function
							End If
						Case 96 ' "`"
							in_single_quote = True
						Case 34 ' """"
							in_double_quote = True
					End Select
				End If
				
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					'Invalid_string_refer_to_original_code
					If in_single_quote Or in_double_quote Or paren <> 0 Then
						ListLength = -1
					End If
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				If Not in_single_quote And Not in_double_quote And paren = 0 Then
					'Invalid_string_refer_to_original_code
					Select Case ch
						Case 9, 32
							Exit Do
					End Select
				End If
			Loop 
		Loop 
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function ListSplit(ByRef list As String, ByRef larray() As String) As Short
		Dim n, i, ch As Short
		Dim paren, list_len, begin As Short
		Dim in_single_quote, in_double_quote As Boolean
		
		ListSplit = -1
		list_len = Len(list)
		
		ReDim larray(0)
		n = 0
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do While True
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					ListSplit = n
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				Select Case ch
					Case 9, 32
						'Invalid_string_refer_to_original_code
					Case Else
						Exit Do
				End Select
			Loop 
			
			'Invalid_string_refer_to_original_code
			n = n + 1
			
			'è¦ç´ ã‚’èª­ã¿è¾¼ã‚€
			ReDim Preserve larray(n)
			begin = i
			Do While True
				If in_single_quote Then
					If ch = 96 Then ' "`"
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If ch = 34 Then ' """"
						in_double_quote = False
					End If
				Else
					Select Case ch
						Case 40, 91 ' "(", "["
							paren = paren + 1
						Case 41, 93 ' ")", "]"
							paren = paren - 1
							If paren < 0 Then
								'Invalid_string_refer_to_original_code
								larray(n) = Mid(list, begin)
								Exit Function
							End If
						Case 96 ' "`"
							in_single_quote = True
						Case 34 ' """"
							in_double_quote = True
					End Select
				End If
				
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					larray(n) = Mid(list, begin)
					'Invalid_string_refer_to_original_code
					If Not in_single_quote And Not in_double_quote And paren = 0 Then
						ListSplit = n
					End If
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				If Not in_single_quote And Not in_double_quote And paren = 0 Then
					'Invalid_string_refer_to_original_code
					Select Case ch
						Case 9, 32
							Exit Do
					End Select
				End If
			Loop 
			larray(n) = Mid(list, begin, i - begin)
		Loop 
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function ListTail(ByRef list As String, ByVal idx As Short) As String
		Dim n, i, ch As Short
		Dim list_len, paren As Short
		Dim in_single_quote, in_double_quote As Boolean
		
		'Invalid_string_refer_to_original_code
		If idx < 1 Then
			Exit Function
		End If
		
		list_len = Len(list)
		
		'Invalid_string_refer_to_original_code
		n = 0
		i = 0
		Do While True
			'Invalid_string_refer_to_original_code
			Do While True
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				Select Case ch
					Case 9, 32
						'Invalid_string_refer_to_original_code
					Case Else
						Exit Do
				End Select
			Loop 
			
			'Invalid_string_refer_to_original_code
			n = n + 1
			
			'Invalid_string_refer_to_original_code
			If n = idx Then
				Exit Do
			End If
			
			'Invalid_string_refer_to_original_code
			Do While True
				If in_single_quote Then
					If ch = 96 Then ' "`"
						in_single_quote = False
					End If
				ElseIf in_double_quote Then 
					If ch = 34 Then ' """"
						in_double_quote = False
					End If
				Else
					Select Case ch
						Case 40, 91 ' "(", "["
							paren = paren + 1
						Case 41, 93 ' ")", "]"
							paren = paren - 1
							If paren < 0 Then
								'Invalid_string_refer_to_original_code
								Exit Function
							End If
						Case 96 ' "`"
							in_single_quote = True
						Case 34 ' """"
							in_double_quote = True
					End Select
				End If
				
				i = i + 1
				
				'Invalid_string_refer_to_original_code
				If i > list_len Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				ch = Asc(Mid(list, i, 1))
				
				'Invalid_string_refer_to_original_code
				If Not in_single_quote And Not in_double_quote And paren = 0 Then
					'Invalid_string_refer_to_original_code
					Select Case ch
						Case 9, 32
							Exit Do
					End Select
				End If
			Loop 
		Loop 
		
		ListTail = Mid(list, i)
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Sub TrimString(ByRef str_Renamed As String)
		Dim j, i, lstr As Short
		
		lstr = Len(str_Renamed)
		i = 1
		j = lstr
		
		'å…ˆé ­ã®ç©ºç™½ã‚’æ¤œç´¢
		Do While i <= j
			Select Case Asc(Mid(str_Renamed, i))
				Case 9, 32, -32448
					i = i + 1
				Case Else
					Exit Do
			End Select
		Loop 
		
		'æœ«å°¾ã®ç©ºç™½ã‚’æ¤œç´¢
		Do While i < j
			Select Case Asc(Mid(str_Renamed, j))
				Case 9, 32, -32448
					j = j - 1
				Case Else
					Exit Do
			End Select
		Loop 
		
		'Invalid_string_refer_to_original_code
		If i <> 1 Or j <> lstr Then
			str_Renamed = Mid(str_Renamed, i, j - i + 1)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Function InStr2(ByRef str_Renamed As String, ByRef str2 As String) As Short
		Dim slen, i As Short
		
		slen = Len(str2)
		i = Len(str_Renamed) - slen + 1
		Do While i > 0
			If Mid(str_Renamed, i, slen) = str2 Then
				InStr2 = i
				Exit Function
			End If
			i = i - 1
		Loop 
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function StrToDbl(ByRef expr As String) As Double
		If IsNumeric(expr) Then
			StrToDbl = CDbl(expr)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function StrToLng(ByRef expr As String) As Integer
		On Error GoTo ErrorHandler
		If IsNumeric(expr) Then
			StrToLng = CInt(expr)
		End If
		Exit Function
ErrorHandler: 
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Function StrToHiragana(ByRef str_Renamed As String) As String
		On Error GoTo ErrorHandler
		
		StrToHiragana = StrConv(str_Renamed, VbStrConv.Hiragana)
		
		Exit Function
		
ErrorHandler: 
		
		StrToHiragana = str_Renamed
	End Function
	
	
	'aã¨bã®æœ€å¤§å€¤ã‚’è¿”ã™
	Public Function MaxLng(ByVal a As Integer, ByVal b As Integer) As Integer
		If a > b Then
			MaxLng = a
		Else
			MaxLng = b
		End If
	End Function
	
	'aã¨bã®æœ€å°å€¤ã‚’è¿”ã™
	Public Function MinLng(ByVal a As Integer, ByVal b As Integer) As Integer
		If a < b Then
			MinLng = a
		Else
			MinLng = b
		End If
	End Function
	
	'aã¨bã®æœ€å¤§å€¤ã‚’è¿”ã™ (Double)
	Public Function MaxDbl(ByVal a As Double, ByVal b As Double) As Double
		If a > b Then
			MaxDbl = a
		Else
			MaxDbl = b
		End If
	End Function
	
	'aã¨bã®æœ€å°å€¤ã‚’è¿”ã™ (Double)
	Public Function MinDbl(ByVal a As Double, ByVal b As Double) As Double
		If a < b Then
			MinDbl = a
		Else
			MinDbl = b
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function LeftPaddedString(ByRef buf As String, ByVal length As Short) As String
		'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		LeftPaddedString = Space(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0)) & buf
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function RightPaddedString(ByRef buf As String, ByVal length As Short) As String
		'UPGRADE_ISSUE: ’è” vbFromUnicode ‚ÍƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		'UPGRADE_ISSUE: LenB ŠÖ”‚ÍƒTƒ|[ƒg‚³‚ê‚Ü‚¹‚ñB Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		RightPaddedString = buf & Space(MaxLng(length - LenB(StrConv(buf, vbFromUnicode)), 0))
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function ReadIni(ByRef ini_section As String, ByRef ini_entry As String) As String
		Dim s As New VB6.FixedLengthString(1024)
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If FileExists(ScenarioPath & "Src.ini") Then
			ret = GetPrivateProfileString(ini_section, ini_entry, "", s.Value, 1024, ScenarioPath & "Src.ini")
		End If
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			ret = GetPrivateProfileString(ini_section, ini_entry, "", s.Value, 1024, AppPath & "Src.ini")
		End If
		
		'Invalid_string_refer_to_original_code
		ReadIni = Left(s.Value, InStr(s.Value, vbNullChar) - 1)
	End Function
	
	
	' Src.ini ãƒ•ã‚¡ã‚¤ãƒ«ã® ini_section ã® ini_entry ã«å€¤ ini_data ã‚’æ›¸ãè¾¼ã‚€
	Public Sub WriteIni(ByRef ini_section As String, ByRef ini_entry As String, ByRef ini_data As String)
		Dim s As New VB6.FixedLengthString(1024)
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If ini_entry = "LastFolder" Then
			ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, AppPath & "Src.ini")
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(ScenarioPath) > 0 And FileExists(ScenarioPath & "Src.ini") Then
			'Invalid_string_refer_to_original_code
			ret = GetPrivateProfileString(ini_section, ini_entry, "", s.Value, 1024, ScenarioPath & "Src.ini")
			If ret > 1 Then
				ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, ScenarioPath & "Src.ini")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, AppPath & "Src.ini")
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'ç½®æ›ã‚’è¡Œã£ãŸã¨ãã«ã¯Trueã‚’è¿”ã™
	Public Function ReplaceString(ByRef s1 As String, ByRef s2 As String, ByRef s3 As String) As Boolean
		Dim buf As String
		Dim len2, len3 As Short
		Dim idx As Short
		
		idx = InStr(s1, s2)
		
		'Invalid_string_refer_to_original_code
		If idx = 0 Then
			Exit Function
		End If
		
		len2 = Len(s2)
		len3 = Len(s3)
		
		'Invalid_string_refer_to_original_code
		If len2 = len3 Then
			Do 
				Mid(s1, idx, len2) = s3
				idx = InStr(s1, s2)
			Loop While idx > 0
		Else
			buf = s1
			s1 = ""
			Do 
				s1 = s1 & Left(buf, idx - 1) & s3
				buf = Mid(buf, idx + len2)
				idx = InStr(buf, s2)
			Loop While idx > 0
			s1 = s1 & buf
		End If
		
		ReplaceString = True
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function FileExists(ByRef fname As String) As Boolean
		If GetFileAttributes(fname) <> -1 Then
			FileExists = True
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub GetLine(ByRef fnum As Short, ByRef line_buf As String, ByRef line_num As Integer)
		Dim buf As String
		Dim idx As Short
		
		line_buf = ""
		Do Until EOF(fnum)
			line_num = line_num + 1
			buf = LineInput(fnum)
			
			'Invalid_string_refer_to_original_code
			If Left(buf, 1) = "#" Then
				GoTo NextLine
			End If
			
			'Invalid_string_refer_to_original_code
			idx = InStr(buf, "//")
			If idx > 0 Then
				buf = Left(buf, idx - 1)
			End If
			
			'Invalid_string_refer_to_original_code
			If Right(buf, 1) <> "_" Then
				TrimString(buf)
				line_buf = line_buf & buf
				Exit Do
			End If
			
			'Invalid_string_refer_to_original_code
			TrimString(buf)
			line_buf = line_buf & Left(buf, Len(buf) - 1)
			
NextLine: 
		Loop 
		
		ReplaceString(line_buf, "Invalid_string_refer_to_original_code", "")
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Function GetWinVersion() As Short
		Dim vinfo As OSVERSIONINFO
		Dim ret As Integer
		
		With vinfo
			'Invalid_string_refer_to_original_code
			.dwOSVersionInfoSize = Len(vinfo)
			
			'Invalid_string_refer_to_original_code
			ret = GetVersionEx(vinfo)
			If ret = 0 Then
				Exit Function
			End If
			
			GetWinVersion = .dwMajorVersion * 100 + .dwMinorVersion
		End With
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function FormatNum(ByVal n As Double) As String
		If n = Int(n) Then
			FormatNum = VB6.Format(n, "0")
		Else
			FormatNum = VB6.Format(n, "0.#######################################################################")
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: str ‚Í str_Renamed ‚ÉƒAƒbƒvƒOƒŒ[ƒh‚³‚ê‚Ü‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
	Public Function IsNumber(ByRef str_Renamed As String) As Boolean
		If Not IsNumeric(str_Renamed) Then
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If Asc(str_Renamed) = 40 Then
			Exit Function
		End If
		
		IsNumber = True
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function GetClassBundle(ByRef aname As String, ByRef idx As Short, Optional ByVal length As Short = 0) As String
		Dim i As Short
		Dim ch As String
		
		i = idx
		ch = Mid(aname, i, 1)
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		Do While ch = "å¼±" Or ch = "åŠ¹" Or ch = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			If i >= Len(aname) Then GoTo NotFoundClass
			i = i + 1
			ch = Mid(aname, i, 1)
		Loop 
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		i = i + 1
		'Invalid_string_refer_to_original_code
		If i > Len(aname) Then
			GoTo NotFoundClass
		End If
		ch = Mid(aname, i, 1)
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
		GoTo NotFoundClass
		'End If
		'End If
		If length = 0 Then
			GetClassBundle = Mid(aname, idx, i - idx + 1)
		Else
			GetClassBundle = Mid(aname, idx, length)
		End If
		
NotFoundClass: 
		idx = i
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function InStrNotNest(ByRef string1 As String, ByRef string2 As String, Optional ByVal start As Short = 1) As Short
		Dim i As Short
		Dim c As String
		
		i = InStr(start, string1, string2)
		'Invalid_string_refer_to_original_code
		If i <= 1 Then
			InStrNotNest = i
		Else
			Do While i > 0
				c = Mid(string1, i - 1, 1)
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: ‘O‚Ìs‚ğ‰ğÍ‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½B Ú×‚É‚Â‚¢‚Ä‚ÍA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ‚ğƒNƒŠƒbƒN‚µ‚Ä‚­‚¾‚³‚¢B
				Exit Do
			Loop 
		End If
		'Invalid_string_refer_to_original_code
		If i < Len(string1) Then
			i = InStr(i + 1, string1, string2)
		Else
			i = 0
		End If
		'Loop
		InStrNotNest = i
		'End If
	End Function
End Module