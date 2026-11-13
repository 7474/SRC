# データビューア（SRC#DataViewer）

ユニット・パイロット・アイテムのデータを見るための Blazor WebAssembly のアプリ（`SRC.Sharp/SRCDataViewer`）。シナリオはプレイしない。

| 置き場所 | データ |
|---|---|
| [7474.github.io/SRC](https://7474.github.io/SRC/)・Static Web Apps（[ワークフロー](../swa-workflows.md)） | [7474/SRC-Data](https://github.com/7474/SRC-Data) |
| [SRC#Site](https://src.koudenpa.net/viewer/)（[7474/SRC-Site](https://github.com/7474/SRC-Site)） | 7474/SRC-Data と、SRC#Site が公開しているシナリオ・素材・パックの zip |

## 頁

| 道 | 内容 |
|---|---|
| `/` | 手元のデータのファイル（`robot.txt`・`pilot.txt` など）を選んで見る |
| `/titles/…` | 7474/SRC-Data の作品のデータ（上のメニューの「Titles」から選ぶ） |
| `/compare` | 組の評価を比べる（下の「比べる」） |
| `/archives` | 置いたサイトが公開している zip（シナリオ・素材・パック）の一覧。下の「アーカイブを読む」の設定があるときだけ、上のメニューに出る |
| `/archive?id=<ID>` | 1 つの zip の中身。タブで「データ」「画像」「音」「ファイル」を切り替える。`index=<一覧の道>` を付けると、その一覧も読んでから開く（置いたサイトの中の道だけ。サイトが承認前の投稿などを見せるのに使う） |

`/archive` の各タブ:

- **データ**: zip の中の `unit.txt`・`robot.txt`・`pilot.txt`・`non_pilot.txt`・`pilot_message.txt`・`pilot_dialog.txt`・`item.txt`（フォルダはどこでもよい）を読み、`/titles/…` と同じ形で見せる。ユニット・パイロットの画像は、その zip と、一緒に重ねる zip（`requires`）の `Bitmap/<分類>/` から探し、無ければ 7474/SRC-Data の画像を使う。パイロットの BGM とユニットの特殊能力「ＢＧＭ」は、その zip と重ねる zip から SRC と同じ規則（左の候補から順に、`Midi` フォルダ → SRC のフォルダ、同じ名前の .mp3 を .mid より先）で探し、見つかればボタンで鳴らせる（見つからなければ名前だけを出す）
- **画像**: BMP・PNG・JPEG・GIF をフォルダ・名前で絞って並べる。データで使っている画像には印を付け、「データで使っていないものだけ」に絞れる
- **音**: WAV・MP3・OGG をその場で鳴らす。MIDI は「合成して鳴らす」で、SoundFont（.sf2）を使って WAV に合成してから鳴らす（ブラウザは MIDI を鳴らせないため。SRC#Web と同じ [MeltySynth](https://github.com/sinshu/meltysynth)）。SoundFont は、その zip と一緒に重ねる zip にある .sf2（後ろほど優先）、無ければ一覧の `soundFont` の zip から探す。見つからなければダウンロードだけ。合成するのは 10 分まで
- **ファイル**: zip の中のすべてのファイル

パック（`kind: pack`）は中の素材（`members`）をまとめて見せる。

### BGM の再生

データの BGM のボタンと、「音」タブの MIDI の「合成して鳴らす」は、画面の下の再生の帯（`BgmBar`。`MainLayout` に置く）で鳴らす。帯は画面を移っても残り、曲は繰り返して鳴り続ける。「止める」で止める。一度合成した MIDI は、頁を読み込み直すまで合成し直さない。

## 比べる

ユニットとパイロットの組を、ゲームと同じ計算で評価した結果（7474/SRC-Data の Pages の `analysis/*.json`。SRC の `tools/pack-analysis --json` が書く）を比べて楽しむ頁（`/compare`。判断は [データの比較を楽しむための道具として整える](../decisions/2026-11-14-data-comparison.md)）。評価は、SRC-Data のデータと役割表（`analysis/`）が変わったときと毎週、Pages で作り直す。

| タブ | 内容 |
|---|---|
| ランキング | 組を指数（実効性能・単純数字を切り替える）か使いどころの評価（上位何 %）で並べる。区分（ロボ・拡張・Sharp）・帯・格・名前で絞る。帯の既定は最上位の帯（指数は帯の中での比べ方なので、帯を混ぜると名前なしの外れ値が上に並ぶ） |
| 横並べ | ランキングで選んだ組（4 組まで）を、画像・レーダーチャート（実効性能は実線、単純数字は破線、250 で頭打ち）・指数・評価・能力値で並べる。選んだ組は URL（`pairs`）に残るので、共有できる |
| 作品 | 作品ごとの組の数・役割表で組んだ数・選んだ値の中央値と最大・いちばんの組。役割表の無い作品だけに絞れる |
| 位置 | 自分のデータ（区分 `Sharp`、区分を選べばその区分）の組が、同じ帯のほかの区分の組の中で上から何 % の位置か |

- 作品の頁のパイロット・ユニットの「比べる」から、その名前で絞ったランキングへ行ける（`compare?band=&q=名前`）
- URL の `tab`・`section`・`band`・`q`・`pairs` で、開くタブと絞り込みを決められる
- 評価を読めなければ、その旨を出す。ほかの頁は今までどおり動く

## アーカイブを読む

データビューアは、置き場所（`<base href>`）の `viewer.json` を起動時に読む。無い・読めないときは今までどおり 7474/SRC-Data だけを使う。

```json
{
  "archiveIndexes": ["/api/viewer/index.json"]
}
```

| 項目 | 内容 |
|---|---|
| `archiveIndexes` | アーカイブの一覧の URL。相対ならデータビューアの置き場所から |
| `analysisIndex` | 組の評価の一覧（`analysis/index.json`）の URL。無ければ 7474/SRC-Data の Pages（`https://7474.github.io/SRC-Data/analysis/index.json`） |

### アーカイブの一覧

```json
{
  "format": 1,
  "name": "SRC#Site",
  "soundFont": "packs/soundfont",
  "archives": [
    {
      "id": "scenarios/lantern",
      "kind": "scenario",
      "title": "灯の巡航船ランタン号",
      "author": "…",
      "page": "/scenarios/lantern",
      "files": "/api/viewer/archives/scenarios/lantern/3.json",
      "base": "/files/scenarios/lantern/3/",
      "root": "lantern",
      "requires": ["packs/src-basic", "packs/faces"]
    },
    {
      "id": "packs/faces",
      "kind": "pack",
      "title": "顔グラフィック",
      "page": "/packs/faces",
      "files": "/api/viewer/archives/packs/faces/5-8a9d11ee.json",
      "base": "/files/packs/faces/5-8a9d11ee/"
    }
  ]
}
```

| 項目 | 内容 |
|---|---|
| `id` | アーカイブの ID（`/archive?id=` と `requires`・`members` に使う）。一覧の中で重ならないようにする（SRC#Site は種類を前に付ける。`scenarios/<ID>`・`packs/<ID>`） |
| `kind` | `scenario`・`pack`・`resource`（素材） |
| `title`・`author`・`description`・`license` | 表示に使う |
| （一覧の）`soundFont` | 任意。MIDI を鳴らす SoundFont（.sf2）を探すアーカイブの ID |
| `page` | サイトの頁 |
| `files` | zip の中のファイルの一覧の URL |
| `base` | zip の中のファイルの URL の前に付けるもの。ファイルの URL は `base` ＋ zip の中の道（区切りごとに URL エンコード） |
| `root` | zip の中のフォルダ。データの名前・画像の分類（`Bitmap/<分類>/`）はこの下で見る |
| `requires` | 一緒に重ねるアーカイブ（前から順に重ね、後ろほど優先する。最後に自分を重ねる） |
| `pack`・`members` | 任意。素材を 1 つずつのアーカイブで出すサイトが、素材の投稿先のパック・パックの中の素材（重ねる順）を書く。`members` を持つパックは、中の素材をまとめて見せる |

URL は、一覧の URL を基準に解決する。

### ファイルの一覧

```json
{ "files": [ { "path": "lantern/Bitmap/Pilot/Lantern_Sora.bmp", "size": 12342 } ] }
```

`path` は zip の中の道（区切りは `/`）。ファイルの中身は `base` ＋ `path` で取る。画像・音はブラウザが直接読むので、データビューアと同じオリジンか、CORS・CORP を許したところに置く。

SRC#Site の側の作りは [7474/SRC-Site の docs/resources.md](https://github.com/7474/SRC-Site/blob/main/docs/resources.md) にある。

## 表示の言語

[多言語対応](../i18n.md) の「データビューア」。
