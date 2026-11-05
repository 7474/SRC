# Unit.cls

VB 版の `SRC/SRC_20121125/Unit.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 169 | 8 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 179 | 81 | （見つからない） | 未確認 |  |
| Property Get Name | 265 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Name | 未確認 |  |
| Property Let Name | 269 | 5 | `SRCCore/Units/Unit.props.cs` Unit.Name | 未確認 |  |
| Property Get Nickname0 | 276 | 30 | `SRCCore/Units/Unit.props.cs` Unit.Nickname0 | 未確認 |  |
| Property Get Nickname | 308 | 15 | `SRCCore/Units/Unit.props.cs` Unit.Nickname | 未確認 |  |
| Property Get KanaName | 325 | 48 | `SRCCore/Units/Unit.props.cs` Unit.KanaName | 未確認 |  |
| Property Get Rank | 375 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Rank | 未確認 |  |
| Property Let Rank | 379 | 15 | `SRCCore/Units/Unit.props.cs` Unit.Rank | 未確認 |  |
| Property Get BossRank | 396 | 3 | `SRCCore/Units/Unit.props.cs` Unit.BossRank | 未確認 |  |
| Property Let BossRank | 400 | 10 | `SRCCore/Units/Unit.props.cs` Unit.BossRank | 未確認 |  |
| Property Get Class | 412 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Class | 未確認 |  |
| Property Get Class0 | 417 | 29 | `SRCCore/Units/Unit.props.cs` Unit.Class0 | 未確認 |  |
| Property Get Size | 449 | 8 | `SRCCore/Units/Unit.props.cs` Unit.Size | 未確認 |  |
| Property Get MaxHP | 460 | 21 | `SRCCore/Units/Unit.props.cs` Unit.MaxHP | 未確認 |  |
| Property Get MaxEN | 483 | 21 | `SRCCore/Units/Unit.props.cs` Unit.MaxEN | 未確認 |  |
| Property Get HP | 506 | 3 | `SRCCore/Units/Unit.props.cs` Unit.HP | 未確認 |  |
| Property Let HP | 510 | 8 | `SRCCore/Units/Unit.props.cs` Unit.HP | 未確認 |  |
| Property Get EN | 520 | 3 | `SRCCore/Units/Unit.props.cs` Unit.EN | 未確認 |  |
| Property Let EN | 524 | 9 | `SRCCore/Units/Unit.props.cs` Unit.EN | 未確認 |  |
| Property Get Speed0 | 536 | 3 | `SRCCore/Units/Unit.props.cs` Unit.Speed0 | 未確認 |  |
| Property Get Speed | 540 | 56 | `SRCCore/Units/Unit.props.cs` Unit.Speed | 未確認 |  |
| Property Get Transportation | 599 | 30 | `SRCCore/Units/Unit.props.cs` Unit.Transportation | 未確認 |  |
| Function IsTransAvailable | 631 | 17 | `SRCCore/Units/Unit.action.cs` Unit.IsTransAvailable | 未確認 |  |
| Property Get Adaption | 651 | 66 | （見つからない） | 未確認 |  |
| Property Get AdaptionMod | 719 | 79 | （見つからない） | 未確認 |  |
| Property Get Armor | 801 | 53 | `SRCCore/Items/Item.cs` Item.Armor | 未確認 |  |
| Property Get Mobility | 856 | 30 | `SRCCore/Items/Item.cs` Item.Mobility | 未確認 |  |
| Property Get Bitmap | 888 | 17 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap<br>`SRCCore/Models/PilotData.cs` PilotData.Bitmap<br>`SRCCore/Models/UnitData.cs` UnitData.Bitmap | 未確認 |  |
| Property Get Value | 907 | 13 | `SRCCore/Units/Unit.props.cs` Unit.Value | 未確認 |  |
| Property Get ExpValue | 922 | 11 | `SRCCore/Units/Unit.props.cs` Unit.ExpValue | 未確認 |  |
| Function CountFeature | 938 | 3 | `SRCCore/Units/Unit.feature.cs` Unit.CountFeature | 未確認 |  |
| Function Feature | 943 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.Feature | 未確認 |  |
| Function FeatureName | 951 | 3 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureName | 未確認 |  |
| Function FeatureNameInt | 955 | 101 | （見つからない） | 未確認 |  |
| Function FeatureName0 | 1057 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureName0 | 未確認 |  |
| Function FeatureLevel | 1065 | 17 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureLevel | 未確認 |  |
| Function FeatureData | 1084 | 11 | `SRCCore/Units/Unit.feature.cs` Unit.FeatureData | 未確認 |  |
| Function FeatureNecessarySkill | 1097 | 11 | `SRCCore/Items/Item.cs` Item.FeatureNecessarySkill | 未確認 |  |
| Function IsFeatureAvailable | 1110 | 11 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureAvailable | 未確認 |  |
| Function IsFeatureLevelSpecified | 1123 | 16 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureLevelSpecified | 未確認 |  |
| Function CountAllFeature | 1141 | 3 | （見つからない） | 未確認 |  |
| Function AllFeature | 1146 | 6 | `SRCCore/Units/Unit.feature.cs` Unit.AllFeature | 未確認 |  |
| Function AllFeatureName | 1154 | 3 | （見つからない） | 未確認 |  |
| Function AllFeatureLevel | 1159 | 17 | （見つからない） | 未確認 |  |
| Function AllFeatureLevelSpecified | 1178 | 14 | （見つからない） | 未確認 |  |
| Function AllFeatureData | 1194 | 11 | （見つからない） | 未確認 |  |
| Function IsAllFeatureLevelSpecified | 1207 | 16 | （見つからない） | 未確認 |  |
| Function IsFeatureActivated | 1225 | 19 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureActivated | 未確認 |  |
| Sub Update | 1250 | 2188 | `SRCCore/Units/Unit.status.cs` Unit.Update | 未確認 |  |
| Sub AddFeatures | 3440 | 36 | `SRCCore/Units/Unit.feature.cs` Unit.AddFeatures | 未確認 |  |
| Function IsFeatureRegistered | 3478 | 12 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureRegistered | 未確認 |  |
| Function IsAllFeatureRegistered | 3492 | 12 | （見つからない） | 未確認 |  |
| Sub UpdateFeatures | 3507 | 94 | `SRCCore/Units/Unit.feature.cs` Unit.UpdateFeatures | 未確認 |  |
| Sub AddOtherForm | 3606 | 3 | `SRCCore/Units/Unit.otherform.cs` Unit.AddOtherForm | 未確認 |  |
| Sub DeleteOtherForm | 3611 | 16 | `SRCCore/Units/Unit.otherform.cs` Unit.DeleteOtherForm | 未確認 |  |
| Function CountOtherForm | 3629 | 3 | `SRCCore/Units/Unit.otherform.cs` Unit.CountOtherForm | 未確認 |  |
| Function OtherForm | 3634 | 45 | `SRCCore/Units/Unit.otherform.cs` Unit.OtherForm | 未確認 |  |
| Function IsOtherFormDefined | 3681 | 12 | `SRCCore/Units/Unit.otherform.cs` Unit.IsOtherFormDefined | 未確認 |  |
| Sub DeleteTemporaryOtherForm | 3695 | 80 | `SRCCore/Units/Unit.otherform.cs` Unit.DeleteTemporaryOtherForm | 未確認 |  |
| Sub AddPilot | 3780 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.AddPilot | 未確認 |  |
| Sub DeletePilot | 3785 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.DeletePilot | 未確認 |  |
| Sub ReplacePilot | 3790 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.ReplacePilot | 未確認 |  |
| Function CountPilot | 3818 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.CountPilot | 未確認 |  |
| Function Pilot | 3823 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.Pilot | 未確認 |  |
| Function MainPilot | 3829 | 195 | `SRCCore/Units/Unit.pilot.cs` Unit.MainPilot | 未確認 |  |
| Sub AddSupport | 4027 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.AddSupport | 未確認 |  |
| Sub DeleteSupport | 4032 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.DeleteSupport | 未確認 |  |
| Sub ReplaceSupport | 4037 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.ReplaceSupport | 未確認 |  |
| Function CountSupport | 4065 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.CountSupport | 未確認 |  |
| Function Support | 4070 | 3 | `SRCCore/Units/Unit.pilot.cs` Unit.Support | 未確認 |  |
| Function AdditionalSupport | 4075 | 77 | `SRCCore/Units/Unit.pilot.cs` Unit.AdditionalSupport | 未確認 |  |
| Function IsSkillAvailable | 4154 | 41 | `SRCCore/Units/Unit.pilot.cs` Unit.IsSkillAvailable | 未確認 |  |
| Function SkillLevel | 4197 | 33 | `SRCCore/Units/Unit.pilot.cs` Unit.SkillLevel | 未確認 |  |
| Function MaxSkillLevel | 4232 | 69 | `SRCCore/Units/Unit.pilot.cs` Unit.MaxSkillLevel | 未確認 |  |
| Function AuraLevel | 4303 | 26 | `SRCCore/Units/Unit.pilot.cs` Unit.AuraLevel | 未確認 |  |
| Function PsychicLevel | 4331 | 27 | `SRCCore/Units/Unit.pilot.cs` Unit.PsychicLevel | 未確認 |  |
| Function SyncLevel | 4360 | 14 | `SRCCore/Units/Unit.pilot.cs` Unit.SyncLevel | 未確認 |  |
| Function PlanaLevel | 4376 | 14 | `SRCCore/Units/Unit.pilot.cs` Unit.PlanaLevel | 未確認 |  |
| Function SkillName0 | 4392 | 41 | `SRCCore/Units/Unit.pilot.cs` Unit.SkillName0 | 未確認 |  |
| Function MaxAction | 4438 | 42 | `SRCCore/Units/Unit.action.cs` Unit.MaxAction | 未確認 |  |
| Property Get Action | 4482 | 7 | `SRCCore/Units/Unit.props.cs` Unit.Action | 未確認 |  |
| Sub UseAction | 4491 | 23 | `SRCCore/Units/Unit.action.cs` Unit.UseAction | 未確認 |  |
| Function SpecialPowerInEffect | 4519 | 34 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerInEffect | 未確認 |  |
| Function IsSpecialPowerInEffect | 4555 | 13 | `SRCCore/Units/Unit.sp.cs` Unit.IsSpecialPowerInEffect | 未確認 |  |
| Function IsUnderSpecialPowerEffect | 4570 | 16 | `SRCCore/Units/Unit.sp.cs` Unit.IsUnderSpecialPowerEffect | 未確認 |  |
| Function CountSpecialPower | 4588 | 3 | `SRCCore/Units/Unit.sp.cs` Unit.CountSpecialPower | 未確認 |  |
| Function SpecialPower | 4593 | 13 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPower | 未確認 |  |
| Function SpecialPowerEffectLevel | 4608 | 22 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerEffectLevel | 未確認 |  |
| Function SpecialPowerData | 4632 | 10 | `SRCCore/Units/Unit.sp.cs` Unit.SpecialPowerData | 未確認 |  |
| Sub MakeSpecialPowerInEffect | 4644 | 15 | `SRCCore/Units/Unit.sp.cs` Unit.MakeSpecialPowerInEffect | 未確認 |  |
| Sub RemoveSpecialPowerInEffect | 4661 | 51 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveSpecialPowerInEffect | 未確認 |  |
| Sub RemoveSpecialPowerInEffect2 | 4714 | 6 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveSpecialPowerInEffect2 | 未確認 |  |
| Sub RemoveAllSpecialPowerInEffect | 4722 | 9 | `SRCCore/Units/Unit.sp.cs` Unit.RemoveAllSpecialPowerInEffect | 未確認 |  |
| Sub CopySpecialPowerInEffect | 4733 | 9 | `SRCCore/Units/Unit.sp.cs` Unit.CopySpecialPowerInEffect | 未確認 |  |
| Sub AddCondition | 4747 | 32 | `SRCCore/Units/Unit.condition.cs` Unit.AddCondition | 未確認 |  |
| Sub DeleteCondition | 4781 | 12 | `SRCCore/Units/Unit.condition.cs` Unit.DeleteCondition | 未確認 |  |
| Sub DeleteCondition0 | 4794 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.DeleteCondition0 | 未確認 |  |
| Function CountCondition | 4799 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.CountCondition | 未確認 |  |
| Function Condition | 4804 | 3 | `SRCCore/Units/Unit.condition.cs` Unit.Condition | 未確認 |  |
| Function ConditionLifetime | 4809 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionLifetime | 未確認 |  |
| Function IsConditionSatisfied | 4819 | 11 | `SRCCore/Units/Unit.condition.cs` Unit.IsConditionSatisfied | 未確認 |  |
| Function ConditionLevel | 4832 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionLevel | 未確認 |  |
| Sub SetConditionLevel | 4842 | 5 | （見つからない） | 未確認 |  |
| Function ConditionData | 4849 | 8 | `SRCCore/Units/Unit.condition.cs` Unit.ConditionData | 未確認 |  |
| Sub UpdateCondition | 4859 | 59 | `SRCCore/Units/Unit.condition.cs` Unit.UpdateCondition | 未確認 |  |
| Function Weapon | 4923 | 3 | `SRCCore/Units/Unit.weapon.cs` Unit.Weapon | 一致 | 範囲外は null を返す（VB は実行時エラー。SRC# の利便性） |
| Function CountWeapon | 4928 | 3 | `SRCCore/Units/Unit.weapon.cs` Unit.CountWeapon | 一致 |  |
| Function WeaponNickname | 4933 | 9 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponNickname | 一致 |  |
| Function WeaponPower | 4945 | 152 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponPower | 修正済み | バトルコンフィグの「攻撃補正」の前に BCVariable.AtkUnit を設定していなかった |
| Function WeaponAdaption | 5099 | 201 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponAdaption | 一致 |  |
| Function WeaponMaxRange | 5302 | 31 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponMaxRange<br>`SRCCore/Units/UnitWeapon.cs` UnitWeapon.SetMaxRangeCorrection | 修正済み | 補正後の最低値を 1 に（WeaponMinRange() だった）。Ｍ・武/突/接・有の武器でもサ属性の射程延長を残す |
| Function WeaponENConsumption | 5335 | 80 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponENConsumption | 一致 |  |
| Function WeaponPrecision | 5417 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponPrecision | 一致 |  |
| Function WeaponCritical | 5422 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponCritical | 一致 |  |
| Function WeaponClass | 5427 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponClass | 一致 |  |
| Function IsWeaponClassifiedAs | 5432 | 65 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponClassifiedAs | 修正済み | 格闘系・射撃系・移動後攻撃可の判定で、補正後ではなく武器データの MaxRange を見る |
| Function WeaponLevel | 5499 | 44 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.WeaponLevel | 修正済み | 数値として読めないレベルでエラーメッセージを出す。メッセージの先頭をユニット名に |
| Function IsWeaponLevelSpecified | 5545 | 6 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponLevelSpecified | 一致 |  |
| Function IsNormalWeapon | 5553 | 17 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsNormalWeapon | 一致 |  |
| Function CountWeaponEffect | 5572 | 31 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.CountWeaponEffect | 一致 |  |
| Function IsSpellWeapon | 5605 | 22 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsSpellWeapon | 一致 |  |
| Function IsFeatWeapon | 5629 | 22 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsFeatWeapon | 一致 |  |
| Function IsWeaponAvailable | 5654 | 322 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponAvailable | 修正済み | 「○属性使用不能」の判定を VB の Len>6・Right$・Left$ に合わせた。変形技の判定の順序を VB に合わせた |
| Function IsWeaponMastered | 5978 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponMastered | 一致 |  |
| Function IsWeaponEnabled | 5983 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponEnabled | 一致 |  |
| Function IsWeaponUseful | 5988 | 81 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsWeaponUseful | 一致 |  |
| Function IsTargetWithinRange | 6072 | 70 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsTargetWithinRange | 修正済み | ステルスのレベルを偶数丸めしてから距離と比べる（VB の lv は Integer） |
| Function IsTargetReachable | 6144 | 70 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsTargetReachable | 一致 | VB の `Abs(.y - i)`（本来は j）も同じにしている |
| Function HitProbability | 6218 | 481 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.HitProbability | 修正済み | 地形適応は 1 つ目の要素（名称）を比べず、能力ごとに -10。未知のサイズの補正を 0 倍に。ECM（マイナスのレベル・範囲）は #612・90fd4dd0 の意図した違い（SRCとの互換性）。「地形適応命中率修正」のＪは VB では wclass が空で効かない不具合なので再現しない |
| Function Damage | 6703 | 785 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.Damage | 修正済み | 対属性のレベルが実数のとき、VB の Mod と同じく偶数丸めの整数で割る |
| Function CriticalProbability | 7490 | 340 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.CriticalProbability | 修正済み | 対属性のレベルが実数のとき、VB の Mod と同じく偶数丸めの整数で割る（Damage と同じ） |
| Function ExpDamage | 7832 | 546 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.ExpDamage | 一致 | 末尾の抹殺の判定は VB では `InStr(w, "殺")`（w は武器の番号）で働かない。VB の不具合なので再現しない（C# は武器属性を見る） |
| Function Absorb | 8383 | 52 | `SRCCore/Units/Unit.attribute.cs` Unit.Absorb | 修正済み | Exit Do を return にした（戻り値は以前と同じ） |
| Function Immune | 8437 | 51 | `SRCCore/Units/Unit.attribute.cs` Unit.Immune | 修正済み | Absorb と同じ |
| Function Resist | 8490 | 51 | `SRCCore/Units/Unit.attribute.cs` Unit.Resist | 修正済み | Absorb と同じ |
| Function Weakness | 8543 | 37 | `SRCCore/Units/Unit.attribute.cs` Unit.Weakness | 修正済み | Absorb と同じ |
| Function Effective | 8582 | 37 | `SRCCore/Units/Unit.attribute.cs` Unit.Effective | 修正済み | Absorb と同じ |
| Function SpecialEffectImmune | 8621 | 26 | `SRCCore/Units/Unit.attribute.cs` Unit.SpecialEffectImmune | 一致 |  |
| Function IsAttributeClassified | 8650 | 65 | `SRCCore/Units/Unit.attribute.cs` Unit.IsAttributeClassified | 修正済み | Exit Do が switch の break でループを抜けていなかった。該当した属性の後ろの「!」と、「魔」で魔法武器のときの判定が VB と違っていた |
| Sub Attack | 8724 | 2147 | `SRCCore/Units/Unit.attack.cs` Unit.Attack | 修正済み | 浄化を探す相手から追加サポートを外した。レベルの無い連属性は攻撃回数 1 回（VB は実行時エラー。SRC# の利便性）。消費したアイテムは武器名で探す（VB の数え方の不具合は再現しない）。BitmapID・新ＧＵＩの ClearUnitStatus は SRC# では不要 |
| Function CheckDodgeFeature | 10873 | 450 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDodgeFeature | 一致 |  |
| Function CheckParryFeature | 11326 | 1054 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckParryFeature | 未確認 |  |
| Function CheckDummyFeature | 12382 | 114 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDummyFeature | 未確認 |  |
| Function CheckShieldFeature | 12498 | 105 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckShieldFeature | 未確認 |  |
| Function CheckDefenseFeature | 12605 | 1728 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDefenseFeature | 未確認 |  |
| Sub CheckAutoAttack | 14335 | 256 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckAutoAttack | 未確認 |  |
| Sub CheckAdditionalAttack | 14594 | 175 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckAdditionalAttack | 未確認 |  |
| Function IsSameCategory | 14772 | 19 | `SRCCore/Units/Unit.attackcheck.cs` Unit.IsSameCategory | 未確認 |  |
| Function CauseEffect | 14793 | 983 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CauseEffect | 未確認 |  |
| Function CheckBlowAttack | 15778 | 313 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckBlowAttack | 未確認 |  |
| Function CheckDrawAttack | 16093 | 191 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckDrawAttack | 未確認 |  |
| Function CheckTeleportAwayAttack | 16286 | 85 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckTeleportAwayAttack | 未確認 |  |
| Function CheckMetamorphAttack | 16373 | 103 | `SRCCore/Units/Unit.attackcheck.cs` Unit.CheckMetamorphAttack | 未確認 |  |
| Sub MapAttack | 16479 | 1040 | `SRCCore/Units/Unit.attackmap.cs` Unit.MapAttack | 未確認 |  |
| Sub UseWeapon | 17521 | 87 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.UseWeapon | 未確認 |  |
| Function Bullet | 17610 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.Bullet | 未確認 |  |
| Function MaxBullet | 17615 | 3 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.MaxBullet | 未確認 |  |
| Sub SetBullet | 17620 | 9 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.SetBullet | 未確認 |  |
| Sub SyncBullet | 17631 | 126 | `SRCCore/Units/Unit.recover.cs` Unit.SyncBullet | 未確認 |  |
| Function Ability | 17763 | 3 | `SRCCore/Units/Unit.ability.cs` Unit.Ability | 未確認 |  |
| Function CountAbility | 17768 | 3 | `SRCCore/Units/Unit.ability.cs` Unit.CountAbility | 未確認 |  |
| Function AbilityNickname | 17773 | 9 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityNickname | 未確認 |  |
| Function AbilityMinRange | 17784 | 6 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityMinRange | 未確認 |  |
| Function AbilityMaxRange | 17792 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityMaxRange | 未確認 |  |
| Function AbilityENConsumption | 17797 | 82 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityENConsumption | 未確認 |  |
| Function IsAbilityClassifiedAs | 17881 | 7 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityClassifiedAs | 未確認 |  |
| Function AbilityLevel | 17890 | 44 | `SRCCore/Units/UnitAbility.cs` UnitAbility.AbilityLevel | 未確認 |  |
| Function IsSpellAbility | 17936 | 22 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsSpellAbility | 未確認 |  |
| Function IsFeatAbility | 17960 | 22 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsFeatAbility | 未確認 |  |
| Function IsAbilityAvailable | 17985 | 340 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityAvailable | 未確認 |  |
| Function IsAbilityMastered | 18327 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityMastered | 未確認 |  |
| Function IsAbilityEnabled | 18332 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityEnabled | 未確認 |  |
| Function IsAbilityUseful | 18337 | 52 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityUseful | 未確認 |  |
| Function IsAbilityEffective | 18391 | 330 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityEffective | 未確認 |  |
| Function IsAbilityApplicable | 18723 | 93 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsAbilityApplicable | 未確認 |  |
| Function IsTargetWithinAbilityRange | 18818 | 32 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsTargetWithinAbilityRange | 未確認 |  |
| Function IsTargetReachableForAbility | 18852 | 21 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsTargetReachableForAbility | 未確認 |  |
| Function Stock | 18875 | 3 | `SRCCore/Units/UnitAbility.cs` UnitAbility.Stock | 未確認 |  |
| Function MaxStock | 18880 | 7 | `SRCCore/Units/UnitAbility.cs` UnitAbility.MaxStock | 未確認 |  |
| Sub SetStock | 18889 | 9 | `SRCCore/Units/UnitAbility.cs` UnitAbility.SetStock | 未確認 |  |
| Function ExecuteAbility | 18904 | 1737 | `SRCCore/Units/Unit.ability.cs` Unit.ExecuteAbility | 未確認 |  |
| Sub ExecuteMapAbility | 20643 | 627 | `SRCCore/Units/Unit.ability.cs` Unit.ExecuteMapAbility | 未確認 |  |
| Sub UseAbility | 21272 | 87 | `SRCCore/Units/UnitAbility.cs` UnitAbility.UseAbility | 未確認 |  |
| Function MaxItemNum | 21364 | 15 | `SRCCore/Units/Unit.item.cs` Unit.MaxItemNum | 未確認 |  |
| Function CountItem | 21381 | 3 | `SRCCore/Units/Unit.item.cs` Unit.CountItem | 未確認 |  |
| Function Item | 21386 | 19 | `SRCCore/Units/Unit.item.cs` Unit.Item | 未確認 |  |
| Sub AddItem | 21407 | 265 | `SRCCore/Units/Unit.item.cs` Unit.AddItem | 未確認 |  |
| Sub AddItem0 | 21673 | 4 | `SRCCore/Units/Unit.item.cs` Unit.AddItem0 | 未確認 |  |
| Sub DeleteItem | 21679 | 256 | `SRCCore/Units/Unit.item.cs` Unit.DeleteItem | 未確認 |  |
| Function ItemSlotSize | 21937 | 31 | `SRCCore/Units/Unit.item.cs` Unit.ItemSlotSize | 未確認 |  |
| Function IsEquiped | 21970 | 11 | `SRCCore/Units/Unit.item.cs` Unit.IsEquiped | 未確認 |  |
| Function WeaponProficiency | 21983 | 11 | `SRCCore/Units/Unit.item.cs` Unit.WeaponProficiency | 未確認 |  |
| Function ArmorProficiency | 21996 | 11 | `SRCCore/Units/Unit.item.cs` Unit.ArmorProficiency | 未確認 |  |
| Function IsAbleToEquip | 22009 | 259 | `SRCCore/Units/Unit.item.cs` Unit.IsAbleToEquip | 未確認 |  |
| Sub PilotMessage | 22273 | 453 | `SRCCore/Units/Unit.message.cs` Unit.PilotMessage | 未確認 |  |
| Sub PlayDialog | 22728 | 167 | `SRCCore/Units/Unit.message.cs` Unit.PlayDialog | 未確認 |  |
| Sub PlayMessage | 22897 | 154 | `SRCCore/Units/Unit.message.cs` Unit.PlayMessage | 未確認 |  |
| Function IsMessageDefined | 23053 | 100 | `SRCCore/Units/Unit.message.cs` Unit.IsMessageDefined | 未確認 |  |
| Sub SysMessage | 23155 | 418 | `SRCCore/Units/Unit.message.cs` Unit.SysMessage | 未確認 |  |
| Function IsSysMessageDefined | 23575 | 252 | `SRCCore/Units/Unit.message.cs` Unit.IsSysMessageDefined | 未確認 |  |
| Function SpecialEffectData | 23832 | 87 | `SRCCore/Units/Unit.se.cs` Unit.SpecialEffectData | 未確認 |  |
| Sub SpecialEffect | 23921 | 325 | `SRCCore/Units/Unit.se.cs` Unit.SpecialEffect | 未確認 |  |
| Function IsSpecialEffectDefined | 24248 | 10 | `SRCCore/Units/Unit.se.cs` Unit.IsSpecialEffectDefined | 未確認 |  |
| Function AnimationData | 24265 | 182 | `SRCCore/Units/Unit.anime.cs` Unit.AnimationData | 未確認 |  |
| Sub PlayAnimation | 24449 | 357 | `SRCCore/Units/Unit.anime.cs` Unit.PlayAnimation | 未確認 |  |
| Function IsAnimationDefined | 24811 | 16 | `SRCCore/Units/Unit.anime.cs` Unit.IsAnimationDefined | 未確認 |  |
| Sub StandBy | 24831 | 264 | `SRCCore/Units/Unit.map.cs` Unit.StandBy | 未確認 |  |
| Sub Move | 25097 | 143 | `SRCCore/Units/Unit.map.cs` Unit.Move | 未確認 |  |
| Sub Jump | 25242 | 119 | `SRCCore/Units/Unit.map.cs` Unit.Jump | 未確認 |  |
| Sub Escape | 25363 | 60 | `SRCCore/Units/Unit.map.cs` Unit.Escape | 未確認 |  |
| Sub Land | 25425 | 136 | `SRCCore/Units/Unit.map.cs` Unit.Land | 未確認 |  |
| Sub Transform | 25563 | 343 | `SRCCore/Units/Unit.map.cs` Unit.Transform | 未確認 |  |
| Sub Combine | 25908 | 427 | `SRCCore/Units/Unit.map.cs` Unit.Combine | 未確認 |  |
| Sub Split | 26337 | 290 | `SRCCore/Units/Unit.map.cs` Unit.Split | 未確認 |  |
| Function GetExp | 26632 | 234 | `SRCCore/Units/Unit.action2.cs` Unit.GetExp | 未確認 |  |
| Sub ChangeParty | 26868 | 41 | `SRCCore/Units/Unit.action2.cs` Unit.ChangeParty | 未確認 |  |
| Sub IncreaseMorale | 26912 | 43 | `SRCCore/Units/Unit.action2.cs` Unit.IncreaseMorale | 未確認 |  |
| Sub Die | 26957 | 68 | `SRCCore/Units/Unit.action2.cs` Unit.Die | 未確認 |  |
| Sub SuicidalExplosion | 27027 | 179 | `SRCCore/Units/Unit.action2.cs` Unit.SuicidalExplosion | 未確認 |  |
| Sub FullRecover | 27211 | 63 | `SRCCore/Units/Unit.recover.cs` Unit.FullRecover | 未確認 |  |
| Sub FullSupply | 27276 | 27 | `SRCCore/Units/Unit.recover.cs` Unit.FullSupply | 未確認 |  |
| Sub BulletSupply | 27305 | 16 | `SRCCore/Units/Unit.recover.cs` Unit.BulletSupply | 未確認 |  |
| Sub RecoverHP | 27323 | 15 | `SRCCore/Units/Unit.recover.cs` Unit.RecoverHP | 未確認 |  |
| Sub RecoverEN | 27340 | 6 | `SRCCore/Units/Unit.recover.cs` Unit.RecoverEN | 未確認 |  |
| Sub Rest | 27348 | 1008 | `SRCCore/Units/Unit.recover.cs` Unit.Rest | 未確認 |  |
| Sub CheckAutoHyperMode | 28358 | 178 | `SRCCore/Units/Unit.recover.cs` Unit.CheckAutoHyperMode | 未確認 |  |
| Function CheckAutoNormalMode | 28538 | 139 | `SRCCore/Units/Unit.recover.cs` Unit.CheckAutoNormalMode | 未確認 |  |
| Sub Reset | 28680 | 33 | `SRCCore/Units/Unit.recover.cs` Unit.Reset | 未確認 |  |
| Property Get Party0 | 28718 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Party0 | 未確認 |  |
| Property Get Party | 28722 | 29 | `SRCCore/Units/Unit.action.cs` Unit.Party | 未確認 |  |
| Property Let Party | 28752 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Party | 未確認 |  |
| Function IsEnemy | 28757 | 106 | `SRCCore/Units/Unit.party.cs` Unit.IsEnemy | 未確認 |  |
| Function IsAlly | 28865 | 39 | `SRCCore/Units/Unit.party.cs` Unit.IsAlly | 未確認 |  |
| Property Get Mode | 28906 | 38 | `SRCCore/Units/Unit.action.cs` Unit.Mode | 未確認 |  |
| Property Let Mode | 28945 | 3 | `SRCCore/Units/Unit.action.cs` Unit.Mode | 未確認 |  |
| Sub LoadUnit | 28953 | 3 | `SRCCore/Units/Unit.onboard.cs` Unit.LoadUnit | 未確認 |  |
| Sub UnloadUnit | 28958 | 15 | `SRCCore/Units/Unit.onboard.cs` Unit.UnloadUnit | 未確認 |  |
| Function CountUnitOnBoard | 28975 | 3 | `SRCCore/Units/Unit.onboard.cs` Unit.CountUnitOnBoard | 未確認 |  |
| Function UnitOnBoard | 28980 | 15 | `SRCCore/Units/Unit.onboard.cs` Unit.UnitOnBoard | 未確認 |  |
| Sub AddServant | 29000 | 8 | `SRCCore/Units/Unit.summon.cs` Unit.AddServant | 未確認 |  |
| Sub DeleteServant | 29010 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.DeleteServant | 未確認 |  |
| Function CountServant | 29027 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.CountServant | 未確認 |  |
| Function Servant | 29032 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.Servant | 未確認 |  |
| Sub DismissServant | 29049 | 29 | `SRCCore/Units/Unit.summon.cs` Unit.DismissServant | 未確認 |  |
| Sub AddSlave | 29083 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.AddSlave | 未確認 |  |
| Sub DeleteSlave | 29088 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.DeleteSlave | 未確認 |  |
| Function CountSlave | 29105 | 3 | `SRCCore/Units/Unit.summon.cs` Unit.CountSlave | 未確認 |  |
| Function Slave | 29110 | 15 | `SRCCore/Units/Unit.summon.cs` Unit.Slave | 未確認 |  |
| Sub DismissSlave | 29127 | 22 | `SRCCore/Units/Unit.summon.cs` Unit.DismissSlave | 未確認 |  |
| Sub Dump | 29153 | 84 | （見つからない） | 未確認 |  |
| Sub Restore | 29239 | 163 | `SRCCore/Units/Unit.cs` Unit.Restore | 未確認 |  |
| Sub RestoreLinkInfo | 29404 | 107 | （見つからない） | 未確認 |  |
| Sub RestoreParameter | 29513 | 95 | （見つからない） | 未確認 |  |
| Function IsOperational | 29613 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.IsOperational | 未確認 |  |
| Function IsEqual | 29632 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.IsEqual | 未確認 |  |
| Function CurrentForm | 29651 | 14 | `SRCCore/Units/Unit.otherform.cs` Unit.CurrentForm | 未確認 |  |
| Function IsHero | 29667 | 9 | `SRCCore/Units/Unit.lookup.cs` Unit.IsHero | 未確認 |  |
| Function LookForAttackHelp | 29678 | 73 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForAttackHelp | 未確認 |  |
| Function LookForGuardHelp | 29753 | 135 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForGuardHelp | 未確認 |  |
| Function MaxSupportAttack | 29890 | 5 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSupportAttack | 未確認 |  |
| Function MaxSupportGuard | 29897 | 5 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSupportGuard | 未確認 |  |
| Function MaxSyncAttack | 29904 | 3 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxSyncAttack | 未確認 |  |
| Function MaxCounterAttack | 29909 | 17 | `SRCCore/Units/Unit.lookup.cs` Unit.MaxCounterAttack | 未確認 |  |
| Function LookForSupportAttack | 29928 | 151 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupportAttack | 未確認 |  |
| Function LookForSupportGuard | 30082 | 217 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupportGuard | 未確認 |  |
| Function LookForSupport | 30301 | 109 | `SRCCore/Units/Unit.lookup.cs` Unit.LookForSupport | 未確認 |  |
| Sub CombinationPartner | 30412 | 406 | `SRCCore/Units/Unit.lookup.cs` Unit.CombinationPartner | 未確認 |  |
| Function IsCombinationAttackAvailable | 30820 | 56 | `SRCCore/Units/UnitWeapon.cs` UnitWeapon.IsCombinationAttackAvailable | 未確認 |  |
| Function IsCombinationAbilityAvailable | 30878 | 56 | `SRCCore/Units/UnitAbility.cs` UnitAbility.IsCombinationAbilityAvailable | 未確認 |  |
| Function IsAbleToEnter | 30936 | 99 | `SRCCore/Units/Unit.lookup.cs` Unit.IsAbleToEnter | 未確認 |  |
| Function IsAvailable | 31037 | 45 | `SRCCore/Units/Unit.lookup.cs` Unit.IsAvailable | 未確認 |  |
| Function IsNecessarySkillSatisfied | 31084 | 50 | `SRCCore/Units/Unit.lookup.cs` Unit.IsNecessarySkillSatisfied | 未確認 |  |
| Function IsNecessarySkillSatisfied2 | 31135 | 570 | `SRCCore/Units/Unit.lookup.cs` Unit.IsNecessarySkillSatisfied2 | 未確認 |  |
| Function IsDisabled | 31707 | 18 | `SRCCore/Units/Unit.lookup.cs` Unit.IsDisabled | 未確認 |  |
| Function IsDefense | 31728 | 7 | `SRCCore/Units/Unit.lookup.cs` Unit.IsDefense | 未確認 |  |
