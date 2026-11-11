---
layout: default
title: CreateFolderコマンド
---
**CreateFolderコマンド**

フォルダを作成

**書式**

**CreateFolder** *folder*

**指定項目説明**

*folder*作成するフォルダ名

**解説**

新規フォルダ*folder*を作成します。フォルダのパスは[**Open**コマンド](Openコマンド.md)と同じようにシナリオが入っているフォルダを起点として計算されます。フォルダ名に「..\」や「../」を含めることは出来ません(エラーになります)。既にフォルダが存在する場合は何もしません。

**例**
```sh
#Logフォルダを作成
CreateFolder Log
```

