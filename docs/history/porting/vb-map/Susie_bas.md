# Susie.bas

VB 版の `SRC/SRC_20121125/Susie.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Function LoadPicture2 | 28 | 83 | `SRCRender/ImageCache.cs` ImageCache.Load | 違いあり | 画像の読み込みは SkiaSharp の SKBitmap.Decode が受け持つので、Susie のプラグインと DLL の呼び出しは SRC#で不要。違うのは、壊れた画像・サポートしない形式のとき。VB は ErrorMessage で「…の読み込み中にエラーが発生しました。」などを出すが、C# はログに書いて null を返すだけで画面には出ない（#1536） |
