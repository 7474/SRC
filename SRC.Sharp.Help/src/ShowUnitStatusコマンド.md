---
layout: default
title: ShowUnitStatusコマンド
---
** 内容はSRC2.2.33のものです **

**ShowUnitStatusコマンド**

ユニットを[ステータスウィンドウ](ステータスウインドウ.md)に表示

**書式**

**ShowUnitStatus** [*unit*]

**指定項目説明**

*unit*ステータスウィンドウに表示するユニットのメインパイロット名、ユニット名称または[ユニットＩＤ](ユニットＩＤ.md)（省略可）

表示を終了する場合は「終了」

**解説**

*unit* の情報をステータスウィンドウに表示します。ステータスウィンドウをクリアし、表示を終了したい場合は*unit* に「終了」を指定して下さい。

**例**
```sh
#相手ユニットの情報を表示
ShowUnitStatus 相手ユニットＩＤ
```

