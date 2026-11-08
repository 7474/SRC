# Graphics.bas

VB 版の `SRC/SRC_20121125/Graphics.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitFade | 125 | 209 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.TransionScrean<br>`SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.TransionScrean | 一致 | FadeIn・FadeOut・WhiteIn・WhiteOut の準備。色の式（黒は 色 * i \ times、白は 色 + (255 - 色) * (times - i) \ times）は、塗る色を半透明で重ねる式に当たる。VB は画像を 256 色に減色しパレットを変えてフェードする（GDI の DIB のパレットによるもの。合わせない）。重ねる式との差は丸めの 1 段階ほど |
| Sub DoFade | 335 | 16 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.TransionScrean | 一致 | 範囲外の times で抜けるのは TransionScrean の 0〜255 の切り詰めに当たる。StretchDIBits は SRC#で不要 |
| Sub FinishFade | 352 | 10 | `SRCRender/ScreenGUI.map.cs` ScreenGUI.TransionScrean | SRC#で不要 | GDI の DC・DIBSection の解放。C# は using で解放する |
| Sub MakeMask | 367 | 31 | `SRCRender/ImageFilters.cs` ImageFilters.MakeTransparent | 一致 | 透過色に当たる画素を抜く。画像がアルファで透過を持つとき（PNG）は色では抜かずにアルファを使う（SRC# の利便性） |
| Sub GetImage | 400 | 36 | （なし） | SRC#で不要 | PictureBox から DIB の画素を取り出す Win32 API の処理。C# は SKBitmap.Pixels を使う |
| Sub SetImage | 438 | 28 | （なし） | SRC#で不要 | 同上（書き戻し） |
| Sub ClearImage | 468 | 6 | （なし） | SRC#で不要 | 画素のバッファの解放。C# は Dispose する |
| Sub CopyImage | 476 | 8 | `SRCRender/ImageFilters.cs` ImageFilters.Rotate | 一致 | Rotate の中で元の画素を写しておく処理に当たる |
| Sub CreateMask | 486 | 20 | （なし） | SRC#で不要 | VB 版のどこからも呼ばれていない |
| Sub CreateImage | 508 | 16 | （なし） | SRC#で不要 | VB 版のどこからも呼ばれていない |
| Sub FadeInit | 526 | 11 | （なし） | SRC#で不要 | VB 版のどこからも呼ばれていない |
| Sub FadeInOut | 539 | 21 | （なし） | SRC#で不要 | VB 版のどこからも呼ばれていない |
| Sub WhiteInOut | 562 | 16 | （なし） | SRC#で不要 | VB 版のどこからも呼ばれていない |
| Sub Bright | 580 | 39 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyBright | 修正済み | 各色に 80 を足して 255 で止める。透過のときは 255 の成分を残し、ほかは 254 で止めるようにした（透過でも 255 まで上がっていた）（#1498）。SRCSharpForm には明の処理が無い |
| Sub Dark | 621 | 33 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyDark<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Dark | 修正済み | カラーマトリクスの 0.5 倍（丸めの違いで 1 段階ずれる）を、VB と同じ \ 2 にした（#1498）。SRCSharpForm は GDI+ のカラーマトリクスのまま |
| Sub Monotone | 656 | 35 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyMonotone<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Monotone | 修正済み | 係数が違っていた（0.3086・0.6094・0.0820）。VB と同じ 0.299・0.587・0.114 にし、Long への代入と同じく銀行丸めにした（#1498）。SRCSharpForm は直していない |
| Sub Sepia | 693 | 35 | `SRCRender/ImageFilters.cs` ImageFilters.ApplySepia<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Sepia | 修正済み | 輝度を Long に銀行丸めしてから、MinLng(1.1 倍, 255)・0.9 倍・0.7 倍を Byte に銀行丸めする（行列で一度に掛ける方法では丸めがずれていた）（#1498）。SRCSharpForm は直していない |
| Sub Sunset | 730 | 35 | `SRCRender/ImageFilters.cs` ImageFilters.ApplySunset<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Sunset | 修正済み | 同上（赤 = MinLng(0.2R + 1.3Y, 255)、緑 = 0.2G + 0.4Y、青 = 0.2B + 0.2Y）（#1498）。SRCSharpForm は直していない |
| Sub Water | 767 | 35 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyWater<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Water | 修正済み | 同上（0.6Y・0.8Y・Y）（#1498）。SRCSharpForm は直していない |
| Sub HReverse | 804 | 12 | `SRCRender/ImageFilters.cs` ImageFilters.Flip | 一致 | 左右反転。画素の入れ替えで書き直したが、前の実装でも同じ結果だった（テストで確かめた） |
| Sub VReverse | 818 | 12 | `SRCRender/ImageFilters.cs` ImageFilters.Flip | 一致 | 上下反転。同上 |
| Sub NegPosReverse | 832 | 33 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyNegPos<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.NegPosReverse | 一致 | 255 - 色。画素ごとの計算に書き直した（前の実装も同じ結果） |
| Sub Silhouette | 867 | 27 | `SRCRender/ImageFilters.cs` ImageFilters.ApplySilhouette<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Silhouette | 修正済み | 「真っ白だけを白、ほかを黒」にしていた。VB と同じく、背景色（PaintPicture の色の指定。既定は白）と同じ画素を白に、ほかを黒にした（#1498）。SRCSharpForm は直していない |
| Sub Rotate | 897 | 113 | `SRCRender/ImageFilters.cs` ImageFilters.Rotate<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Rotate | 修正済み | VB と同じく回転前の位置を三角関数で求め（円周率は 3.14159265）、CLng で丸めた画素を取る。正方形でない 90 度・270 度も一般の式で回す（Skia の回転だった）。透過しないときに空いた部分は、使われていない ScreenGUI.BGColor ではなく PaintPicture の背景色の指定（既定は白）で塗る（#1498）。SRCSharpForm は直していない |
| Sub ColorFilter | 1012 | 89 | `SRCRender/ImageFilters.cs` ImageFilters.ApplyColorFilter<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.ColorFilter | 修正済み | tratio = MinLng(MaxLng(CLng(100 * trans_par), 0), 100) にし、0 なら何もせず、ほかは (色 * (100 - tratio) + 描画色 * tratio) \ 100。透過で描画色が背景色と同じときは 255 の成分を 254 にずらす。透過率は Double のまま渡す（#1498）。SRCSharpForm は直していない |
