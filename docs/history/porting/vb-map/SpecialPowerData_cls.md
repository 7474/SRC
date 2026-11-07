# SpecialPowerData.cls

VB 版の `SRC/SRC_20121125/SpecialPowerData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 48 | 5 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData | 一致 | VB は 3 つの配列を ReDim (0) する。C# は効果の List を空で作る |
| Sub SetEffect | 56 | 106 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.SetEffect | 修正済み | VB は ReDim で効果を作り直すのに、C# は追加していたので置き換えにした。効果名だけのときの効果データは VB と同じく "" にした（#1498）。CDbl に失敗するとき、VB は Load の On Error でデータエラーになって終了するが、C# の Conversions.ToDouble は 0 を返す（合わせていない） |
| Function CountEffect | 165 | 3 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.CountEffect | 一致 |  |
| Function EffectType | 170 | 3 | `SRCCore/Models/SpecialPowerEffect.cs` SpecialPowerEffect.strEffectType | 一致 | 番号ではなく Effects の要素で引く |
| Function EffectLevel | 175 | 3 | `SRCCore/Models/SpecialPowerEffect.cs` SpecialPowerEffect.dblEffectLevel | 一致 | 同上 |
| Function EffectData | 180 | 3 | `SRCCore/Models/SpecialPowerEffect.cs` SpecialPowerEffect.strEffectData | 一致 | 同上 |
| Function IsEffectAvailable | 185 | 17 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.IsEffectAvailable | 一致 | 「スペシャルパワー」効果を再帰で見る順序も同じ |
| Function Useful | 206 | 109 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.Useful | 一致 |  |
| Function Effective | 318 | 252 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.Effective | 修正済み | 状態回復の「属性使用不能」は、VB と同じく名前が 6 文字より長いものだけを見るようにした（それまでは「属性使用不能」そのものも対象で、文化依存の EndsWith を使っていた）（#1498）。ＳＰ回復は AllPilots で VB の順と同じ。気力増加は MainPilots を使い MainPilot の MaxMorale で比べる点も同じ |
| Sub Execute | 574 | 97 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.Execute | 修正済み | 「全味方」で Sleep 300 が列ごとのループの中にあったので、VB と同じくループのあとで一度だけにした（#1498） |
| Function Apply | 675 | 1017 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.Apply | 修正済み | ＳＰ回復で、サブ・サポート・追加サポートのメッセージを VB と同じくＳＰが変わったときだけ出す。状態回復の「属性使用不能」は Effective と同じく 6 文字より長い名前だけにした（#1498）。ほかの効果は分岐・丸め（CLng・IntDiv・MaxHP\4）を確かめて一致。合わせないもの: 状態回復で「〜属性使用不能」を消すループは VB では範囲外の Condition(j) でエラー 9 になる（VB の不具合）ので、C# はすべて消す。復活で召喚者に戻す処理（#627）は SRC# で足したもの。偵察の msg は効果ごとに作る（VB は関数全体で 1 つの変数） |
| Function CountTarget | 1696 | 106 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.CountTarget | 一致 |  |
| Function PlayAnimation | 1804 | 202 | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData.PlayAnimation | 一致 | FindNormalLabel の見つからないときの戻り値（VB は 0、C# は -1）に合わせて判定している。エラーのときは VB と同じく色を戻さず False を返す。picMain.Refresh は GUI.RefreshScreen・GUI.UpdateScreen が受け持つ |
