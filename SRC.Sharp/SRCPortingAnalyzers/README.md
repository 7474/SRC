# SRCPortingAnalyzers

VB 版（`SRC/SRC_20121125`）からの移植で起きやすい誤りを、型の情報を使って見つける Roslyn のアナライザーです。SRCCore にだけ付け、規則に当たるとビルドが失敗します（`ci-passed` の前に気づく）。

元の SRC の動作を期待値にしたテスト（[判断の記録](../../docs/decisions/2026-10-15-vb-behavior-as-test-oracle.md)）は、テストを書いた箇所しか守れません。これまでに見つかった違い（7474/SRC#1446 の照らし合わせ、#1459〜#1495）の多くは同じ型の誤りだったので、型ごとに機械で洗い出し、新しく入らないようにします。

## 規則

| ID | 規則 | 直し方 |
|---|---|---|
| SRCP001 | 実数（`double`・`float`・`decimal`）を `(int)`・`(long)` などで整数へ変換しない。C# のキャストは切り捨てだが、VB の Integer・Long への代入と `CInt`・`CLng` は偶数への丸め | VB のコードを見て、意図に合う関数を使う。代入・`CInt`・`CLng` → `VBMath.CLng`、`Int` 関数 → `VBMath.Int`（負の数は小さいほうへ）、`Fix` 関数 → `VBMath.Fix`（0 に近いほうへ）、`\` の被除数 → `VBMath.IntDiv`。`Math.Round`・`Floor`・`Ceiling`・`Truncate` の結果のキャストは、丸め方を書いてあるので対象にしない |
| SRCP002 | `&&` と `\|\|` を括弧なしで混ぜない。VB の `And`・`Or` の優先順位は C# と同じだが、移植のときに `If`・`ElseIf` の連なりを 1 つの式にまとめて組み合わせを誤った例がある（7474/SRC#1490） | VB の条件と同じ組み合わせかを確かめてから括弧を付ける |

VB の演算子と関数の仕様:

- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/operator-precedence
- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/int-fix-functions
- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/type-conversion-functions

## 規則に当たらない書き方にできないとき

VB に当たるコードが無い（SRC# で足した処理）などで、切り捨てが正しい箇所は `VBMath.Fix` を使います。`#pragma warning disable` は使いません。

## 新しい規則を足すとき

`PortingAnalyzer.cs` に規則を足し、[テスト](../SRCPortingAnalyzersTests) に当たる例と当たらない例を書きます。既存のコードで当たる箇所を VB と突き合わせて直してから、同じ PR で有効にします。
