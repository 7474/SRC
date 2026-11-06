# PilotData.cls

VB 版の `SRC/SRC_20121125/PilotData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 76 | 5 | `SRCCore/Models/PilotData.cs` PilotData.PilotData | 一致 | コンストラクタ |
| Sub Class_Terminate | 83 | 37 | （見つからない） | SRC#で不要 | Collection の解放 |
| Property Get Nickname | 122 | 7 | `SRCCore/Models/PilotData.cs` PilotData.Nickname | 一致 |  |
| Property Let Nickname | 130 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Nickname | 一致 |  |
| Property Get KanaName | 135 | 14 | `SRCCore/Models/PilotData.cs` PilotData.KanaName | 一致 |  |
| Property Let KanaName | 150 | 3 | `SRCCore/Models/PilotData.cs` PilotData.KanaName | 一致 |  |
| Property Get Bitmap0 | 155 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap0 | 一致 |  |
| Property Get Bitmap | 159 | 7 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap | 一致 |  |
| Property Let Bitmap | 167 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap | 一致 |  |
| Sub AddSkill | 173 | 75 | `SRCCore/Models/PilotData.cs` PilotData.AddSkill | 一致 | Static の last_sname・last_sdata はインスタンスのフィールドにした。エリアスは RegisterAlias に分けている |
| Function Skill | 250 | 26 | `SRCCore/Models/PilotData.cs` PilotData.Skill | 修正済み | 同じ名前の特殊能力が重複して並んでいた（continue が内側の for にかかっていた）。VB の 32 個までの配列は、SRC# では個数を制限しない |
| Function IsSkillAvailable | 278 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsSkillAvailable | 一致 |  |
| Function SkillLevel | 294 | 23 | `SRCCore/Models/PilotData.cs` PilotData.SkillLevel | 一致 |  |
| Function SkillLevel2 | 319 | 23 | `SRCCore/Models/PilotData.cs` PilotData.SkillLevel2 | 一致 | Integer への代入は CLng（偶数丸め） |
| Function SkillData | 344 | 19 | `SRCCore/Models/PilotData.cs` PilotData.SkillData | 一致 |  |
| Function SkillName | 365 | 49 | `SRCCore/Models/PilotData.cs` PilotData.SkillName | 一致 |  |
| Function SkillType | 416 | 15 | `SRCCore/Models/PilotData.cs` PilotData.SkillType | 一致 |  |
| Sub AddSpecialPower | 434 | 9 | `SRCCore/Models/PilotData.cs` PilotData.AddSpecialPower | 一致 |  |
| Function CountSpecialPower | 445 | 9 | `SRCCore/Models/PilotData.cs` PilotData.CountSpecialPower | 一致 |  |
| Function SpecialPower | 456 | 14 | `SRCCore/Models/PilotData.cs` PilotData.SpecialPower | 修正済み | 見つからないとき null ではなく空文字列を返すようにした |
| Function IsSpecialPowerAvailable | 472 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsSpecialPowerAvailable | 修正済み | 同じスペシャルパワーが複数あるとき、VB は最初の定義の獲得レベルだけを見る。C# はどれか 1 つを満たせば True にしていた |
| Function SpecialPowerCost | 488 | 17 | `SRCCore/Models/PilotData.cs` PilotData.SpecialPowerCost | 一致 |  |
| Sub Clear | 508 | 33 | `SRCCore/Models/PilotData.cs` PilotData.Clear | 一致 |  |
| Sub AddFeature | 544 | 157 | `SRCCore/Models/PilotData.cs` PilotData.AddFeature | 違いあり | エリアスの展開は AliasData.ExpandFeature で、VB と一致。「XLvabc」のようにレベルが数値でないとき、VB は CDbl の実行時エラーで pilot.txt の読み込みを打ち切るが、C# は 0 として続ける（UnitData・ItemData にも同じ形がある。#1526） |
| Function CountFeature | 703 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountFeature | 一致 |  |
| Function Feature | 711 | 6 | `SRCCore/Models/PilotData.cs` PilotData.Feature | 一致 | 無いときは両方とも例外（VB エラー 5、C# NullReferenceException） |
| Function FeatureName | 719 | 14 | `SRCCore/Models/PilotData.cs` PilotData.FeatureName | 修正済み | 共通の FeatureNameWithLv（Level > 0。UnitData.cls と同じ）を使っていた。PilotData・ItemData の VB は `Level <> DEFAULT_LEVEL` なので、Lv0 や負のレベルにも「Lv」を付ける |
| Function FeatureLevel | 735 | 14 | `SRCCore/Models/PilotData.cs` PilotData.FeatureLevel | 一致 |  |
| Function FeatureData | 751 | 11 | `SRCCore/Models/PilotData.cs` PilotData.FeatureData | 一致 |  |
| Function IsFeatureAvailable | 764 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsFeatureAvailable | 一致 |  |
| Function Weapon | 781 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Weapon | 一致 |  |
| Function CountWeapon | 786 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountWeapon | 一致 |  |
| Function AddWeapon | 794 | 10 | `SRCCore/Models/PilotData.cs` PilotData.AddWeapon | 一致 |  |
| Function Ability | 807 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Ability | 一致 |  |
| Function CountAbility | 812 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountAbility | 一致 |  |
| Function AddAbility | 820 | 10 | `SRCCore/Models/PilotData.cs` PilotData.AddAbility | 一致 |  |
