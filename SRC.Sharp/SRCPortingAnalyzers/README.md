# SRCPortingAnalyzers

VB 版（`SRC/SRC_20121125`）からの移植で起きやすい誤りを、型の情報を使って見つける Roslyn のアナライザーです。SRCCore にだけ付け、規則に当たるとビルドが失敗します（`ci-passed` の前に気づく）。

元の SRC の動作を期待値にしたテスト（[判断の記録](../../docs/decisions/2026-10-15-vb-behavior-as-test-oracle.md)）は、テストを書いた箇所しか守れません。これまでに見つかった違い（7474/SRC#1446 の照らし合わせ、#1459〜#1495）の多くは同じ型の誤りだったので、型ごとに機械で洗い出し、新しく入らないようにします。

## 規則

| ID | 規則 | 直し方 |
|---|---|---|
| SRCP001 | 実数（`double`・`float`・`decimal`）を `(int)`・`(long)` などで整数へ変換しない。C# のキャストは切り捨てだが、VB の Integer・Long への代入と `CInt`・`CLng` は偶数への丸め | VB のコードを見て、意図に合う関数を使う。代入・`CInt`・`CLng` → `VBMath.CLng`、`Int` 関数 → `VBMath.Int`（負の数は小さいほうへ）、`Fix` 関数 → `VBMath.Fix`（0 に近いほうへ）、`\` の被除数 → `VBMath.IntDiv`。`Math.Round`・`Floor`・`Ceiling`・`Truncate` の結果のキャストは、丸め方を書いてあるので対象にしない |
| SRCP002 | `&&` と `\|\|` を括弧なしで混ぜない。VB の `And`・`Or` の優先順位は C# と同じだが、移植のときに `If`・`ElseIf` の連なりを 1 つの式にまとめて組み合わせを誤った例がある（7474/SRC#1490） | VB の条件と同じ組み合わせかを確かめてから括弧を付ける |
| SRCP003 | `string` のフィールドと自動実装のプロパティに初期値を書く。VB の `String` の変数・クラスのメンバーは `""` で始まるが、C# は `null` で始まる。システム変数「フェイズ」「選択」が null を返していた（7474/SRC#1543）。対象は VB のモジュール・クラスを移した型のある名前空間（`SRCCore` の直下と `Models`・`Units`・`Pilots`・`Items`・`Events`・`Commands`・`CmdDatas`・`Maps`・`Expressions`）のクラス。`const`・本体のあるプロパティ・すべてのコンストラクターが代入しているものは対象にしない | VB の宣言を見て `= ""` を付ける。VB のフォームに書いてある値（`Caption` など）で始まるものは、その値を書く |
| SRCP004 | `GeneralLib.ToList` を使わない。VB の `ListSplit`・`ListIndex`・`ListLength` と違い、閉じていない括弧を区別せず（`ListLength` は -1 を返す）、閉じ括弧が多すぎる箇所の後ろを別の要素にする。ForEach・SetEffect など 7 か所で結果が違っていた（7474/SRC#1534） | `GeneralLib.ListSplit`・`ListIndex`・`ListLength`（VB と同じ結果）を使う。`LIndex`・`LLength` は VB の `LIndex`・`LLength` |

VB の演算子と関数の仕様:

- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/operator-precedence
- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/int-fix-functions
- https://learn.microsoft.com/office/vba/language/reference/user-interface-help/type-conversion-functions

## 規則に当たらない書き方にできないとき

VB に当たるコードが無い（SRC# で足した処理）などで、切り捨てが正しい箇所は `VBMath.Fix` を使います。`#pragma warning disable` は使いません。

SRCP003 で、`null` に意味がある（VB のオブジェクトの参照を ID や名前で持ち、`null` が `Nothing` に当たる、SRC# のセーブデータの形など）ものは、メンバーか型に `SuppressMessage` を付けて、`Justification` に理由を書きます。

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "VB の Unit（オブジェクトの参照。未搭乗は Nothing）を ID で持つ。null は Nothing に当たる")]
[JsonProperty]
public string UnitId { get; set; }
```

## アナライザーにしなかった型

7474/SRC#1498 の突き合わせで繰り返し見つかった誤りのうち、次のものはアナライザーの規則にしていません（7474/SRC#1555）。

- 名前で探す `FirstOrDefault`（使い切ったアイテムを、VB が番号で決めているのに名前の一致で探していた。7474/SRC#1508）: SRCCore に `FirstOrDefault(x => x.Name == ...)` の形は約 50 か所あるが、VB も `For i = 1 To .CountAbility: If .Ability(i).Name = aname Then Exit For` のように名前で探す箇所がほとんどで、形からは誤りと区別できない。誤りは「VB がどの番号を引いているか」で、VB と突き合わせないと分からない
- 新しい `TODO`・`XXX`・コメントアウトされたコード: Roslyn のアナライザーにはせず、CI（`port-map-check`）の `python3 tools/vb_port_map.py --check` で、[一覧](../../docs/history/porting/vb-map/todo.md)に状態が「未確認」の行があると落とす。一覧を作る仕組みと、判断（状態とメモ）を書く場所がすでにあり、アナライザーでは判断を書く場所を別に作ることになるため。足したら `python3 tools/vb_port_map.py` で一覧を作り直し、直すか、状態とメモを書く

## 新しい規則を足すとき

`PortingAnalyzer.cs` に規則を足し、[テスト](../SRCPortingAnalyzersTests) に当たる例と当たらない例を書きます。既存のコードで当たる箇所を VB と突き合わせて直してから、同じ PR で有効にします。
