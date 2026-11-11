# TerrainDataList.cls

VB 版の `SRC/SRC_20121125/TerrainDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 31 | 7 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList | 一致 | VB は全番号に空の TerrainData を作る。C# は null のままで、Item は null を返し、Map 側がそれを扱う |
| Sub Class_Terminate | 40 | 7 | （なし） | SRC#で不要 | GC が受け持つ |
| Function IsDefined | 50 | 7 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.IsDefined | 修正済み | 範囲外の番号で、Item が EmptyTerrain を返すので True になっていた。番号が範囲の中にあり、ID が 0 以上のときだけ True にした（#1498）。VB の範囲外は BoundsCheck を外しているので不定 |
| Function Item | 60 | 3 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Item・TerrainDataList.ItemByName | 修正済み | #1503 の手がかり: VB は名前で探すとき（ChangeTerrain・ChangeLayer・GUI のビットマップ）に OrderedID の順で探す。C# の ItemByName は番号の小さい順だったので、登録の順にした（#1498）。範囲外の番号で EmptyTerrain を返すのは SRC# の都合 |
| Function Name | 65 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.Name | 一致 | Item(ID).Name |
| Function Bitmap | 70 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.Bitmap | 一致 | Item(ID).Bitmap |
| Function Class | 75 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.Class | 一致 | Item(ID).Class |
| Function MoveCost | 80 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.MoveCost | 一致 | Item(ID).MoveCost |
| Function HitMod | 85 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.HitMod | 一致 | Item(ID).HitMod |
| Function DamageMod | 90 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.DamageMod | 一致 | Item(ID).DamageMod |
| Function IsFeatureAvailable | 97 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.IsFeatureAvailable | 一致 | Item(ID) 経由 |
| Function FeatureLevel | 101 | 3 | `SRCCore/Models/TerrainData.cs` TerrainData.FeatureLevel | 一致 | Item(ID) 経由 |
| Function FeatureData | 105 | 3 | `SRCCore/Models/FeatureData.cs` FeatureData.Data | 一致 | Item(ID).Feature(name) 経由 |
| Function OrderedID | 111 | 3 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.OrderedID・TerrainDataList.Count | 修正済み | C# に無かったので、登録の順の一覧として足した。DefinePlaceholder も登録の順に加える（#1498） |
| Sub Load | 117 | 193 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Load | 修正済み | (1) 地形効果の行を区切るとき、switch の中の break が for を抜けておらず、最後の「,」で区切っていた（1 行に 3 つ以上の地形効果があると誤る）。VB の Exit For と同じく最初の「,」で区切る (2) 定義済みの番号は VB と同じく TerrainData を使い回して .Clear し、登録の順を変えない (3) 名称を読む前のエラーのデータ名を VB と同じく空にした（#1498）。移動コストの CInt(2*CDbl) は VBMath.CLng で同じ。ErrorHandler（開けないときの「fname が開けません。」、打ち切る誤りの DataErrorMessage と TerminateSRC）は SRC.LoadDataFile が受け持つ。ゲームとして実行するとき（SRC.ShowDataErrorDialogs）だけダイアログを出して終了し、続ける誤りも SRC.AddDataError でダイアログに出す（#1550）。VB は ErrorHandler で TerminateSRC ではなく End するが、SRC# は画面の後始末を GUI に任せるため TerminateSRC で終える |
| Sub Clear | 312 | 8 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Clear | 一致 | 登録の順も消す。C# は配列を null にする（VB は名称などを残す）。SRC# では呼ばれていない |
