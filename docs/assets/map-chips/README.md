# 汎用グラフィック集のマップチップの番号と意味

元の SRC の汎用グラフィック集（SRC フルパッケージ 2.2.33 の `Bitmap/Map`、84 地形・7,215 枚）について、**地形ごとに、絵の番号が何を表すか**を書いたデータ（7474/SRC#1663）。マップの各マスは「地形＋絵の番号」を持ち（`<画像名><番号:0000>.bmp`。SRCCore の `Map.SearchMapImageFile`）、汎用グラフィック集は岸辺・角・つながり・大きな建物の部品を番号で描き分けている。

使い道:

- 自作の最小の汎用データ（`SRC.Sharp/Content/basic-data`）を、**汎用グラフィック集と同じ番号を同じ意味で**描く。汎用グラフィック集を重ねても（SRC-Site の共有パック）、どちらで作ったマップも正しく見える
- マップ作成ツール・場面のカタログで、まわりの地形に合わせて境目の番号を選ぶ（自動でつなぐ）

**ここにあるのは番号の意味の文章だけで、汎用グラフィック集の絵は含まない**（[再配布しない判断](../../decisions/2026-10-12-asset-redistribution.md)）。番号と意味は、フルパッケージの絵を見て書いた。

## ファイル

`<画像名>.json`（`terrain.txt` の画像名。例: 海は `sea.json`）。地形の番号との対応は汎用データの `Data/System/terrain.txt`（[マップデータ](../../../SRC.Sharp.Help/src/マップデータ.md)）。

```json
{
  "terrain": "sea",
  "count": 175,
  "exists": "0-61 500-511 1000-1014 2000-2056 2500-2527 3000",
  "ranges": [
    {
      "from": 1, "to": 4,
      "kind": "edge",
      "style": "default",
      "neighbor": "plain",
      "tiles": { "1": ["S"], "2": ["W"], "3": ["N"], "4": ["E"] },
      "description": "…",
      "confidence": "high"
    }
  ],
  "rules": "番号の付け方の規則（読み取れたもの）"
}
```

| 項目 | 意味 |
|---|---|
| `terrain` | 画像名（ファイル名の頭） |
| `count` | 絵の枚数 |
| `exists` | 実在する番号（空白区切りの範囲）。`ranges` はこの番号をちょうど一度ずつ覆う |
| `ranges[].from`・`to` | 同じ意味の番号の範囲（範囲の中の実在しない番号は無視する） |
| `ranges[].kind` | `base`（基本の絵）・`variant`（変種）・`edge`（辺の境目）・`corner`（外角）・`inner_corner`（内角）・`strip`・`end`・`junction`（道・川・線路などの線のつながり）・`object`（何マスかで組む大きな物の部品）・`style`（別の画風の組）・`animation`・`decoration`・`placeholder`（使えない仮の絵）・`other` |
| `ranges[].style` | 地形に画風の組が複数あるときの組の名前（`default`・`dark`・`snow` など） |
| `ranges[].neighbor` | 境目に接する別の地形（`plain`・`sand`・`snow`・`cliff`・`space` など）。線のつながりは `connects` |
| `ranges[].tiles` | 番号ごとの方角。境目は別の地形がある辺・角（`N`・`E`・`S`・`W`・`NE`・`SE`・`SW`・`NW`。`N` が上）、線は抜ける方角。`{"番号": ["N", "W"]}`、または `{"番号": {"sides": [...], "neighbor": "...", "confidence": "..."}}`（番号ごとに接する地形や確かさが違うとき） |
| `ranges[].object` | 大きな物の大きさ（`width`・`height`）と番号の並び |
| `ranges[].confidence` | `high`（はっきり読めた）・`guess`（推測） |

## 確かさと残り

- 番号の範囲と意味は全 84 地形・全番号について書いた。小さな境目の絵は、2 倍・4 倍に拡大して見た
- 番号ごとの方角（`tiles`）は、屋外のマップで「自動でつなぐ」に要る地形（海・川・道路・崖・山・林・砂浜・平地・線路・橋・深海・火星の崖・クレーター・月の海・沼・街・洞窟）を埋めた。壁・床・大きな障害物・運動場・基地などの屋内・建物の組は、範囲ごとの意味だけで、番号ごとの方角はまだ無い
- `confidence: "guess"` の範囲・番号は、使う前に絵で確かめる

テスト: `SRCAuthoringTests/MapChipCatalogTests.cs`（書式・番号の重なりと漏れ・方角の値）。
