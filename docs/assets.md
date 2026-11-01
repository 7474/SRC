# 素材

SRC# でのプレイに使う汎用データ（グラフィック・戦闘アニメ・効果音・BGM・地形データなど）の扱い。SRC のフルパッケージの素材を本リポジトリで再配布しないと決めた理由は [判断の記録](./decisions/2026-10-12-asset-redistribution.md) にある。

## 本リポジトリに含むもの・含まないもの

| 素材 | 扱い |
|---|---|
| SRC フルパッケージの汎用データ（グラフィック集・汎用戦闘アニメ・効果音集・地形データ・スペシャルパワー eve） | 含まない。利用者が [SRC 公式サイト](http://www.src-srpg.jpn.org/play_newinstall.shtml) から入手する |
| 自作の最小の汎用データ（`SRC.Sharp/Content/basic-data`） | 含む（CC0 1.0）。地形データ・スペシャルパワーデータ・地形とユニットの画像・効果音・BGM を、汎用データと同じ番号・ファイル名で作っている（[Content](../SRC.Sharp/Content/README.md)） |
| 遊べるシナリオ「灯の巡航船ランタン号」（`SRC.Sharp/Content/lantern`） | 含む（CC0 1.0） |
| E2E テストのコーパス（`SRC.Sharp/E2E/`）のデータと音声 | 含む。自作の最小のもの（[E2E テスト](./gui/e2e-testing.md)） |
| MIDI の SoundFont（.sf2） | リポジトリには含まない。SRC#Avalonia はビルド時に TimGM6mb（GPL-2.0）を取得して同梱し、手元の SoundFont が無いときに使う（[SoundFont](./gui/avalonia.md#soundfont)・[判断の記録](./decisions/2026-11-01-bundle-soundfont.md)）。SRC#Web の組み込みのカタログは、ビルド時に TimGM6mb（GPL-2.0）を取得して素材として配信する（[SRC#Web の音声](./gui/web.md#音声)） |

## 素材が無い場合の動作

SRC#Avalonia・SRC#Web・`srcsharp play`（`SRCRender` の `ScreenSession`）は `SRC.AllowMissingAssets = true` で動き、素材が無くても実行を続ける。SRC#Form は素材を必須とする。

- 起動時の環境チェック（`Bitmap`・`Sound` フォルダ）と `Data\System\terrain.txt` の不足は情報ログに留める
- マップが未定義の地形番号を使っている場合は、仮の地形（`TerrainDataList.DefinePlaceholder`。陸・移動コスト 1・修正なし）を登録して読み込む
- 画像の無い地形は地形の種類の色で塗り、地形番号ごとに明度を変えて枠を付ける。画像の無いユニットは陣営の色と名前の頭文字で表示する
- 効果音・BGM が無い場合は鳴らさない。スペシャルパワー・汎用戦闘アニメの eve は、ファイルが無ければ読み込まない

## 未対応

- フルパッケージを利用者側で取得・検証・展開するスクリプト
- グラフィック集・汎用戦闘アニメ・地形データ・スペシャルパワー eve を、公式サイト管理人の許諾を得て用途を限定して配布すること（許諾は得ていない）
