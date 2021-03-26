** 内容はSRC2.2.33のものです **

**RenameFileコマンド**

ファイル名を変更

**書式**

**RemoveFile** *file1 file2*

**指定項目説明**

*file1*ファイル名を変更するファイル

*file2*変更先のファイル名

**解説**

指定したファイル*file1* のファイル名を*file2* に変更します。ファイルのパスは[**Open**コマンド](Openコマンド.md)と同じようにシナリオが入っているフォルダを起点として計算されます。

**例**
```sh
RenameFile data.txt data.txt.bak
```

