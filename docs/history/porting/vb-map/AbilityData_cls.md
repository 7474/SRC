# AbilityData.cls

VB 版の `SRC/SRC_20121125/AbilityData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 42 | 10 | （なし） | SRC#で不要 | Collection の解放 |
| Function Nickname | 54 | 7 | `SRCCore/Models/AbilityData.cs` AbilityData.Nickname | 一致 |  |
| Sub SetEffect | 63 | 120 | `SRCCore/Models/AbilityData.cs` AbilityData.SetEffect | 修正済み | 「Lv」の後が数値でないとき、VB は CDbl の実行時エラーになるが、C# は 0 として読んでいた。「回復=」のようにデータが空のときも、VB は Asc("") の実行時エラーになるが、C# は IndexOutOfRangeException が Load の外へ出ていた。どちらも LoadAbility で原因を書かないデータエラーとして打ち切るようにした（#1498） |
| Function NewAbilityEffect | 184 | 4 | （なし） | SRC#で不要 | new AbilityEffect() で済む |
| Function CountEffect | 190 | 3 | `SRCCore/Models/AbilityData.cs` AbilityData.CountEffect | 一致 |  |
| Function EffectType | 195 | 3 | `SRCCore/Models/AbilityEffect.cs` AbilityEffect.EffectType | 一致 |  |
| Function EffectLevel | 200 | 3 | `SRCCore/Models/AbilityEffect.cs` AbilityEffect.Level | 一致 | 効果のオブジェクトのフィールドを直接読む |
| Function EffectData | 205 | 3 | `SRCCore/Models/AbilityEffect.cs` AbilityEffect.Data | 一致 | 同上 |
| Function EffectName | 210 | 403 | `SRCCore/Models/AbilityData.cs` AbilityData.EffectName | 修正済み | 「最低でも１ターン持続」の MaxLng は Long の引数なので、VB はここでターン数を整数に丸める（偶数丸め）。C# は Math.Max で端数を残していたので、ターン数が 10.4 だと「(10ターン)」を出さず、1.4 体の召喚を「１体召喚」と出していた（#1498）。SelectedUnit が無いとき VB は実行時エラーになるが、C# は補正をしないで続ける |
| Function WeaponType | 615 | 11 | `SRCCore/Models/AbilityData.cs` AbilityData.WeaponType | 一致 |  |
| Function IsItem | 628 | 10 | `SRCCore/Models/AbilityData.cs` AbilityData.IsItem | 一致 |  |
