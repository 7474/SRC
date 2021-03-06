**FillColorコマンド**

図形の背景色を変更

**書式**

**FillColor** *rgb*

**指定項目説明**

*rgb*背景色のＲＧＢ値

**解説**

[**Line**コマンド](Lineコマンド.md)などのコマンドで描画する図形の背景色を変更するためのコマンドです。ＲＧＢ16進(HTMLの色指定と同形式)で描画色を指定します。RGB関数を使えばＲＧＢ値からＲＧＢ16進を計算することができます。

デフォルトの状態では図形の背景描画方法は透過となっています。このため、**FillColor**コマンドで背景色を設定しただけでは背景の塗りつぶしは行われません。**FillColor**コマンドを使う際は必ず[**FillStyle**コマンド](FillStyleコマンド.md)を使って図形の背景描画の方法を変更してください。

**FillColor**コマンドが実行されてない場合、背景色は白(255,255,255)になります。

**FillColor**コマンドで行った設定はイベント終了時に自動的に解除されます。

**例**

#色を変えながらランダムに円を描画

FillStyle 塗りつぶし

#色を変えながらランダムに円を描画

For i = 1 To 100

Color RGB(i, 0, 255 - i)

FillColor RGB(i, 0, 255 - i)

Circle (80 + Random(400)) (80 + Random(400)) 25

Next
