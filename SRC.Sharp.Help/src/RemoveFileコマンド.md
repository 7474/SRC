---
layout: default
title: RemoveFileコマンド
---
** 内容はSRC2.2.33のものです **

**RemoveFileコマンド**

ファイルを削除

**書式**

**RemoveFile** *file*

**指定項目説明**

*file*削除するファイルのファイル名

**解説**

指定したファイル*file* を削除します。ファイルのパスは[**Open**コマンド](Openコマンド.md)と同じようにシナリオが入っているフォルダを起点として計算されます。指定したファイルが存在しなかった場合、コマンドはそのまま終了します。エラーは発生しません。

**例**
```sh
#プレイヤーが指定したファイルを削除する
Talk システム
消去するデータファイルを選んでください
End
fname = LoadFileDialog("データファイル", "dat")
If fname <> "" Then
RemoveFile fname
EndIf
```

