---
layout: default
title: RemoveFolderコマンド
---
** 内容はSRC2.2.33のものです **

**RemoveFolderコマンド**

フォルダを削除

**書式**

**RemoveFolder** *folder*

**指定項目説明**

*folder*削除するフォルダのフォルダ名

**解説**

指定したフォルダ*folder* を削除します。

フォルダのパスは[**Open**コマンド](Openコマンド.md)と同じようにシナリオが入っているフォルダを起点として計算されます。指定したフォルダが存在しなかった場合、コマンドはそのまま終了します。エラーは発生しません。

*folder* の中にあるファイルやフォルダも全て削除されます。

**例**
```sh
#Logフォルダを削除
RomoveFolder Log
```

