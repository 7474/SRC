# ToolTip.frm

VB 版の `SRC/SRC_20121125/ToolTip.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_Load | 48 | 6 | （なし） | SRC#で不要 | SetWindowPos だけ |
| Sub ShowToolTip | 56 | 44 | `SRCRender/ScreenRenderer.cs` ScreenRenderer.DrawTooltip | 修正済み | 大きさ（文字＋6・＋4）、位置（カーソルの 24 ピクセル下）、枠（上・左が RGB(200,200,200)、下・右が黒）、文字の位置 (3, 2) を合わせた（#1498）。画面の中に収めるのは SRC# の都合で残す。SRCSharpForm は WinForms の ToolTip のまま |
