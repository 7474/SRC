---
layout: default
title: Fontコマンド
---
** 内容はSRC2.2.33のものです **

**Fontコマンド**

[**PaintString**コマンド](PaintStringコマンド.md)で使うフォントを変更

**書式**

**Font** [*options*]

**指定項目説明**

*options*フォントのオプション

**解説**

[**PaintString**コマンド](PaintStringコマンド.md)で表示する文字列のフォントを変更します。指定可能なフォントのオプションは以下の通りです。

**数値pt**フォントのサイズを指定した数値に変更します。標準のフォントは16ptです。

**#rrggbb**ＲＧＢ16進(HTMLの色指定と同形式)でフォントの色を指定します。

RGB関数を使えばＲＧＢ値からＲＧＢ16進を計算することができます。

**Ｐ明朝**フォントタイプを「ＭＳ Ｐ明朝」に変更します。

**Ｐゴシック**フォントタイプを「ＭＳ Ｐゴシック」に変更します。

**明朝**フォントタイプを「ＭＳ 明朝」に変更します。

**ゴシック**フォントタイプを「ＭＳ ゴシック」に変更します。

*フォント名*フォントタイプを指定したフォントに変更します。

フォント名にスペースが含まれる場合はフォント名を「"」で囲んでください。

なお、上記４つのフォント以外のフォントはPCによって利用できない場合

があるので注意して下さい(利用できない場合は類似のフォントが選択されます)。

また、上記の４つのフォント以外のフォントは全角文字の表示の際に文字化けして

しまうことがあるので注意して下さい。

**Regular**フォントをItalic無し、Bold無しに設定します。

**Italic**フォントをItalicあり(斜体)にします。

**Bold**フォントをBoldあり(太字)にします。

**背景**文字をマップ背景として書き込みます。

背景オプションを指定して書きこんだ文字はマップの画像と同等に扱われ、

[**ChangeMap**コマンド](ChangeMapコマンド.md)でマップを変更するか、

[**Night**コマンド](Nightコマンド.md)等でマップの表示色を変更するまで消去されません。

![](./images/bm0.gif) 背景オプションを使って書き込んだ文字は[**Redraw**コマンド](Redrawコマンド.md)を使うまで表示されません。

**保持**文字を[**ClearPicture**コマンド](ClearPictureコマンド.md)で消えないようにして描画します。

アニメーション表示時の背景等を描画する時に使います。

保持オプションを使って描画された文字は[**Redraw**コマンド](Redrawコマンド.md)や[**Center**コマンド](Centerコマンド.md)等によって

マップウィンドウが再描画される際に消去されます。

**通常**文字をマップ背景ではなく一時的な書き込みとして書き込みます。

これらのオプションは複数同時に適用することができます。指定しなかったオプションに関する設定は変化しません。例えば「Font Bold」を実行した後に「Font Italic」を実行した場合、フォントは「Italicあり」でかつ「Boldあり」の設定になり、「Boldあり」の設定はキャンセルされません。

オプションを指定せずに**Font**コマンドを実行した場合はフォント設定が標準の設定に戻されます。標準の設定は、

**Font 16pt #000000 Ｐ明朝 Bold 通常**

と同じです。

**Font**コマンドで行った設定はイベント終了時に自動的に解除されます。

**Font**コマンドで設定したフォントに関する情報はFont関数で参照することが出来ます。

**例**
```sh
#フォントサイズ・種類を変更
Font 24pt Ｐゴシック Regular
#フェードインしながらメッセージを表示
For i = 1 To 20
#色を変更
Font RGB(4 \* i, 8 \* i, 12.5 \* i)
#メッセージを表示
PaintString - 200 A long time ago in a galaxy far,
PaintString far away ....
#画面を更新
Refresh
Next
#デフォルトのフォントに戻しておく
Font
```

