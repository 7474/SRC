---
layout: default
title: StopBGMコマンド
---
** 内容はSRC2.2.33のものです **

**StopBGMコマンド**

ＢＧＭを停止

**書式**

**StopBGM**

**解説**

ＢＧＭの演奏を停止します。ＢＧＭが[**StartBGM**コマンド](StartBGMコマンド.md)、[**PlayMIDI**コマンド](PlayMIDIコマンド.md)のどちらで開始されている場合も有効です。

ターン開始時等の際にもＢＧＭを止めたままにしておきたい場合は[**KeepBGM**コマンド](KeepBGMコマンド.md)を**StopBGM**コマンドの直後に使用して下さい。

**例**
```sh
StopBGM
```

