# Content

SRC# で遊べるシナリオと、それを動かすための最小の汎用データ。SRC#Web（GitHub Pages）の組み込みのカタログで配信する。

## ライセンス

元の SRC から派生していないため、SRC# 本体（GPL-3.0）とは分けて、コンテンツを [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/deed.ja)、プログラムを MIT で配布する。

| 対象 | ライセンス |
|---|---|
| `lantern/`（シナリオ・データ・マップ・画像・BGM） | CC0 1.0（`lantern/LICENSE`） |
| `basic-data/` の素材（画像・効果音・BGM・地形データ・`Lib` のイベント） | CC0 1.0（`basic-data/LICENSE`） |
| `basic-data/make.py` | MIT（ファイルの先頭に記載） |
| `basic-data/Data/System/sp.txt` | GPL-3.0。スペシャルパワーの解説文を SRC のヘルプから写しているため |
| `lantern.play` | CC0 1.0 |

| フォルダ・ファイル | 内容 |
|---|---|
| `lantern/` | 「灯の巡航船ランタン号」。全五話＋終話の短いオリジナルシナリオ。ヘルプにあるイベントコマンドをすべて使う（Flash を除く）。**生成AIを使って作った**（シナリオ・データ・BGM は Claude、顔グラフィックとユニットの画像は PixelLab。[lantern/README.md](lantern/README.md)） |
| `lantern.play` | ランタン号を最初から最後まで自動でプレイする手順（`srcsharp play` の PlayScript） |
| `basic-data/` | 自作の最小の汎用データ。地形データ・スペシャルパワーデータ・地形の画像・ユニットの画像・効果音・BGM |

## basic-data

SRC のフルパッケージの汎用データは再配布できない（[素材の扱い](../../docs/assets.md)・[判断の記録](../../docs/decisions/2026-10-12-asset-redistribution.md)）。そのため、同じファイル名で差し替えられる素材を自作した。

- 範囲
  - 地形: 19 種類（ランタン号が使う分）
  - ユニットの画像: 汎用データの `Bitmap/Unit` と同じ 22 種類。ランタン号は自前の画像（`lantern/Bitmap/Unit`）を使うため、汎用データの名前で画像を参照するシナリオ向け
  - スペシャルパワー: 20 種類（ランタン号が使う分）
  - 効果音（`Sound/*.wav`）: 54 種類。汎用効果音集と同じ名前で、SRC# が武器の種類などから選ぶ戦闘の効果音（Beam・Gun・Missile・Sword・Explode など）と、ランタン号が `PlaySound` で鳴らす音（Explode(Long)・Explode(Nuclear)・Transform・Sharara・Applause）。元の SRC のシナリオでも戦闘の効果音が鳴る
  - BGM（`Midi/*.mid`）: 11 曲。SRC が場面ごとに鳴らす標準の名前（Opening・Briefing・Intermission・Subtitle、Map1・Map2（地上の味方・敵フェイズ）、Map3・Map4（屋内）、Map5・Map6（宇宙）、default（戦闘））。シナリオが `RenameBGM` などで別の曲を指定しなければ、これが鳴る（MIDI を鳴らすには SoundFont が要る）
- 番号と名前: 地形の番号と名前、画像・効果音・BGM のファイル名は汎用データ・元の SRC の標準と同じにしている。汎用データや MIDI を重ねれば、そちらで表示・再生される
- 作り方: `basic-data/make.py` が作る（Pillow が必要）。乱数の種を固定しているので、作り直しても同じになる
- 入っていないもの: 戦闘アニメ・MIDI の SoundFont。戦闘アニメは表示せず、MIDI は SoundFont が無いと鳴らない（SRC#Web の組み込みのカタログでは、ランタン号に SoundFont の TimGM6mb を重ねる）

### 生成法

basic-data の素材は、すべて生成AIの Claude（Anthropic）が書いたプログラム（`make.py`）と文書で作った。**画像生成AI・音声生成AI・作曲AIは使っていない。** 録音した音や、他から持ってきた素材・曲も使っていない。

| 素材 | 作り方 |
|---|---|
| 地形の画像（`Bitmap/Map`）・ユニットの画像（`Bitmap/Unit`） | `make.py` が Pillow で、図形と点描を組み合わせて 32×32 に描く |
| 効果音（`Sound`） | `make.py` が数式で合成する。ホワイトノイズ（ローパス・ハイパスで音色を変える）・正弦波・矩形波・ノコギリ波・三角波に、周波数の変化と音量の変化（立ち上がりと減衰）を付けて重ねる。例: 銃声は短いノイズと下がる打撃音、爆発は低いノイズと下がる低音、ビームは下がる矩形波。22050 Hz・16 ビット・モノラルの WAV |
| BGM（`Midi`） | `make.py` が規則で作曲し、標準 MIDI ファイルに書く。曲ごとに主音・音階（長調・短調）・テンポ・和音の進行・音色・ドラムの型を決め、伴奏（和音）・ベース（根音）・メロディ（和音の音を中心に、前の音から近い音へ動く。リズムは乱数で 2 つ作り交互に使う）・ドラムを並べる。乱数の種は曲名 |
| 地形データ（`Data/System/terrain.txt`） | `make.py` が書く。地形の番号と名前は汎用データに合わせた |
| スペシャルパワーデータ（`Data/System/sp.txt`） | `make.py` が書く。効果の説明はヘルプ（スペシャルパワー.md）から写した（そのため GPL-3.0） |
| `Lib` のイベント（ユニット・パイロットステータス表示） | ヘルプ（ユニットステータス.md・MakeUnitListコマンド.md など）をもとに、Claude が手で書いた |

## 動かし方

```sh
# 最後まで自動でプレイする
srcsharp play Content/lantern/01_灯の港.eve --script Content/lantern.play --app-path Content/basic-data
```

`basic-data/` を SRC のフォルダとして使う（汎用データの代わり）。
