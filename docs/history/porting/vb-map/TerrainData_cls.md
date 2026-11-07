# TerrainData.cls

VB 版の `SRC/SRC_20121125/TerrainData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 39 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData | 一致 | ID = -1 |
| Sub Class_Terminate | 44 | 12 | （なし） | SRC#で不要 | GC が受け持つ |
| Sub AddFeature | 59 | 47 | `SRCCore/Models/TerrainData.cs` TerrainData.AddFeature | 修正済み | データの無い地形効果の StrData を、VB と同じく "" にした（#1498）。同じ名前の効果のキーを「名前＋個数」にする処理も同じ |
| Function CountFeature | 108 | 6 | `SRCCore/Models/TerrainData.cs` TerrainData.CountFeature | 一致 |  |
| Function Feature | 116 | 6 | `SRCCore/Models/TerrainData.cs` TerrainData.Feature | 一致 | C# は FeatureData を返す（名前は .Name）。VB では呼ぶ箇所が無い |
| Function FeatureName | 124 | 14 | `SRCCore/Models/FeatureData.cs` FeatureData.FeatureName | 一致 | VB では地形について呼ぶ箇所が無い |
| Function FeatureLevel | 140 | 14 | `SRCCore/Models/TerrainData.cs` TerrainData.FeatureLevel | 一致 | 無いとき 0、DEFAULT_LEVEL のとき 1 |
| Function FeatureData | 156 | 11 | `SRCCore/Models/FeatureData.cs` FeatureData.Data | 一致 | Status.bas の地形情報に当たるのは SRCRender の ScreenGUI.status.cs |
| Function IsFeatureAvailable | 169 | 14 | `SRCCore/Models/TerrainData.cs` TerrainData.IsFeatureAvailable | 一致 |  |
| Function IsFeatureLevelSpecified | 185 | 16 | `SRCCore/Models/FeatureData.cs` FeatureData.HasLevel | 一致 | VB では地形について呼ぶ箇所が無い |
| Sub Clear | 203 | 13 | `SRCCore/Models/TerrainData.cs` TerrainData.Clear | 修正済み | C# に無かったので足した。VB と同じく ID と地形効果だけを消す。TerrainDataList.Load で、定義済みの番号を読み直すときに使う（#1498） |
