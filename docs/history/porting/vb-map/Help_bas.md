# Help.bas

VB 版の `SRC/SRC_20121125/Help.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub SkillHelp | 13 | 35 | `SRCCore/Help.cs` Help.SkillHelp | 一致 | 名前の調べ方（数値なら番号、そうでなければ Lv の前の部分）と表示の手順が同じ |
| Function SkillHelpMessage | 50 | 741 | `SRCCore/Help.cs` Help.SkillHelpMessage | 修正済み | 解説が無いときは VB と同じく "" を返す（null を返していた）（#1498）。文言はすべての Case で VB と同じ。丸めも同じ（CLng は VBMath.CLng、100 * slevel \ 16 は VBMath.IntDiv、p.Level \ 8 と MaxSP \ 5 は int の割り算、気力上限・気力下限の MaxLng は CLng を通す）。ハンターの ElseIf 3 > 2（いつも「、」が付く）も VB のまま |
| Sub FeatureHelp | 794 | 33 | `SRCCore/Help.cs` Help.FeatureHelp | 一致 | 手がかり（.AllFeatureName(findex)）を確かめた。VB の AllFeatureName は見つからないと findex を返すが、続く FeatureHelpMessage の .AllFeature(findex) にエラー処理が無く Collection のエラーになるので、VB でも動かない。呼び出し元（Command の一覧・Info）はある番号か名前しか渡さない |
| Function FeatureHelpMessage | 829 | 2456 | `SRCCore/Help.cs` Help.FeatureHelpMessage | 修正済み | ＥＣＭの文言を VB の「同時に相手の…能力の効果を無効化。」に戻した（C# は「能力のレベルを、この能力のレベル分だけ下げる。」だった）。解説が無いときは "" を返す（#1498）。ほかの文字列・Format の引数・If と Case の条件は機械的に突き合わせて同じ。フィールド・アーマー・レジスト・攻撃回避のレベルの合計は、C# が自分も含めて Sum するので VB の i <> fid のものを足すのと同じ値。合わせないもの: 支配で PDList.IsDefined(LIndex(fdata, 2)) と 2 番目を固定で見る VB の書き間違い（C# は i 番目を見る）。修理不可の For i = 2 To fdata は VB では型の不一致（13）だが、C# は 0 回のループ |
| Function AttributeName | 3287 | 306 | `SRCCore/Help.cs` Help.AttributeName | 修正済み | どの Case にも当たらないときは VB と同じく "" を返す（null を返していた）（#1498）。Case "反" と 2 回目の Case "銃" が空なのも VB のまま |
| Sub AttributeHelp | 3595 | 28 | `SRCCore/Help.cs` Help.AttributeHelp | 一致 | 名前に「レベル」と全角にした数字を付けるところも同じ |
| Function AttributeHelpMessage | 3625 | 922 | `SRCCore/Help.cs` Help.AttributeHelpMessage | 修正済み | (1) 「変」は、VB と同じくすべての変形技から、データの 1 番目が武器名のものを探す（最初の変形技しか見ず、変形技が 2 つ以上あると変形先が空になっていた） (2) 「合」は、データの無い合体技でも例外にせず LIndex で読む (3) 解説の無い属性では msg を "" のままにした（等身大基準のときに NullReferenceException になっていた）（#1498）。Ｒ属性で msg を「通常の半分」で上書きする VB の書き方もそのまま。属性レベルの CDbl が数値でないとき、VB はエラー 13、C# は 0（合わせていない） |
