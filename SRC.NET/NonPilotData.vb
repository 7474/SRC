Option Strict Off
Option Explicit On
Friend Class NonPilotData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'名称
	Public Name As String
	
	'愛称
	Private strNickname As String
	
	'Invalid_string_refer_to_original_code
	Private proBitmap As String
	'Invalid_string_refer_to_original_code
	Public IsBitmapMissing As Boolean
	
	
	'愛称
	Public ReadOnly Property Nickname0() As String
		Get
			Nickname0 = strNickname
		End Get
	End Property
	
	
	Public Property Nickname() As String
		Get
			Dim pname As String
			Dim idx As Short
			
			Nickname = strNickname
			
			'Invalid_string_refer_to_original_code
			If InStr(Nickname, "主人公") = 1 Or InStr(Nickname, "ヒロイン") = 1 Then
				Nickname = GetValueAsString(Nickname & "愛称")
			End If
			
			ReplaceSubExpression(Nickname)
			
			'Invalid_string_refer_to_original_code
			idx = InStr(Name, "(")
			If idx > 1 Then
				'Invalid_string_refer_to_original_code
				pname = Left(Name, idx - 1)
				
				'Invalid_string_refer_to_original_code
				If Not PList.IsDefined(pname) Then
					Exit Property
				End If
				
				With PList.Item(pname)
					'Invalid_string_refer_to_original_code
					If .Unit_Renamed Is Nothing Then
						Exit Property
					End If
					
					With .Unit_Renamed
						'念のため……
						If .CountPilot = 0 Then
							Exit Property
						End If
						
						'Invalid_string_refer_to_original_code
						If pname <> .MainPilot.Name And pname <> .MainPilot.Data.Nickname Then
							Exit Property
						End If
						
						'Invalid_string_refer_to_original_code
						If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
							pname = .FeatureData("Invalid_string_refer_to_original_code")
							idx = InStr(pname, "$(愛称)")
							If idx > 0 Then
								pname = Left(pname, idx - 1) & Nickname & Mid(pname, idx + 5)
							End If
							Nickname = pname
						End If
					End With
				End With
			End If
		End Get
		Set(ByVal Value As String)
			strNickname = Value
		End Set
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property Bitmap0() As String
		Get
			Bitmap0 = proBitmap
		End Get
	End Property
	
	
	Public Property Bitmap() As String
		Get
			If IsBitmapMissing Then
				Bitmap = "-.bmp"
			Else
				Bitmap = proBitmap
			End If
		End Get
		Set(ByVal Value As String)
			proBitmap = Value
		End Set
	End Property
End Class