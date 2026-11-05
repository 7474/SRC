# Command.bas

VB 版の `SRC/SRC_20121125/Command.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub ProceedCommand | 156 | 1787 | `SRCCore/Commands/Command.process.cs` Command.ProceedCommand | 未確認 |  |
| Sub CancelCommand | 1945 | 139 | `SRCCore/Commands/Command.process.cs` Command.CancelCommand | 未確認 |  |
| Sub UnitCommand | 2087 | 252 | `SRCCore/Commands/Command.unitcommand.cs` Command.UnitCommand | 未確認 |  |
| Sub StartMoveCommand | 2344 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartMoveCommand | 未確認 |  |
| Sub FinishMoveCommand | 2365 | 104 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishMoveCommand | 未確認 |  |
| Sub StartTeleportCommand | 2474 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartTeleportCommand | 未確認 |  |
| Sub FinishTeleportCommand | 2495 | 126 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishTeleportCommand | 未確認 |  |
| Sub StartJumpCommand | 2626 | 17 | `SRCCore/Commands/Command.unitmove.cs` Command.StartJumpCommand | 未確認 |  |
| Sub FinishJumpCommand | 2647 | 122 | `SRCCore/Commands/Command.unitmove.cs` Command.FinishJumpCommand | 未確認 |  |
| Sub StartAttackCommand | 2774 | 246 | `SRCCore/Commands/Command.attack.cs` Command.StartAttackCommand | 未確認 |  |
| Sub FinishAttackCommand | 3024 | 866 | `SRCCore/Commands/Command.attack.cs` Command.FinishAttackCommand | 未確認 |  |
| Sub MapAttackCommand | 3894 | 169 | `SRCCore/Commands/Command.attack.cs` Command.MapAttackCommand | 未確認 |  |
| Sub StartAbilityCommand | 4069 | 296 | `SRCCore/Commands/Command.unitability.cs` Command.StartAbilityCommand | 未確認 |  |
| Sub FinishAbilityCommand | 4369 | 184 | `SRCCore/Commands/Command.unitability.cs` Command.FinishAbilityCommand | 未確認 |  |
| Sub MapAbilityCommand | 4557 | 162 | `SRCCore/Commands/Command.unitability.cs` Command.MapAbilityCommand | 未確認 |  |
| Sub StartSpecialPowerCommand | 4724 | 505 | `SRCCore/Commands/Command.unitsp.cs` Command.StartSpecialPowerCommand | 未確認 |  |
| Sub FinishSpecialPowerCommand | 5233 | 67 | `SRCCore/Commands/Command.unitsp.cs` Command.FinishSpecialPowerCommand | 未確認 |  |
| Sub StartFixCommand | 5305 | 78 | `SRCCore/Commands/Command.unitsupply.cs` Command.StartFixCommand | 未確認 |  |
| Sub FinishFixCommand | 5387 | 67 | `SRCCore/Commands/Command.unitsupply.cs` Command.FinishFixCommand | 未確認 |  |
| Sub StartSupplyCommand | 5459 | 71 | `SRCCore/Commands/Command.unitsupply.cs` Command.StartSupplyCommand | 未確認 |  |
| Sub FinishSupplyCommand | 5534 | 54 | `SRCCore/Commands/Command.unitsupply.cs` Command.FinishSupplyCommand | 未確認 |  |
| Sub ChargeCommand | 5593 | 104 | `SRCCore/Commands/Command.unitxxx.cs` Command.ChargeCommand | 未確認 |  |
| Sub StartTalkCommand | 5701 | 44 | `SRCCore/Commands/Command.unitxxx.cs` Command.StartTalkCommand | 未確認 |  |
| Sub FinishTalkCommand | 5749 | 32 | `SRCCore/Commands/Command.unitxxx.cs` Command.FinishTalkCommand | 未確認 |  |
| Sub TransformCommand | 5785 | 234 | `SRCCore/Commands/Command.unitform.cs` Command.TransformCommand | 未確認 |  |
| Sub HyperModeCommand | 6023 | 164 | `SRCCore/Commands/Command.unitform.cs` Command.HyperModeCommand | 未確認 |  |
| Sub CancelTransformationCommand | 6191 | 58 | `SRCCore/Commands/Command.unitform.cs` Command.CancelTransformationCommand | 未確認 |  |
| Sub SplitCommand | 6253 | 204 | `SRCCore/Commands/Command.unitform.cs` Command.SplitCommand | 未確認 |  |
| Sub CombineCommand | 6461 | 149 | `SRCCore/Commands/Command.unitform.cs` Command.CombineCommand | 未確認 |  |
| Sub ExchangeFormCommand | 6614 | 121 | `SRCCore/Commands/Command.unitform.cs` Command.ExchangeFormCommand | 未確認 |  |
| Sub StartLaunchCommand | 6740 | 93 | `SRCCore/Commands/Command.launch.cs` Command.StartLaunchCommand | 未確認 |  |
| Sub FinishLaunchCommand | 6837 | 70 | `SRCCore/Commands/Command.launch.cs` Command.FinishLaunchCommand | 未確認 |  |
| Sub StartOrderCommand | 6912 | 71 | `SRCCore/Commands/Command.unitxxx.cs` Command.StartOrderCommand | 未確認 |  |
| Sub FinishOrderCommand | 6987 | 16 | `SRCCore/Commands/Command.unitxxx.cs` Command.FinishOrderCommand | 未確認 |  |
| Sub FeatureListCommand | 7008 | 267 | `SRCCore/Commands/Command.preview.cs` ItemKind.FeatureListCommand | 未確認 |  |
| Sub WeaponListCommand | 7279 | 160 | `SRCCore/Commands/Command.preview.cs` ItemKind.WeaponListCommand | 未確認 |  |
| Sub AbilityListCommand | 7443 | 155 | `SRCCore/Commands/Command.preview.cs` ItemKind.AbilityListCommand | 未確認 |  |
| Sub ShowAreaInSpeedCommand | 7602 | 14 | `SRCCore/Commands/Command.preview.cs` ItemKind.ShowAreaInSpeedCommand | 未確認 |  |
| Sub ShowAreaInRangeCommand | 7620 | 38 | `SRCCore/Commands/Command.preview.cs` ItemKind.ShowAreaInRangeCommand | 未確認 |  |
| Sub WaitCommand | 7664 | 139 | `SRCCore/Commands/Command.unitcommand.cs` Command.WaitCommand | 未確認 |  |
| Sub MapCommand | 7806 | 51 | `SRCCore/Commands/Command.mapcommend.cs` Command.MapCommand | 未確認 |  |
| Sub EndTurnCommand | 7861 | 74 | `SRCCore/Commands/Command.mapcommend.cs` Command.EndTurnCommand | 未確認 |  |
| Sub UnitListCommand | 7939 | 304 | `SRCCore/Commands/Command.mapcommend.cs` Command.UnitListCommand | 未確認 |  |
| Sub SearchSpecialPowerCommand | 8247 | 216 | `SRCCore/Commands/Command.mapcommend.cs` Command.SearchSpecialPowerCommand | 未確認 |  |
| Sub RestartCommand | 8467 | 17 | `SRCCore/Commands/Command.mapcommend.cs` Command.RestartCommand | 未確認 |  |
| Sub QuickLoadCommand | 8488 | 21 | `SRCCore/Commands/Command.mapcommend.cs` Command.QuickLoadCommand | 未確認 |  |
| Sub QuickSaveCommand | 8513 | 16 | `SRCCore/Commands/Command.mapcommend.cs` Command.QuickSaveCommand | 未確認 |  |
| Sub DumpCommand | 8534 | 57 | `SRCCore/Commands/Command.mapcommend.cs` Command.DumpCommand | 未確認 |  |
| Sub GlobalMapCommand | 8596 | 126 | `SRCCore/Commands/Command.mapcommend.cs` Command.GlobalMapCommand | 未確認 |  |
| Sub SaveSelections | 8725 | 34 | `SRCCore/Commands/Command.state.cs` Command.SaveSelections | 未確認 |  |
| Sub RestoreSelections | 8761 | 57 | `SRCCore/Commands/Command.state.cs` Command.RestoreSelections | 未確認 |  |
| Sub SwapSelections | 8820 | 20 | `SRCCore/Commands/Command.state.cs` Command.SwapSelections | 未確認 |  |
