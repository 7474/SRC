# NowLoading.frm

VB 版の `SRC/SRC_20121125/NowLoading.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Progress | 73 | 6 | `SRCSharpForm/Forms/NowLoading.cs` frmNowLoading.Progress<br>`SRCRender/ScreenRenderer.cs` ScreenRenderer.DrawNowLoading | 修正済み | SRCSharpForm は ProgressBar で一致。SRCRender は「Now Loading...」の文字だけで進み具合のバーが無かった。NowLoading.frm と同じ配置（214×88 の灰色のダイアログ、左 16・上 56・183×13 の picBar）で、白地に &H800000 で ScaleWidth × Value \ Size まで両端を含めて塗る。Times New Roman 18 ポイントの太字は SRCRender の環境に無いので、明朝体の代替フォント（SerifFamily）の太字を Times New Roman の幅まで横に縮めて描く。Size が 0 のとき VB はゼロ除算の実行時エラーになるが、C# はバーを塗らない（#1536 で直した） |
