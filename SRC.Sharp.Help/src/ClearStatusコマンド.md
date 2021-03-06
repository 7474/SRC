---
layout: default
title: ClearStatusコマンド
---
** 内容はSRC2.2.33のものです **

**ClearStatusコマンド**

ユニットの状態を回復

**書式**

**ClearStatus** [*unit*] *status*

**指定項目説明**

*unit*回復させるユニットの[メインパイロット名](メインパイロット名.md)または[ユニットＩＤ](ユニットＩＤ.md)（省略可）

*status*回復させる特殊状態

**解説**

指定した*unit* の特殊状態*status* を回復させます。指定可能な特殊状態に関しては[**SetStatus**コマンド](SetStatusコマンド.md)を参照してください。

**例**
```sh
ClearStatus ジェイ 行動不能
特殊能力の付加を解除するのにも**ClearStatus**コマンドを使います。*status*に「*特殊能力名*付加」を指定してください。特殊能力のレベル指定は不要です。
```

**例**

ClearStatus ＨＰ回復付加
