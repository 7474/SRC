# Graphics.bas

VB 版の `SRC/SRC_20121125/Graphics.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitFade | 125 | 209 | （見つからない） | 未確認 |  |
| Sub DoFade | 335 | 16 | （見つからない） | 未確認 |  |
| Sub FinishFade | 352 | 10 | （見つからない） | 未確認 |  |
| Sub MakeMask | 367 | 31 | （見つからない） | 未確認 |  |
| Sub GetImage | 400 | 36 | （見つからない） | 未確認 |  |
| Sub SetImage | 438 | 28 | （見つからない） | 未確認 |  |
| Sub ClearImage | 468 | 6 | `SRCRender/Compat/Controls.cs` PictureBox.ClearImage<br>`SRCSharpForm/Extensions/PictureBoxExtension.cs` PictureBoxExtension.ClearImage | 未確認 |  |
| Sub CopyImage | 476 | 8 | （見つからない） | 未確認 |  |
| Sub CreateMask | 486 | 20 | （見つからない） | 未確認 |  |
| Sub CreateImage | 508 | 16 | （見つからない） | 未確認 |  |
| Sub FadeInit | 526 | 11 | （見つからない） | 未確認 |  |
| Sub FadeInOut | 539 | 21 | （見つからない） | 未確認 |  |
| Sub WhiteInOut | 562 | 16 | （見つからない） | 未確認 |  |
| Sub Bright | 580 | 39 | （見つからない） | 未確認 |  |
| Sub Dark | 621 | 33 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Dark | 未確認 |  |
| Sub Monotone | 656 | 35 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Monotone | 未確認 |  |
| Sub Sepia | 693 | 35 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Sepia | 未確認 |  |
| Sub Sunset | 730 | 35 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Sunset | 未確認 |  |
| Sub Water | 767 | 35 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Water | 未確認 |  |
| Sub HReverse | 804 | 12 | （見つからない） | 未確認 |  |
| Sub VReverse | 818 | 12 | （見つからない） | 未確認 |  |
| Sub NegPosReverse | 832 | 33 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.NegPosReverse | 未確認 |  |
| Sub Silhouette | 867 | 27 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Silhouette | 未確認 |  |
| Sub Rotate | 897 | 113 | `SRCRender/ImageFilters.cs` ImageFilters.Rotate<br>`SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.Rotate | 未確認 |  |
| Sub ColorFilter | 1012 | 89 | `SRCSharpForm/Extensions/ImageExtension.cs` ImageExtension.ColorFilter | 未確認 |  |
