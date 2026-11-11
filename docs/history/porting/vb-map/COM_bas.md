# COM.bas

VB 版の `SRC/SRC_20121125/COM.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub OperateUnit | 13 | 2079 | `SRCCore/COM.cs` COM.OperateUnit | 修正済み | 重要なユニットの判定は 1 番目の効果が「回復」か。先制反撃の判定は Dice(16) を必ず振る（VB の Or は短絡しない）。武器の名前が変わっていたら攻撃しない（w=-1）。手動反撃ですべて選べずキャンセルしたら選択を抜ける（無限ループしていた）。等身大基準の援護防御の表示の空白。ターゲット選びの間接攻撃の判定で VB が w（最初は 0）を見る書き誤りは再現しない |
| Sub TryHyperMode | 2094 | 107 | `SRCCore/COM.cs` COM.TryHyperMode | 修正済み | 発動の気力を 100 + CInt(10*Lv)（偶数丸め）で比べる。追加パイロットの有無をデータの名前で調べる。メッセージは共通の PilotMassageIfDefined を使うので、GUI.Center と RefreshScreen が余分に呼ばれる（表示だけの違い） |
| Function TryBattleTransform | 2203 | 167 | `SRCCore/COM.cs` COM.TryBattleTransform | 修正済み | 追加パイロットの有無をデータの名前で調べる。パイロットデータが無いときは VB と同じくエラーにして終える |
| Function TryMoveTransform | 2372 | 185 | `SRCCore/COM.cs` COM.TryMoveTransform | 修正済み | 追加パイロットの扱いを TryBattleTransform と同じに。VB の `If SelectedY > ty`（ty はまだ 0）は書き間違いなので再現しない |
| Sub TryInstantAbility | 2559 | 113 | `SRCCore/COM.cs` COM.TryInstantAbility | 修正済み | SRC# で足した霊力消費の判定（29d28a3b）を外した。霊力が足りるかは IsAbilityAvailable が見る |
| Function TrySummonning | 2674 | 73 | `SRCCore/COM.cs` COM.TrySummonning | 修正済み | 合体技パートナーの設定を、VB と同じく使用イベントのあとにした |
| Function TryMapHealing | 2749 | 226 | `SRCCore/COM.cs` COM.TryMapHealing | 修正済み | VB と同じく最後のアビリティから調べる（評価が同じなら番号の大きいほう） |
| Function TryHealing | 2977 | 317 | `SRCCore/COM.cs` COM.TryHealing | 修正済み | 移動して使うかを VB の「射程 1 か Ｐ属性で、Ｑ属性なら不可」で決める。同じ回復量・EN のときはデータの使用回数で比べる |
| Function TryFix | 3296 | 237 | `SRCCore/COM.cs` COM.TryFix | 一致 | VB の `For k = 2 To u.FeatureData("修理不可")`（文字列を上限にする不具合）は再現しない |
| Function TryMapAttack | 3535 | 381 | `SRCCore/COM.cs` COM.TryMapAttack | 修正済み | ターゲット数の下限は、移動したかに関わらず通常武器が「移動前」で使えるかで決める（VB の MOD START マージ） |
| Function CountTargetInRange | 3918 | 95 | `SRCCore/COM.cs` COM.CountTargetInRange | 一致 |  |
| Sub TrySpecialPower | 4015 | 251 | `SRCCore/COM.cs` COM.TrySpecialPower | 修正済み | 最初に SelectedPilot = p を設定する。候補はパイロットの SpecialPower の名前のまま持つ（VB の slist と同じ） |
| Function IsSPEffectUseful | 4267 | 15 | `SRCCore/COM.cs` COM.IsSPEffectUseful | 一致 |  |
| Function SelectWeapon | 4287 | 1025 | `SRCCore/COM.cs` COM.SelectWeapon | 修正済み | 衰・滅のレベルを VB の CInt と同じく偶数丸めしてから比べる。-1 で早く返るとき C# は out の max_prob・max_dmg を 0 にする（VB は前の周回の値が残る不具合なので再現しない） |
| Function SelectDefense | 5314 | 193 | `SRCCore/COM.cs` COM.SelectDefense | 一致 | w が無効なときに "" を返すガードは C# だけ（VB は実行時エラー） |
| Function IsAbleToCounterAttack | 5509 | 101 | `SRCCore/COM.cs` COM.IsAbleToCounterAttack<br>`SRCCore/Units/UnitWeapon.cs` UnitWeapon.CanUseFor | 一致 | UnitWeapon.CanUseFor(Counter) に移してある |
| Function SearchNearestEnemy | 5612 | 65 | `SRCCore/COM.cs` COM.SearchNearestEnemy | 一致 |  |
| Function DistanceFromNearestEnemy | 5679 | 11 | `SRCCore/COM.cs` COM.DistanceFromNearestEnemy | 一致 |  |
