Option Strict Off
Option Explicit On
Module BCVariable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public IsConfig As Boolean
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	' ---------かならず定義されるデータ
	Public MeUnit As Unit
	
	'Invalid_string_refer_to_original_code
	Public AtkUnit As Unit
	
	' 防御側ユニット定義
	Public DefUnit As Unit
	
	' 武器番号
	Public WeaponNumber As Short
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public AttackExp As Integer
	
	'Invalid_string_refer_to_original_code
	Public AttackVariable As Integer
	
	' 防御側定義変数
	Public DffenceVariable As Integer
	
	' 地形補正
	Public TerrainAdaption As Double
	
	' サイズ補正
	Public SizeMod As Double
	
	' 最終値
	Public LastVariable As Integer
	
	'Invalid_string_refer_to_original_code
	Public WeaponPower As Integer
	
	'Invalid_string_refer_to_original_code
	Public Armor As Integer
	
	' ザコ補正
	Public CommonEnemy As Integer
	
	'Invalid_string_refer_to_original_code
	Public Sub DataReset()
		AttackExp = 0
		AttackVariable = 0
		DffenceVariable = 0
		TerrainAdaption = 1
		SizeMod = 1
		LastVariable = 0
		WeaponPower = 0
		CommonEnemy = 0
	End Sub
End Module