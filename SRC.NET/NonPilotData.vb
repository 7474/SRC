Option Strict Off
Option Explicit On
Friend Class NonPilotData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ノンパイロットデータのクラス
	
	'名称
	Public Name As String
	
	'愛称
	Private strNickname As String
	
	'ビットマップ名
	Private proBitmap As String
	'ビットマップが存在するか
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
			
			'イベントで愛称が変更されている？
			If InStr(Nickname, "主人公") = 1 Or InStr(Nickname, "ヒロイン") = 1 Then
				Nickname = GetValueAsString(Nickname & "愛称")
			End If
			
			ReplaceSubExpression(Nickname)
			
			'表情パターンの場合
			idx = InStr(Name, "(")
			If idx > 1 Then
				'パイロット本来の名称or愛称を切り出し
				pname = Left(Name, idx - 1)
				
				'そのパイロットが作成されている？
				If Not PList.IsDefined(pname) Then
					Exit Property
				End If
				
				With PList.Item(pname)
					'パイロットがユニットに乗っている？
					If .Unit_Renamed Is Nothing Then
						Exit Property
					End If
					
					With .Unit_Renamed
						'念のため……
						If .CountPilot = 0 Then
							Exit Property
						End If
						
						'パイロットはメインパイロット？
						If pname <> .MainPilot.Name And pname <> .MainPilot.Data.Nickname Then
							Exit Property
						End If
						
						'パイロット愛称変更能力を適用
						If .IsFeatureAvailable("パイロット愛称") Then
							pname = .FeatureData("パイロット愛称")
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
	
	'ビットマップ
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