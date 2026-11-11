# Susie.bas

VB 版の `SRC/SRC_20121125/Susie.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Function LoadPicture2 | 28 | 83 | `SRCRender/ImageCache.cs` ImageCache.Load<br>`SRCRender/ScreenGUI.draw.cs` ScreenGUI.ReportPictureError | 修正済み | 画像の読み込みは SkiaSharp の SKBitmap.Decode が受け持つので、Susie のプラグインと DLL の呼び出しは SRC#で不要。壊れた画像・サポートしない形式のとき、C# はログに書いて null を返すだけで画面に出していなかった。ImageCache.Load が読めなかったこと（DecodeFailed）と見つかったパスを返し、DrawPicture が VB と同じ文言（「…の読み込み中にエラーが発生しました。」「…の画像フォーマットはサポートされていません。」）を ErrorMessage で出すようにした。.bmp・.jpg・.gif・.png 以外は中身によらずサポートしない形式とする。VB は失敗した画像も PicBuf に名前を残すので、同じ画像のメッセージは 1 度だけ（C# は名前ごとに 1 度）。文言のファイル名は SRC# が開くパス（シナリオなどのフォルダからの相対パス）で、VB は絶対パス。PNG の Susie プラグインが無いときの文言は SRC#に当たる場合が無い。VB は失敗しても空の PicBuf を描いて続けるが、C# は描かずに False を返す（#1536 で直した） |
