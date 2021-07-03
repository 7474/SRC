---
layout: default
title: PaintSysStringコマンド
---
** 内容は2.3系のものです **

**PaintSysStringコマンド**

文字列を表示

**書式**

**PaintSysString** [*x y*] *message* [*option*]

**指定項目説明**

*x*表示領域のX座標（マップ座標）

*y*表示領域のY座標（マップ座標）

*message*表示する文字列

*option*表示のオプション

**解説**

マップウィンドウにダメージや回復量等のシステム情報用に文字列message を表示します。表示に使われるフォントサイズや文字色は本体がダメージや回復量を表示するときに使うものと同じものが使われます(Fontコマンドでの設定は無視される)。

表示位置はx y にマップ座標で指定して行います。[PaintStringコマンド](PaintStringコマンド.md)等で使われているピクセル座標とは異なるので注意してください。

optionには「非同期」を指定することが出来ます。この場合、PaintSysStringコマンドを実行しても画面は更新されません。画面を更新したいタイミングで[Refreshコマンド](Refreshコマンド.md)を実行して下さい。


**例**
```sh
#ダメージを表示
戦闘アニメ_ダメージ表示発動:
PaintSysString X(相手ユニットＩＤ) Y(相手ユニットＩＤ) Args(1)
Return
```
