# PilotData.cls

VB 版の `SRC/SRC_20121125/PilotData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 76 | 5 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 83 | 37 | （見つからない） | 未確認 |  |
| Property Get Nickname | 122 | 7 | `SRCCore/Models/PilotData.cs` PilotData.Nickname | 未確認 |  |
| Property Let Nickname | 130 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Nickname | 未確認 |  |
| Property Get KanaName | 135 | 14 | `SRCCore/Models/PilotData.cs` PilotData.KanaName | 未確認 |  |
| Property Let KanaName | 150 | 3 | `SRCCore/Models/PilotData.cs` PilotData.KanaName | 未確認 |  |
| Property Get Bitmap0 | 155 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap0 | 未確認 |  |
| Property Get Bitmap | 159 | 7 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap | 未確認 |  |
| Property Let Bitmap | 167 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Bitmap | 未確認 |  |
| Sub AddSkill | 173 | 75 | `SRCCore/Models/PilotData.cs` PilotData.AddSkill | 未確認 |  |
| Function Skill | 250 | 26 | `SRCCore/Models/PilotData.cs` PilotData.Skill | 未確認 |  |
| Function IsSkillAvailable | 278 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsSkillAvailable | 未確認 |  |
| Function SkillLevel | 294 | 23 | `SRCCore/Models/PilotData.cs` PilotData.SkillLevel | 未確認 |  |
| Function SkillLevel2 | 319 | 23 | `SRCCore/Models/PilotData.cs` PilotData.SkillLevel2 | 未確認 |  |
| Function SkillData | 344 | 19 | `SRCCore/Models/PilotData.cs` PilotData.SkillData | 未確認 |  |
| Function SkillName | 365 | 49 | `SRCCore/Models/PilotData.cs` PilotData.SkillName | 未確認 |  |
| Function SkillType | 416 | 15 | `SRCCore/Models/PilotData.cs` PilotData.SkillType | 未確認 |  |
| Sub AddSpecialPower | 434 | 9 | `SRCCore/Models/PilotData.cs` PilotData.AddSpecialPower | 未確認 |  |
| Function CountSpecialPower | 445 | 9 | `SRCCore/Models/PilotData.cs` PilotData.CountSpecialPower | 未確認 |  |
| Function SpecialPower | 456 | 14 | `SRCCore/Models/PilotData.cs` PilotData.SpecialPower | 未確認 |  |
| Function IsSpecialPowerAvailable | 472 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsSpecialPowerAvailable | 未確認 |  |
| Function SpecialPowerCost | 488 | 17 | `SRCCore/Models/PilotData.cs` PilotData.SpecialPowerCost | 未確認 |  |
| Sub Clear | 508 | 33 | `SRCCore/Models/PilotData.cs` PilotData.Clear | 未確認 |  |
| Sub AddFeature | 544 | 157 | `SRCCore/Models/PilotData.cs` PilotData.AddFeature | 未確認 |  |
| Function CountFeature | 703 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountFeature | 未確認 |  |
| Function Feature | 711 | 6 | `SRCCore/Models/PilotData.cs` PilotData.Feature | 未確認 |  |
| Function FeatureName | 719 | 14 | `SRCCore/Models/PilotData.cs` PilotData.FeatureName | 未確認 |  |
| Function FeatureLevel | 735 | 14 | `SRCCore/Models/PilotData.cs` PilotData.FeatureLevel | 未確認 |  |
| Function FeatureData | 751 | 11 | `SRCCore/Models/PilotData.cs` PilotData.FeatureData | 未確認 |  |
| Function IsFeatureAvailable | 764 | 14 | `SRCCore/Models/PilotData.cs` PilotData.IsFeatureAvailable | 未確認 |  |
| Function Weapon | 781 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Weapon | 未確認 |  |
| Function CountWeapon | 786 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountWeapon | 未確認 |  |
| Function AddWeapon | 794 | 10 | `SRCCore/Models/PilotData.cs` PilotData.AddWeapon | 未確認 |  |
| Function Ability | 807 | 3 | `SRCCore/Models/PilotData.cs` PilotData.Ability | 未確認 |  |
| Function CountAbility | 812 | 6 | `SRCCore/Models/PilotData.cs` PilotData.CountAbility | 未確認 |  |
| Function AddAbility | 820 | 10 | `SRCCore/Models/PilotData.cs` PilotData.AddAbility | 未確認 |  |
