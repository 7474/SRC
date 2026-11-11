# Title.frm

VB 版の `SRC/SRC_20121125/Title.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Form_Load | 106 | 5 | `SRCRender/ScreenRenderer.cs` ScreenRenderer.DrawTitle<br>`SRCSharpForm/Forms/Title.cs` frmTitle.frmTitle_Load | SRC#で不要 | VB は VB の版を出す。SRC# は SRC# の版を出す（docs/versioning.md） |
