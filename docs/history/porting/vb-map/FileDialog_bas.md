# FileDialog.bas

VB 版の `SRC/SRC_20121125/FileDialog.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Function LoadFileDialog | 44 | 57 | `SRCCore/UIInterface/IGUI.cs` IGUI.SelectLoadFile | SRC#で不要 | GetOpenFileName を使う Win32 のダイアログなので GUI が受け持つ。呼び出し元（Expression の LoadFileDialog 関数）は Expression の項目で突き合わせ済み |
| Function SaveFileDialog | 103 | 57 | `SRCCore/UIInterface/IGUI.cs` IGUI.SelectSaveFile | SRC#で不要 | 同上（GetSaveFileName）。VB は ftype3 のフィルタに fsuffix2 を使う書き間違いがある |
