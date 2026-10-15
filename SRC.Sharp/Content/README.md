# Content

SRC# で遊べるシナリオと、それを動かすための最小の汎用データ。SRC#Web（GitHub Pages）の組み込みのカタログで配信する。ライセンスは SRC# と同じ GPL-3.0。

| フォルダ・ファイル | 内容 |
|---|---|
| `lantern/` | 「灯の巡航船ランタン号」。全五話＋終話の短いオリジナルシナリオ。ヘルプにあるイベントコマンドをすべて使う（Flash を除く）。**生成AIを使って作った**（シナリオ・データ・BGM は Claude、顔グラフィックとユニットの画像は PixelLab。[lantern/README.md](lantern/README.md)） |
| `lantern.play` | ランタン号を最初から最後まで自動でプレイする手順（`srcsharp play` の PlayScript） |
| `basic-data/` | 自作の最小の汎用データ。地形データ・スペシャルパワーデータ・地形の画像・ユニットの画像 |

## basic-data

SRC のフルパッケージの汎用データは再配布できない（[素材データの再配布の検討](../../docs/assets/redistribution-study.md)）。そのため、同じファイル名で差し替えられる素材を自作した。

- 範囲: ランタン号が使う分だけ
  - 地形: 19 種類
  - ユニットの画像: 汎用データの `Bitmap/Unit` と同じ 22 種類。ランタン号は自前の画像（`lantern/Bitmap/Unit`）を使うため、汎用データの名前で画像を参照するシナリオ向け
  - スペシャルパワー: 20 種類
- 番号と名前: 地形の番号と名前、画像のファイル名は汎用データと同じにしている。汎用データを重ねれば、そちらの画像で表示される
- 作り方: `basic-data/make.py` が作る（Pillow が必要）。乱数の種を固定しているので、作り直しても同じになる
- 入っていないもの: 効果音・戦闘アニメ・MIDI の SoundFont。音は鳴らず、戦闘アニメは表示しない

## 動かし方

```sh
# 最後まで自動でプレイする
srcsharp play Content/lantern/01_灯の港.eve --script Content/lantern.play --app-path Content/basic-data
```

`basic-data/` を SRC のフォルダとして使う（汎用データの代わり）。
