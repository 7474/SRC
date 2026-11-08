# Flash.bas

VB 版の `SRC/SRC_20121125/Flash.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub PlayFlash | 10 | 57 | （なし） | SRC#で不要 | Macromedia Flash Player（ActiveX）での再生。Flash Player はもう無い。SRC# では PlayFlash コマンドは対応していないコマンドとして、エラーを出して読み飛ばす |
| Sub ClearFlash | 69 | 9 | （なし） | SRC#で不要 | 同上（ClearFlash コマンド） |
| Sub GetEvent | 86 | 30 | （なし） | SRC#で不要 | Flash の FSCommand から呼ぶサブルーチンを実行する。Flash が無いので要らない |
