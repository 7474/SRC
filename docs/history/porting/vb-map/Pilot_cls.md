# Pilot.cls

VB 版の `SRC/SRC_20121125/Pilot.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 99 | 13 | （見つからない） | 未確認 |  |
| Property Get Name | 114 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Name | 未確認 |  |
| Property Let Name | 118 | 4 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Name | 未確認 |  |
| Property Get Nickname0 | 124 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Nickname0 | 未確認 |  |
| Property Get Nickname | 128 | 69 | `SRCCore/Expressions/Functions/Other/Other.cs` Nickname<br>`SRCCore/Items/Item.cs` Item.Nickname<br>`SRCCore/Models/AbilityData.cs` AbilityData.Nickname | 未確認 |  |
| Property Get KanaName | 199 | 70 | `SRCCore/Pilots/Pilot.props.cs` Pilot.KanaName | 未確認 |  |
| Property Get Sex | 271 | 10 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Sex | 未確認 |  |
| Property Get Class | 283 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Class | 未確認 |  |
| Property Get ExpValue | 288 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.ExpValue | 未確認 |  |
| Property Get Personality | 293 | 25 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Personality | 未確認 |  |
| Property Get Bitmap | 320 | 55 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap<br>`SRCCore/Models/PilotData.cs` PilotData.Bitmap<br>`SRCCore/Models/UnitData.cs` UnitData.Bitmap | 未確認 |  |
| Property Get BGM | 377 | 3 | `SRCCore/Pilots/Pilot.props.cs` Pilot.BGM | 未確認 |  |
| Property Get MessageType | 382 | 17 | `SRCCore/Pilots/Pilot.props.cs` Pilot.MessageType | 未確認 |  |
| Property Get Defense | 402 | 14 | `SRCCore/Pilots/Pilot.props.cs` Pilot.Defense | 未確認 |  |
| Sub Update | 419 | 417 | `SRCCore/Pilots/Pilot.status.cs` Pilot.Update | 未確認 |  |
| Sub UpdateSupportMod | 838 | 315 | `SRCCore/Pilots/Pilot.status.cs` Pilot.UpdateSupportMod | 未確認 |  |
| Property Get Level | 1158 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Level | 未確認 |  |
| Property Let Level | 1162 | 9 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Level | 未確認 |  |
| Property Get Exp | 1173 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Exp | 未確認 |  |
| Property Let Exp | 1177 | 39 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Exp | 未確認 |  |
| Property Get Morale | 1219 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Morale | 未確認 |  |
| Property Let Morale | 1223 | 3 | `SRCCore/Pilots/Pilot.level.cs` Pilot.Morale | 未確認 |  |
| Sub SetMorale | 1227 | 15 | `SRCCore/Pilots/Pilot.level.cs` Pilot.SetMorale | 未確認 |  |
| Property Get MaxMorale | 1243 | 8 | `SRCCore/Pilots/Pilot.level.cs` Pilot.MaxMorale | 未確認 |  |
| Property Get MinMorale | 1252 | 8 | `SRCCore/Pilots/Pilot.level.cs` Pilot.MinMorale | 未確認 |  |
| Property Get MaxSP | 1265 | 55 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.MaxSP | 未確認 |  |
| Property Get SP | 1322 | 36 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP | 未確認 |  |
| Property Let SP | 1359 | 51 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP | 未確認 |  |
| Property Get SP0 | 1411 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP0 | 未確認 |  |
| Property Let SP0 | 1415 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SP0 | 未確認 |  |
| Function MaxPlana | 1423 | 34 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.MaxPlana | 未確認 |  |
| Property Get Plana | 1459 | 36 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana | 未確認 |  |
| Property Let Plana | 1496 | 51 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana | 未確認 |  |
| Property Get Plana0 | 1548 | 3 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana0 | 未確認 |  |
| Property Let Plana0 | 1552 | 3 | `SRCCore/Pilots/Pilot.plana.cs` Pilot.Plana0 | 未確認 |  |
| Function CountSkill | 1560 | 3 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.CountSkill | 未確認 |  |
| Function Skill | 1565 | 6 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.Skill | 未確認 |  |
| Function IsSkillAvailable | 1573 | 49 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.IsSkillAvailable | 未確認 |  |
| Function IsSkillAvailable2 | 1625 | 11 | （見つからない） | 未確認 |  |
| Function SkillLevel | 1640 | 73 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillLevel | 未確認 |  |
| Function IsSkillLevelSpecified | 1715 | 56 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.IsSkillLevelSpecified | 未確認 |  |
| Function SkillData | 1773 | 62 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillData | 未確認 |  |
| Function SkillName | 1837 | 272 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillName | 未確認 |  |
| Function SkillName0 | 2111 | 207 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillName0 | 未確認 |  |
| Function SkillNameForNS | 2322 | 137 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillNameForNS | 未確認 |  |
| Function SkillType | 2461 | 77 | `SRCCore/Pilots/Pilot.skill.cs` Pilot.SkillType | 未確認 |  |
| Property Get CountSpecialPower | 2543 | 19 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.CountSpecialPower | 未確認 |  |
| Property Get SpecialPower | 2564 | 19 | `SRCCore/Expressions/Functions/Unit/Unit.cs` SpecialPower<br>`SRCCore/Models/PilotData.cs` PilotData.SpecialPower<br>`SRCCore/Units/Unit.sp.cs` Unit.SpecialPower | 未確認 |  |
| Function IsSpecialPowerAvailable | 2585 | 20 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.IsSpecialPowerAvailable | 未確認 |  |
| Function IsSpecialPowerUseful | 2607 | 3 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.IsSpecialPowerUseful | 未確認 |  |
| Function SpecialPowerCost | 2612 | 66 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.SpecialPowerCost | 未確認 |  |
| Sub UseSpecialPower | 2680 | 51 | `SRCCore/Pilots/Pilot.sp.cs` Pilot.UseSpecialPower | 未確認 |  |
| Sub Ride | 2736 | 65 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.Ride | 未確認 |  |
| Sub GetOff | 2803 | 46 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.GetOff | 未確認 |  |
| Function IsSupport | 2851 | 81 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.IsSupport | 未確認 |  |
| Function IsAbleToRide | 2934 | 68 | `SRCCore/Pilots/Pilot.unit.cs` Pilot.IsAbleToRide | 未確認 |  |
| Sub Dump | 3007 | 11 | （見つからない） | 未確認 |  |
| Sub Restore | 3020 | 36 | `SRCCore/Pilots/Pilot.cs` Pilot.Restore | 未確認 |  |
| Sub RestoreLinkInfo | 3058 | 19 | （見つからない） | 未確認 |  |
| Sub RestoreParameter | 3079 | 23 | （見つからない） | 未確認 |  |
| Sub FullRecover | 3107 | 15 | `SRCCore/Pilots/Pilot.other.cs` Pilot.FullRecover | 未確認 |  |
| Function SynchroRate | 3124 | 18 | `SRCCore/Pilots/Pilot.other.cs` Pilot.SynchroRate | 未確認 |  |
| Function CommandRange | 3144 | 19 | `SRCCore/Pilots/Pilot.other.cs` Pilot.CommandRange | 未確認 |  |
| Function TacticalTechnique0 | 3165 | 3 | `SRCCore/Pilots/Pilot.other.cs` Pilot.TacticalTechnique0 | 未確認 |  |
| Function TacticalTechnique | 3169 | 15 | `SRCCore/Pilots/Pilot.other.cs` Pilot.TacticalTechnique | 未確認 |  |
| Function Relation | 3186 | 3 | `SRCCore/Pilots/Pilot.other.cs` Pilot.Relation | 未確認 |  |
| Function HasMana | 3191 | 13 | `SRCCore/Pilots/Pilot.other.cs` Pilot.HasMana | 未確認 |  |
