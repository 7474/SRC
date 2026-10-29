# データビューア（SRC#DataViewer）

ユニット・パイロット・アイテムのデータを見るための Blazor WebAssembly のアプリ（`SRC.Sharp/SRCTestBlazor`）。シナリオはプレイしない。

| 置き場所 | データ |
|---|---|
| [7474.github.io/SRC](https://7474.github.io/SRC/)・Static Web Apps（[ワークフロー](../swa-workflows.md)） | [7474/SRC-Data](https://github.com/7474/SRC-Data) |
| [SRC#Site](https://src.koudenpa.net/viewer/)（[7474/SRC-Site](https://github.com/7474/SRC-Site)） | 7474/SRC-Data と、SRC#Site が公開しているシナリオ・素材・パックの zip |

## 頁

| 道 | 内容 |
|---|---|
| `/` | 手元のデータのファイル（`robot.txt`・`pilot.txt` など）を選んで見る |
| `/titles/…` | 7474/SRC-Data の作品のデータ（上のメニューの「Titles」から選ぶ） |
| `/archives` | 置いたサイトが公開している zip（シナリオ・素材・パック）の一覧。下の「アーカイブを読む」の設定があるときだけ、上のメニューに出る |
| `/archive?id=<ID>` | 1 つの zip の中身。タブで「データ」「画像」「音」「ファイル」を切り替える |

`/archive` の各タブ:

- **データ**: zip の中の `unit.txt`・`robot.txt`・`pilot.txt`・`non_pilot.txt`・`pilot_message.txt`・`pilot_dialog.txt`・`item.txt`（フォルダはどこでもよい）を読み、`/titles/…` と同じ形で見せる。ユニット・パイロットの画像は、その zip と、一緒に重ねる zip（`requires`）の `Bitmap/<分類>/` から探し、無ければ 7474/SRC-Data の画像を使う
- **画像**: BMP・PNG・JPEG・GIF をフォルダ・名前で絞って並べる。データで使っている画像には印を付け、「データで使っていないものだけ」に絞れる
- **音**: WAV・MP3・OGG をその場で鳴らす。MIDI はダウンロードだけ（この画面では鳴らせない）
- **ファイル**: zip の中のすべてのファイル

パック（`kind: pack`）は中の素材（`members`）をまとめて見せる。

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

### アーカイブの一覧

```json
{
  "format": 1,
  "name": "SRC#Site",
  "archives": [
    {
      "id": "lantern",
      "kind": "scenario",
      "title": "灯の巡航船ランタン号",
      "author": "…",
      "page": "/scenarios/lantern",
      "files": "/api/viewer/archives/scenarios/lantern/3.json",
      "base": "/files/scenarios/lantern/3/",
      "root": "lantern",
      "requires": ["src-basic", "resource.faces"]
    },
    { "id": "resource.faces", "kind": "resource", "title": "顔グラフィック", "pack": "user-credit", "files": "…", "base": "…" },
    { "id": "user-credit", "kind": "pack", "title": "投稿素材（作者表記）", "members": ["resource.faces"] }
  ]
}
```

| 項目 | 内容 |
|---|---|
| `id` | アーカイブの ID（`/archive?id=` と `requires`・`members` に使う） |
| `kind` | `scenario`・`resource`（素材）・`pack` |
| `title`・`author`・`description`・`license` | 表示に使う |
| `page` | サイトの頁 |
| `files` | zip の中のファイルの一覧の URL |
| `base` | zip の中のファイルの URL の前に付けるもの。ファイルの URL は `base` ＋ zip の中の道（区切りごとに URL エンコード） |
| `root` | zip の中のフォルダ。データの名前・画像の分類（`Bitmap/<分類>/`）はこの下で見る |
| `requires` | 一緒に重ねるアーカイブ（前から順に重ね、後ろほど優先する。最後に自分を重ねる） |
| `pack`・`members` | 素材の投稿先のパック・パックの中の素材 |

URL は、一覧の URL を基準に解決する。

### ファイルの一覧

```json
{ "files": [ { "path": "lantern/Bitmap/Pilot/Lantern_Sora.bmp", "size": 12342 } ] }
```

`path` は zip の中の道（区切りは `/`）。ファイルの中身は `base` ＋ `path` で取る。画像・音はブラウザが直接読むので、データビューアと同じオリジンか、CORS・CORP を許したところに置く。

SRC#Site の側の作りは [7474/SRC-Site の docs/resources.md](https://github.com/7474/SRC-Site/blob/main/docs/resources.md) にある。

## 表示の言語

[多言語対応](../i18n.md) の「データビューア」。
