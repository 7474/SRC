# Item.cls

VB 版の `SRC/SRC_20121125/Item.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 31 | 6 | `SRCCore/Items/Item.cs` Item.Item | 一致 | Exist・Activated を True にし、Data・Unit を空にする |
| Sub Class_Terminate | 39 | 4 | （なし） | SRC#で不要 | 参照を外すだけ。GC が受け持つ |
| Property Get Name | 45 | 3 | `SRCCore/Items/Item.cs` Item.Name | 一致 |  |
| Property Let Name | 49 | 3 | `SRCCore/Items/Item.cs` Item.Name | 一致 | VB は IDList.Item のポインタを持つ。C# は名前（ItemName。JSON に保存する）を持ち、使うたびに IDList から引く |
| Function Nickname | 54 | 11 | `SRCCore/Items/Item.cs` Item.Nickname | 一致 | SelectedUnitForEvent を一時的に替えて式を置き換える |
| Function KanaName | 67 | 11 | `SRCCore/Items/Item.cs` Item.KanaName | 一致 | 同上 |
| Function Class | 80 | 3 | `SRCCore/Items/Item.cs` Item.Class | 一致 |  |
| Function Class0 | 84 | 24 | `SRCCore/Items/Item.cs` Item.Class0 | 一致 | Exit Do を return にした。「(」の無い「…専用)」で i が 0 になったとき、VB は Mid$ でエラー 5 になる（データの誤り） |
| Function Part | 110 | 3 | `SRCCore/Items/Item.cs` Item.Part | 一致 |  |
| Function HP | 115 | 3 | `SRCCore/Items/Item.cs` Item.HP | 一致 |  |
| Function EN | 120 | 3 | `SRCCore/Items/Item.cs` Item.EN | 一致 |  |
| Function Armor | 125 | 3 | `SRCCore/Items/Item.cs` Item.Armor | 一致 |  |
| Function Mobility | 130 | 3 | `SRCCore/Items/Item.cs` Item.Mobility | 一致 |  |
| Function Speed | 135 | 3 | `SRCCore/Items/Item.cs` Item.Speed | 一致 |  |
| Function CountFeature | 140 | 3 | `SRCCore/Items/Item.cs` Item.CountFeature | 一致 |  |
| Function Feature | 145 | 3 | `SRCCore/Items/Item.cs` Item.Feature | 一致 | VB は名前を返す。C# は FeatureData を返す（Variant の Index は string と int の多重定義に分けた） |
| Function FeatureName | 150 | 3 | `SRCCore/Items/Item.cs` Item.FeatureName | 一致 |  |
| Function FeatureLevel | 155 | 3 | `SRCCore/Items/Item.cs` Item.FeatureLevel | 一致 |  |
| Function FeatureData | 160 | 3 | `SRCCore/Items/Item.cs` Item.FeatureData | 一致 |  |
| Function FeatureNecessarySkill | 165 | 3 | `SRCCore/Items/Item.cs` Item.FeatureNecessarySkill | 一致 |  |
| Function IsFeatureAvailable | 170 | 3 | `SRCCore/Items/Item.cs` Item.IsFeatureAvailable | 一致 |  |
| Function Weapon | 175 | 3 | `SRCCore/Items/Item.cs` Item.Weapon | 一致 |  |
| Function CountWeapon | 180 | 3 | `SRCCore/Items/Item.cs` Item.CountWeapon | 一致 |  |
| Function Ability | 185 | 3 | `SRCCore/Items/Item.cs` Item.Ability | 一致 |  |
| Function CountAbility | 190 | 3 | `SRCCore/Items/Item.cs` Item.CountAbility | 一致 |  |
| Function Size | 195 | 3 | `SRCCore/Items/Item.cs` Item.Size | 一致 |  |
| Function IsAvailable | 201 | 218 | `SRCCore/Items/Item.cs` Item.IsAvailable | 一致 | 分岐ごとに突き合わせた（Disable・装備個所・武器クラスと防具クラス・必要技能と不必要技能・「～装備」・パイロット能力付加と強化・エリアス）。ALDList の AliasType のループは AliasData.HasType にした |
| Sub Dump | 422 | 8 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON のセーブ形式（SRC# の利便性）。ID・ItemName・UnitId・Exist・Activated を保存する |
| Sub Restore | 432 | 14 | `SRCCore/Items/Item.cs` Item.Restore | SRC#で不要 | JSON から読み込み、SRC の参照だけを戻す |
| Sub RestoreLinkInfo | 448 | 12 | `SRCCore/Items/Item.cs` Item.Unit | SRC#で不要 | Unit は JSON の UnitId で UList から引く。VB と同じく、ユニットが無ければ空になる |
| Sub RestoreParameter | 462 | 9 | （なし） | SRC#で不要 | VB も読み飛ばすだけ |
