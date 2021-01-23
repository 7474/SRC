Attribute VB_Name = "BCVariable"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

' バトルコンフィグデータが有効か？
Public IsConfig As Boolean

' バトルコンフィグデータの各種変数を定義する

' バトルコンフィグデータ対象中心ユニット定義
' ---------かならず定義されるデータ
Public MeUnit As Unit

' 攻撃側ユニット定義
Public AtkUnit As Unit

' 防御側ユニット定義
Public DefUnit As Unit

' 武器番号
Public WeaponNumber As Integer

' ---------定義されない場合もある(計算後にリセットされる)データ
' 攻撃値
Public AttackExp As Long

' 攻撃側定義変数
Public AttackVariable As Long

' 防御側定義変数
Public DffenceVariable As Long

' 地形補正
Public TerrainAdaption As Double

' サイズ補正
Public SizeMod As Double

' 最終値
Public LastVariable As Long

' 武器攻撃力
Public WeaponPower As Long

' 装甲値
Public Armor As Long

' ザコ補正
Public CommonEnemy As Long

'定義されないこともあるデータをここでリセットする
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

