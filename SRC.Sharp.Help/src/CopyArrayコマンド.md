---
layout: default
title: CopyArrayコマンド
---
** 内容はSRC2.2.33のものです **

**CopyArrayコマンド**

配列をコピー

**書式**

**CopyArray** *var1 var2*

**指定項目説明**

*var1*コピー元の配列名

*var2*コピー先の配列名

**解説**

*var1* の内容を*var2* にコピーします。コピー先の配列*var2* が既に作成済みの場合、元の内容は消去されます。

**例**
```sh
#配列aの内容をtmpに保存する
CopyArray a tmp
```

