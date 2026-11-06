# Help.bas

VB 版の `SRC/SRC_20121125/Help.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub SkillHelp | 13 | 35 | `SRCCore/Help.cs` Help.SkillHelp | 未確認 |  |
| Function SkillHelpMessage | 50 | 741 | `SRCCore/Help.cs` Help.SkillHelpMessage | 未確認 |  |
| Sub FeatureHelp | 794 | 33 | `SRCCore/Help.cs` Help.FeatureHelp | 未確認 | #1498（Unit.cls 能力値）で見つけた手がかり: VB の .AllFeatureName(findex)（Help.bas 804・806 行）は見つからないとき findex を返す。C# の Help.cs の u.AllFeature(findex).FeatureName(u) は見つからないと NullReferenceException になる |
| Function FeatureHelpMessage | 829 | 2456 | `SRCCore/Help.cs` Help.FeatureHelpMessage | 未確認 |  |
| Function AttributeName | 3287 | 306 | `SRCCore/Help.cs` Help.AttributeName | 未確認 |  |
| Sub AttributeHelp | 3595 | 28 | `SRCCore/Help.cs` Help.AttributeHelp | 未確認 |  |
| Function AttributeHelpMessage | 3625 | 922 | `SRCCore/Help.cs` Help.AttributeHelpMessage | 未確認 |  |
