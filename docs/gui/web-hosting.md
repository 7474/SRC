# SRC#Web を外部のサイトに置く

SRC#Web（ブラウザ版）は静的ファイルだけで動くため、どこの静的ホスティングにも置けます。置いたサイトでは次のことができます。

- URL を開くと、決まったシナリオをすぐに始める
- そのサイトが配信するシナリオを一覧から選んで遊ぶ
- 利用者が、手元のシナリオや素材（汎用グラフィック集など）の zip を追加して遊ぶ
- シナリオに、汎用データなどの素材を重ねて配信する

設計の全体は [SRC#Web](./web.md) を参照してください。

## 置き方

1. [Releases](https://github.com/7474/SRC/releases) の `SRCSharpWeb.zip` を展開する。自分でビルドする場合は `dotnet workload install wasm-tools` の後に `dotnet publish SRC.Sharp/SRCSharpWeb/SRCSharpWeb.csproj -c Release -o out` を実行し、`out/wwwroot` を使う。
2. サイトのルート以外（例: `https://example.com/games/src/`）に置く場合は、`index.html` の `<base href="/" />` を置いた場所のパスに書き換える（例: `<base href="/games/src/" />`）。
3. 配信するシナリオの zip とカタログ（`catalog.json`）を置き、必要ならサイトの設定（`srcweb.json`）を置く。
4. HTTPS で配信する。

### ホスティングの条件

| 条件 | 理由 |
|---|---|
| HTTPS（または localhost） | Service Worker と SharedArrayBuffer に必要 |
| クロスオリジン分離 | ゲームを別スレッドで動かすため。ヘッダ `Cross-Origin-Opener-Policy: same-origin` と `Cross-Origin-Embedder-Policy: require-corp` を付けられるならそれでよい。付けられない場合（GitHub Pages など）は同梱の `coi-serviceworker.js` が付ける（初回だけ自動で再読み込みされる） |
| `.wasm` などの配信 | `application/wasm` の MIME タイプで返す。多くのホスティングは対応済み |
| 他のサイトの zip・カタログを読む場合 | 配信元が CORS（`Access-Control-Allow-Origin`）を許可している必要がある。クロスオリジン分離のページから読むため、`Cross-Origin-Resource-Policy: cross-origin` も必要な場合がある |

## サイトの設定（srcweb.json）

サイトのルート（`index.html` と同じ場所）に `srcweb.json` を置くと、見出しや配信するシナリオ、利用者に許す操作を決められます。置かなければ既定値で動きます。

```json
{
  "title": "○○シナリオ集",
  "catalogs": ["scenarios/catalog.json", "https://assets.example.com/catalog.json"],
  "allowUserArchives": true,
  "allowUserCatalogs": true,
  "start": null
}
```

| 項目 | 既定値 | 内容 |
|---|---|---|
| `title` | `SRC#Web` | 画面の見出しとページのタイトル |
| `catalogs` | `["scenarios/catalog.json"]` | 読み込むカタログの URL。サイトからの相対 URL か絶対 URL |
| `allowUserArchives` | `true` | 利用者が zip（シナリオ・素材）を追加できるか |
| `allowUserCatalogs` | `true` | 利用者がカタログの URL を追加できるか。URL の `catalog`・`archive` パラメータも、これが `true` のときだけ使える |
| `start` | なし | 開いたらすぐに始めるシナリオ（カタログの `id`）。シナリオ 1 本だけを配信するサイト向け |

## カタログ（catalog.json）

配信するシナリオと素材の一覧です。zip の URL はカタログの URL からの相対 URL でも書けます。

```json
{
  "format": 1,
  "name": "○○シナリオ集",
  "entries": [
    {
      "id": "my-scenario",
      "title": "私のシナリオ",
      "author": "作者",
      "license": "再配布の条件など",
      "description": "説明",
      "archive": "my-scenario.zip",
      "root": "私のシナリオ",
      "eve": "start.eve",
      "requires": ["general-data"]
    },
    {
      "id": "general-data",
      "kind": "asset",
      "title": "汎用データ",
      "archive": "assets/general-data.zip",
      "root": "汎用データ"
    }
  ]
}
```

| 項目 | 内容 |
|---|---|
| `id` | エントリの名前。URL の `play` で指定する。セーブデータもこの名前ごとにブラウザに保存される |
| `kind` | `scenario`（既定）・`asset`（素材）。素材は一覧に出さず、シナリオの `requires` から使う |
| `archive` | zip の URL |
| `root` | zip の中のフォルダ。そのフォルダ以下だけを使う（zip がフォルダごと圧縮されている場合に指定する） |
| `eve` | はじめに実行するイベントファイル（`root` からのパス）。セーブデータが無ければ、選んだらすぐに始める |
| `requires` | 一緒に使う素材の `id` |
| `title`・`author`・`license`・`description` | 一覧に表示する情報 |

エントリの一覧だけの配列（`[{ "id": …, "archive": … }]`）も読めます。

### 素材（汎用データの同梱）

`kind: "asset"` の zip は、元の SRC で素材を SRC のフォルダに置くのと同じ扱いで、SRC のフォルダ（`Bitmap`・`Sound`・`Data` などがある場所）に展開します。シナリオの `requires` に書いた素材を、そのシナリオを始める前に展開します。素材の zip は、SRC のフォルダの中身（`Bitmap/…`・`Sound/…`）の形にしてください（フォルダごと圧縮した場合は `root` を指定）。

利用者が「素材の zip」として追加したものも、どのシナリオにも重ねます。重ねる順は「利用者の素材 → シナリオが求める素材」で、後のものが優先されます（シナリオのフォルダにあるファイルはさらに優先）。

これにより、汎用グラフィック集などの素材を別の配信元から使ったり、シナリオと一緒に汎用データを配信したりできます。素材を配信する場合、その素材の再配布の条件を守るのは配信するサイトの責任です（[素材データの再配布の検討](../assets/redistribution-study.md)）。

## URL で開く

| URL | 動作 |
|---|---|
| `?play=my-scenario` | カタログの `my-scenario` をすぐに始める |
| `?catalog=https://example.com/catalog.json` | そのカタログも一覧に出す（その表示の間だけ。保存はしない） |
| `?catalog=…&play=…` | 別のサイトのカタログのシナリオをすぐに始める |
| `?archive=https://example.com/a.zip&eve=start.eve` | zip を直接読んで始める。`root`・`requires`（素材の `id` をカンマ区切り）も指定できる |

一覧の各シナリオの「リンク」が、そのシナリオを直接開く URL です。`catalog`・`archive` は `allowUserCatalogs` が `true` のときだけ使えます。

## 利用者が追加するもの

`allowUserArchives` が `true` のとき、利用者は一覧の画面で次のものを追加できます。追加したものはブラウザ（IndexedDB）に保存され、次回も使えます。

- シナリオの zip: 一覧から選んで遊ぶ
- 素材の zip: どのシナリオにも重ねる

`allowUserCatalogs` が `true` のとき、カタログの URL を追加できます（localStorage に保存）。

## 安全性

シナリオは SRC のイベントとして解釈されるだけで、スクリプト（JavaScript）は実行されません。zip の展開では、展開先の外を指すパスを無視します。
