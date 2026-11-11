# Map.bas

VB 版の `SRC/SRC_20121125/Map.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitMap | 87 | 19 | `SRCCore/Maps/Map.cs` Map.InitMap | 一致 | VB は SetMapSize の後で全マスの LayerType・LayerBitmapNo・BoxType を 0 にする。C# は SetMapSize の既定値（NO_LAYER_NUM・下層）のまま。BoxType 0 の VB は TDList(LayerType=0) を、C# は TerrainType=0 を引くので、どちらも地形 0 を見る。画像も描かないので結果は同じ。起動時に一度呼ぶだけ |
| Function TerrainEffectForHit | 108 | 13 | `SRCCore/Maps/MapCell.cs` MapCell.Terrain + TerrainData.HitMod | 一致 | Map.Terrain(x,y).HitMod に移した。BoxType で下層と上層を選ぶ分岐は MapCell.Terrain にある |
| Function TerrainEffectForDamage | 123 | 13 | `SRCCore/Maps/MapCell.cs` MapCell.Terrain + TerrainData.DamageMod | 一致 | 同上 |
| Function TerrainEffectForHPRecover | 138 | 13 | `SRCCore/Models/TerrainData.cs` TerrainData.EffectForHPRecover | 一致 | 10×FeatureLevel を Integer に入れるときの偶数丸めは VBMath.CLng で同じ |
| Function TerrainEffectForENRecover | 153 | 13 | `SRCCore/Models/TerrainData.cs` TerrainData.EffectForENRecover | 一致 | 同上 |
| Function TerrainName | 168 | 13 | `SRCCore/Maps/MapCell.cs` MapCell.Terrain + TerrainData.Name | 一致 |  |
| Function TerrainClass | 183 | 13 | `SRCCore/Maps/MapCell.cs` MapCell.Terrain + TerrainData.Class | 一致 |  |
| Function TerrainMoveCost | 198 | 13 | `SRCCore/Maps/MapCell.cs` MapCell.Terrain + TerrainData.MoveCost | 一致 |  |
| Function TerrainHasObstacle | 213 | 13 | `SRCCore/Maps/Map.cs` Map.Terrain + TerrainData.IsFeatureAvailable("衝突") | 一致 | 呼ぶ側は Unit.attackcheck.cs |
| Function TerrainHasMoveStop | 229 | 10 | `SRCCore/Models/TerrainData.cs` TerrainData.HasMoveStop | 一致 |  |
| Function TerrainDoNotEnter | 241 | 19 | `SRCCore/Models/TerrainData.cs` TerrainData.DoNotEnter | 一致 | 「侵入禁止」の互換もある |
| Function TerrainHasFeature | 262 | 10 | `SRCCore/Maps/Map.cs` Map.Terrain + TerrainData.IsFeatureAvailable | 一致 |  |
| Function UnitAtPoint | 275 | 11 | `SRCCore/Maps/Map.cs` Map.UnitAtPoint | 一致 |  |
| Function SearchTerrainImageFile | 288 | 106 | `SRCCore/Maps/Map.cs` Map.SearchTerrainImageFile・Map.SearchLayerImageFile | 修正済み | 地形か画像番号が NO_LAYER_NUM なら探さない判定を足した（#1498）。MapImageFileTypeData の代わりに、見つけたファイルのパスを残す。フォルダの有無を Static に覚えておく処理は FileSystem.FindFirstFile が受け持つ |
| Sub LoadMapData | 397 | 110 | `SRCCore/Maps/Map.cs` Map.LoadMapData | 修正済み | マップファイルが無いときに、共有の MapCell.EmptyCell を全マスに入れていた。これをマスごとに作るようにし、LayerBitmapNo を VB と同じ NO_LAYER_NUM にした（#1498）。未定義の地形のときは、VB は「定義されていないN番の地形データが使われています」を出して End する。C# はこれを内側の例外に入れ、「マップファイル「…」のデータが不正です」の TerminateException を投げる（ファイル名が分かるようにするための SRC# の都合なので直していない） |
| Sub SetMapSize | 509 | 75 | `SRCCore/Maps/Map.cs` Map.SetMapSize | 修正済み | MaskData を VB と同じく全マス True にした（#1498）。画面の部品（picBack・スクロールバー）は GUIMap.SetMapSize が受け持つ |
| Sub ClearMap | 586 | 43 | `SRCCore/Maps/Map.cs` Map.ClearMap | 修正済み | SetMapSize(1,1) を使うようになったので、VB と同じく MaskData(1,1) を False にした。UList.ClearUnitBitmap は SRC#で不要（ユニットを描くたびに描画モードを掛けている）。VB は MapX・MapY を変えないが、C# は SetMapSize で画面の中央に戻す。次にマップを読むときに SetMapSize で戻すので、動作は変わらない |
| Sub DumpMapData | 632 | 40 | `SRCCore/Maps/Map.cs` Map（JSON。MapFileNameForSave など）・`SRCCore/SRC.save.cs` | SRC#で不要 | SRC# は JSON で保存する。保存する項目（シナリオからの相対パスのファイル名・描画モード・マップ限定・幅と高さ・地形と画像番号・レイヤー・BoxType・MapX と MapY）は VB と同じ。「(マップ限定)」は MapDrawIsMapOnly として別に持つ |
| Function RestoreMapData | 677 | 135 | `SRCCore/Maps/Map.cs` Map.Restore・Map.IsMapChanged | 一致 | 読み込んだ後の状態（MapFileName・MapDrawMode・MapDrawIsMapOnly・マップの大きさ・マスのデータ・ユニットの配置・IsMapDirty・MapX と MapY）を VB と突き合わせた。違いは 3 つで、どれも直していない。(1) VB は保存したファイル名が空でも ScenarioPath を前に付けるので MapFileName が空でなくなる。C# は空のまま（VB の不具合）。(2) ClearUnitBitmap は SRC#で不要。(3) C# は SetMapSize を毎回呼ぶ（VB は大きさが変わったときだけ）。MaskData がリセットされるだけで影響は無い。フィルタの色と透過度は VB と同じく復元しない |
| Sub AreaInRange | 816 | 136 | `SRCCore/Maps/Map.cs` Map.AreaInRange | 一致 | ユニットを選び直す範囲は C# がマップ全体、VB は x1〜x2 だけ。範囲の外は True なので結果は同じ |
| Sub AreaInReachable | 955 | 133 | `SRCCore/Maps/Map.cs` Map.AreaInReachable | 修正済み | 「味方の敵」「ＮＰＣの敵」で、VB は暴走・魅了・憑依だけを見る（混乱・睡眠の味方は対象にしない）。C# は AreaInRange と同じ判定を使っていたので直した。既存のテストの期待値も VB に合わせた（#1498） |
| Sub AreaWithUnit | 1090 | 51 | `SRCCore/Maps/Map.cs` Map.AreaWithUnit | 一致 |  |
| Sub AreaInCross | 1143 | 32 | `SRCCore/Maps/Map.cs` Map.AreaInCross | 一致 |  |
| Sub AreaInLine | 1177 | 38 | `SRCCore/Maps/Map.cs` Map.AreaInLine | 一致 |  |
| Sub AreaInWideCross | 1217 | 76 | `SRCCore/Maps/Map.cs` Map.AreaInWideCross | 一致 |  |
| Sub AreaInCone | 1295 | 82 | `SRCCore/Maps/Map.cs` Map.AreaInCone | 一致 |  |
| Sub AreaInSector | 1379 | 121 | `SRCCore/Maps/Map.cs` Map.AreaInSector | 一致 |  |
| Sub AreaInSectorCross | 1502 | 15 | `SRCCore/Maps/Map.cs` Map.AreaInSectorCross | 一致 |  |
| Sub AreaInPointToPoint | 1519 | 40 | `SRCCore/Maps/Map.cs` Map.AreaInPointToPoint | 一致 | 途中の座標は VBMath.CLng で偶数丸めにしている |
| Sub AreaInSpeed | 1562 | 998 | `SRCCore/Maps/Map.cs` Map.AreaInSpeed | 修正済み | 地形適応は、すべての「地形適応」の能力から地形を集めるようにした（UnitMoveProps）。ＺＯＣ無効化・隣接ユニットＺＯＣ無効化のレベルは、Integer に偶数丸めしてから比べる。広域ＺＯＣ無効化の距離の判定では zarea を手続きの変数として扱い、前に判定したユニットの値が残る VB の動作に合わせた。扱わないエリアでは例外にせず、移動コストを 1000000 のままにする（FillMoveCost）。合体のための進入の判定を VB の For k に合わせた（MaskAlreadyUnitExist・CanCombineWith）。地形適応のデータが空のとき、VB は ErrorMessage と TerminateSRC で止まるが、SRC# は無視して続ける（未対応） |
| Sub AreaInTeleport | 2563 | 288 | `SRCCore/Maps/Map.cs` Map.AreaInTeleport | 修正済み | 封印された母艦（`IsDisabled("母艦")`）には着艦できないようにした。AreaInSpeed のほうは VB でも IsDisabled を見ない。合体の判定は AreaInSpeed と同じ直し方 |
| Sub AreaInMoveAction | 2853 | 268 | `SRCCore/Maps/Map.cs` Map.AreaInMoveAction | 修正済み | 侵入禁止の地形の判定で、すべての地形適応を見るようにした（UnitMoveProps）。移動制限・進入不可は、VB と同じく宇宙にいるユニットだけを判定することを確かめた |
| Function IsLineBlocked | 3123 | 74 | `SRCCore/Maps/Map.cs` Map.IsLineBlocked | 一致 |  |
| Sub NearestPoint | 3199 | 359 | `SRCCore/Maps/Map.cs` Map.NearestPoint | 修正済み | AreaInSpeed 用の FillMoveCost を流用していたのをやめ、VB の NearestPoint 独自の移動コストの表を移した（FillNearestPointMoveCost）。違っていた点: 深水・線路・ホバー移動（砂漠・月面）・ジャンプ移動はコスト 1、透過移動を見ない、地上の水は水上移動では渡れない、地中の月面は進入できない、宇宙は全地形がその地形のコスト、空中は空以外がコスト 2、扱わない地形はコスト 0、地形適応は最初の能力だけ見る。作業用の配列は 52×52 の固定をやめ、マップの大きさに合わせた |
| Sub SafetyPoint | 3560 | 67 | `SRCCore/Maps/Map.cs` Map.SafetyPoint | 修正済み | #1501 の手がかりを確かめた。X・Y を out から ref に変え、移動先が見つからなければ渡された値のまま残す（同じ距離の地点を比べる基準も、渡された X・Y になる）。呼び出し側の COM.cs（2 か所）と Unit.attackcheck.cs は、out を ref に変えただけ。配列はマップの大きさに合わせた |
| Sub SearchMoveRoute | 3630 | 178 | `SRCCore/Maps/Map.cs` Map.SearchMoveRoute | 修正済み | 現在位置（TotalMoveCost が 0 の地点）が見つからないとき、VB と同じく ox・oy を 0 にした（これまでは tx・ty だった）。経路の配列が 0 始まりなのは SRC# の都合 |
