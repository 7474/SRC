VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BattleConfigData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'バトルコンフィグデータのクラス
' --- ダメージ計算、命中率算出など、バトルに関連するエリアスを定義します。

'名称
Public Name As String

'計算式
Public ConfigCalc As String

'クラスの初期化
Private Sub Class_Initialize()
    ConfigCalc = ""
End Sub

'バトルコンフィグデータに基づいた置換＆計算の実行
'実行前に使う可能性のある変数を事前に代入しておくこと
Public Function Calculate() As Double
Dim expr As String
Dim morales As Long
    
    expr = ConfigCalc
    
    'コンフィグ変数を有効にする
    BCVariable.IsConfig = True
    
    '式を評価する
    Calculate = GetValueAsDouble(expr)
    
    'コンフィグ変数を無効にする
    BCVariable.IsConfig = False
End Function

