# GeneralLib.bas

VB 版の `SRC/SRC_20121125/GeneralLib.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub RndReset | 60 | 13 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.RndReset | 一致 | 4096 個の系列と RndIndex = 0 は同じ。系列は VB の Rnd ではなく .NET の Random で作るので、同じシードでも値は違う（セーブデータは SRC# の形式なので合わせない） |
| Function Dice | 75 | 18 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.Dice | 修正済み | Option「乱数系列非保存」のときは乱数系列を使わず、RndIndex を進めない分岐を足した（#1498）。GeneralLib は static なので、SRC のコンストラクタで IsRndHistoryUnsaved にその SRC のオプションを見る処理を入れる |
| Function LIndex | 96 | 53 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LIndex | 一致 | 区切りは半角スペースだけ |
| Function LLength | 151 | 29 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LLength | 一致 |  |
| Function LSplit | 183 | 35 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LSplit | 一致 | 配列が 0 始まりなのは SRC# の規約 |
| Function IsSpace | 220 | 11 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.IsSpace | 修正済み | 先頭の 1 文字だけを見る（空文字列か、タブ・CR・半角スペース・160 なら真）。IsNullOrWhiteSpace だったので、全角スペースや LF も真になっていた（#1498） |
| Sub LAppend | 233 | 13 | （なし） | SRC#で不要 | 元の SRC でも呼び出し元が無い |
| Function SearchList | 248 | 12 | （なし） | SRC#で不要 | 元の SRC でも呼び出し元が無い |
| Function ListIndex | 263 | 150 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListIndex | 修正済み | VB の走査どおりに書き直した。閉じ括弧が多すぎると、その要素と後ろの要素は空文字列になる（ToList が残りを別の要素にしていた）（#1498） |
| Function ListLength | 415 | 93 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListLength | 修正済み | 括弧・クォートが閉じていないとき、閉じ括弧が多すぎるときに -1 を返す（#1498。#1504 の手がかり）。直接の呼び出し元（Help・PlaySound・Print・Telop・LLength 関数・Unit.anime・SpecialPowerData）は、VB と同じく -1 を回数や == 1 の比較に使うだけなので落ちない |
| Function ListSplit | 511 | 100 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListSplit | 修正済み | 閉じていなければ -1。閉じ括弧が多すぎれば最後の要素を残り全体にする（VB の larray(n) = Mid$(list, begin)）。ListSplitStrict は ListSplit にまとめた（#1498） |
| Function ListTail | 613 | 98 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListTail | 修正済み | idx 番目の要素の先頭から元の文字列をそのまま返す（Mid$(list, i)）。要素の間の空白とタブを 1 つの半角スペースに詰めていたうえ、idx = 1 で空文字列を返していた（#1498） |
| Sub TrimString | 714 | 32 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.TrimString | 修正済み | C# に無かったので足した（タブ・半角スペース・全角スペースを取り除く）。いまは GetLine だけが使う（#1498） |
| Function InStr2 | 748 | 13 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.InStr2 | 一致 |  |
| Function StrToDbl | 764 | 5 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToDbl | 修正済み | 「NaN」「Infinity」「1e400」は VB の IsNumeric が偽なので 0 にする（double.TryParse は読んでしまう）（#1498） |
| Function StrToLng | 771 | 8 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToLng | 一致 | IsNumeric でなければ 0。CLng は偶数丸めで、あふれは 0（On Error） |
| Function StrToHiragana | 783 | 11 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToHiragana | 一致 | Strings.StrConv に任せる |
| Function MaxLng | 797 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MaxLng | 一致 |  |
| Function MinLng | 806 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MinLng | 一致 |  |
| Function MaxDbl | 815 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MaxDbl | 修正済み | VB と同じく a > b で比べる。Math.Max は NaN を返すが、VB は NaN と比べた偽で b を返す（#1498） |
| Function MinDbl | 824 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MinDbl | 修正済み | 同上（a < b）（#1498） |
| Function LeftPaddedString | 834 | 3 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LeftPaddedString | 修正済み | 幅を LenB(StrConv(buf, vbFromUnicode)) と同じ Shift_JIS のバイト数（Strings.LenB）にした。正規表現は半角カナの一部とタブを 2 と数えていた。StrWidth も LenB にした（#1498） |
| Function RightPaddedString | 839 | 3 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.RightPaddedString | 修正済み | 同上（#1498） |
| Function ReadIni | 845 | 19 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.GetItem | 修正済み | シナリオの Src.ini の値が空なら本体側から読む（GetPrivateProfileString は空の値で 0 を返す）。空を返していた（#1498） |
| Sub WriteIni | 867 | 28 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.SetItem | 一致 | シナリオにある項目はシナリオへ、無い項目は本体側へ書く。VB の ret > 1 で 1 文字の値を本体側に書く不具合には合わせない |
| Function ReplaceString | 899 | 34 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ReplaceString | 一致 | 長さが同じなら先頭から探し直して置き換え、違えば前から順に置き換える流れが同じ。置き換えた文字列を戻り値で返す（VB は ByRef の s1 を書き換えて True を返す）。s2 が空か s2 = s3 のとき VB は無限ループになるが、C# は置き換えずに返す（VB の不具合） |
| Function FileExists | 936 | 5 | `SRCCore/Filesystem/LocalFilesystem.cs` LocalFileSystem.FileExists | SRC#で不要 | ファイルの有無は IFileSystem が受け持つ（zip の中や大文字・小文字の解決も含む） |
| Sub GetLine | 948 | 35 | `SRCCore/Lib/SrcDataReader.cs` SrcDataReader.GetLine | 修正済み | VB の順にした: 「#」は空白を取り除く前の行の先頭で見る、コメントを除いてから TrimString、行末の「_」も空白を取り除く前の行で見る。先に Trim していたので「data // c」が末尾の空白付きで返り、「abc _ 」が結合されていた（#1498）。Ctrl+Z を取り除く・LF だけの改行も読むのは SRC# の便利さ |
| Function GetWinVersion | 986 | 17 | （なし） | SRC#で不要 | Windows の版。MCI の種類を選ぶためだけに使う |
| Function FormatNum | 1006 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.FormatNum | 一致 | .NET のカスタム書式も VB の Format$ と同じく 15 桁で丸める（0.1+0.2 → 0.3）。無限大の表記だけは違う |
| Function IsNumber | 1016 | 12 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.IsNumber | 一致 |  |
| Function GetClassBundle | 1037 | 35 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.GetClassBundle | 一致 |  |
| Function InStrNotNest | 1074 | 25 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.InStrNotNest | 一致 |  |
