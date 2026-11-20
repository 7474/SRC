---
layout: default
title: Debugコマンド
---
**Debugコマンド**

デバッグログの出力

**書式**

**Debug** *message*

**指定項目説明**

*message*出力するメッセージ

**解説**

*message*をログに出力します。

> **元の SRC との違い**: 元の SRC の Debug コマンドは、配布された Src.exe では何も出力しません（VB5 の Debug.Print は、コンパイルした実行ファイルでは何もしないため）。SRC# はログに出力します。（7474/SRC#1512）

ログの場所（SRC#Form・SRC#Avalonia はログのフォルダのファイル、SRC#Web はブラウザのコンソール、VS Code 拡張はデバッグコンソール）は [ログ](ログ.md) を見てください。

ログレベルは `[INF]`、出し元は `SRCCore.Event` として出力されます。これは `[DBG]` レベルは本体動作上のデバッグログ出力に使用されているためです。

**例**
```sh
Set i = 1
Debug デバッグメッセージ$i
```

出力内容
```
2026-10-17 21:00:00.000 [INF] SRCCore.Event: デバッグメッセージ1
```
