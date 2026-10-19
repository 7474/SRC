# SRC#Web を外部のサイトに置く

SRC#Web（ブラウザ版）は静的ファイルだけで動くため、どこの静的ホスティングにも置けます。置いたサイトでは次のことができます。

- URL を開くと、決まったシナリオをすぐに始める
- そのサイトが配信するシナリオを一覧から選んで遊ぶ
- 利用者が、手元のシナリオや素材（汎用グラフィック集など）の zip を追加して遊ぶ
- シナリオと一緒に、汎用データなどの素材を配信して読み込ませる

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
  "start": null,
  "language": null
}
```

| 項目 | 既定値 | 内容 |
|---|---|---|
| `title` | `SRC#Web` | 画面の見出しとページのタイトル |
| `catalogs` | `["scenarios/catalog.json"]` | 読み込むカタログの URL。サイトからの相対 URL か絶対 URL |
| `playCatalog` | なし | URL の `play` で開いたときに読む、シナリオごとのカタログの URL。`{id}` をシナリオの `id`（URL エンコードしたもの）に置き換える（例: `/api/catalog/{id}.json`）。指定すると `catalogs` をすべて読まずに、そのカタログだけからシナリオと `requires` の素材を探して始める。読めない・シナリオか素材が無い場合は `catalogs` を読む。URL に `catalog`・`archive` があるときは使わない |
| `allowUserArchives` | `true` | 利用者が zip（シナリオ・素材）を追加できるか。`false` なら下の 2 つにかかわらず追加できない |
| `allowUserScenarios` | `true` | 利用者がシナリオの zip を追加できるか |
| `allowUserAssets` | `true` | 利用者が素材の zip を追加できるか。`false` なら以前に追加した素材も一覧に出さず、読み込まない（サイトの素材だけで遊ばせる） |
| `ratingConfirm` | `false` | カタログの `rating`（年齢の区分）を使うか。`true` なら `r15`・`r18` のシナリオに印を付け、始める前に年齢を確かめる。`false` なら `rating` を無視する |
| `allowUserCatalogs` | `true` | 利用者がカタログの URL を追加できるか。URL の `catalog`・`archive` パラメータも、これが `true` のときだけ使える |
| `start` | なし | 開いたらすぐに始めるシナリオ（カタログの `id`）。シナリオ 1 本だけを配信するサイト向け |
| `language` | なし（日本語） | 表示言語の既定。対応言語のカルチャ名（`ja`・`en`・`zh-Hans` など 15 の言語）か `auto`。利用者が画面で選んだ言語が優先する（[多言語対応](../i18n.md#srcweb)） |
| `errorReporting` | なし（送らない） | エラーの収集。下の「エラーの収集」を参照 |

### エラーの収集

`errorReporting.mackerel` を書くと、エラーが起きたときだけ、その内容を [Mackerel のトレース](https://mackerel.io/ja/docs/entry/tracing/guide/client-token)（クライアントトークン）へ OpenTelemetry のトレース（OTLP/HTTP JSON）として送ります。書かなければ何も送りません。

```json
{
  "errorReporting": {
    "mackerel": {
      "clientToken": "（Mackerel のクライアントトークン）",
      "serviceName": "srcsharp-web"
    }
  }
}
```

| 項目 | 既定値 | 内容 |
|---|---|---|
| `clientToken` | なし | Mackerel のクライアントトークン（`X-Mackerel-Client-Token` ヘッダで送る）。無ければ送らない |
| `serviceName` | `srcsharp-web` | トレースのサービス名（`service.name`） |
| `endpoint` | `https://otlp-vaxila.mackerelio.com/v1/traces` | 送り先 |

- 送るエラー: 未処理の例外・Promise の reject・`console.error`（.NET の例外や `[MONO]` の出力を含む）、ゲームの例外、シナリオのエラーメッセージ（`ErrorMessage`）
- 1 つのエラーを 1 つのスパン（状態はエラー）にし、次を付けます
  - 直前のコンソールのログ（最大 30 件）をスパンのイベント `log` に、エラーの内容をイベント `exception` に
  - 実行中のシナリオの行（`srcweb.scenario.line`、`ファイル名:行番号 行の内容`）と画面の状態（`srcweb.screen`）
  - ページの URL（`url.full`）とブラウザ（`user_agent.original`）。`deployment.environment.name` はサイトのホスト名
- 同じエラーは 1 度だけ、1 回の読み込みで最大 10 件まで送ります
- クライアントトークンはページから読めます。Mackerel 側でトレースの投稿にしか使えない権限ですが、公開するサイトに置く前に [クライアントトークンの説明](https://mackerel.io/ja/docs/entry/tracing/guide/client-token) を確認してください

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
| `version` | zip の版（任意の文字列）。取得した zip はブラウザにキャッシュし、次からはネットワークを使わない。zip を同じ URL のまま差し替えたら、`version` を変える（ハッシュ値や日付など）。省くと、利用者がブラウザのサイトデータを消すまで最初に取得したものを使い続ける |
| `root` | zip の中のフォルダ。そのフォルダ以下だけを使う（zip がフォルダごと圧縮されている場合に指定する） |
| `eve` | はじめに実行するイベントファイル（`root` からのパス）。セーブデータが無ければ、選んだらすぐに始める |
| `requires` | 一緒に使う素材の `id` |
| `rating` | 年齢の区分。`general`（既定）・`r15`・`r18`。サイトの設定の `ratingConfirm` が `true` のときだけ使う。`r15`・`r18` のシナリオは一覧に「R15」「R18」の印を付け、始める前（一覧から選んだとき・URL の `play`・`start`）に「15 歳（18 歳）以上ですか」と確かめる。「はい」と答えた区分はブラウザ（localStorage）に覚え、次からは確かめない（`r18` に答えれば `r15` も確かめない）。「いいえ」なら始めずに一覧に留まる |
| `title`・`author`・`license`・`description` | 一覧に表示する情報 |

エントリの一覧だけの配列（`[{ "id": …, "archive": … }]`）も読めます。

### 素材（汎用データの同梱）

`kind: "asset"` の zip は、元の SRC で素材を SRC のフォルダに置くのと同じ扱いで、SRC のフォルダ（`Bitmap`・`Sound`・`Data` などがある場所）に展開します。シナリオの `requires` に書いた素材を、そのシナリオを始める前に展開します。素材の zip は、SRC のフォルダの中身（`Bitmap/…`・`Sound/…`）の形にしてください（フォルダごと圧縮した場合は `root` を指定）。

利用者が「素材の zip」として追加したものも、どのシナリオでも読み込みます。読み込む順は「シナリオが求める素材 → 利用者の素材」で、後のものが優先されます（シナリオのフォルダにあるファイルはさらに優先）。同じ名前のファイルがあれば利用者の素材が使われるため、利用者は手元の BGM・効果音などでサイトの素材を差し替えられます。

MIDI の SoundFont（.sf2）も素材として配信できます。シナリオのアーカイブに .sf2 が無ければ、素材に含まれる .sf2 で MIDI を合成します（[SRC#Web の音声](./web.md)）。

これにより、汎用グラフィック集などの素材を別の配信元から使ったり、シナリオと一緒に汎用データを配信したりできます。素材を配信する場合、その素材の再配布の条件を守るのは配信するサイトの責任です（[素材データの再配布の検討](../assets/redistribution-study.md)）。

## URL で開く

| URL | 動作 |
|---|---|
| `?play=my-scenario` | カタログの `my-scenario` をすぐに始める。サイトの設定に `playCatalog` があれば、そのシナリオのカタログだけを読む |
| `?catalog=https://example.com/catalog.json` | そのカタログも一覧に出す（その表示の間だけ。保存はしない） |
| `?catalog=…&play=…` | 別のサイトのカタログのシナリオをすぐに始める |
| `?archive=https://example.com/a.zip&eve=start.eve` | zip を直接読んで始める。`root`・`requires`（素材の `id` をカンマ区切り）も指定できる |
| `?log=Debug` | ブラウザのコンソールに出すログのレベル（`Trace`・`Debug`・`Information`・`Warning`・`Error`）。ほかの指定と組み合わせられる（[ログの設計](../logging.md)） |

一覧の各シナリオの「リンク」が、そのシナリオを直接開く URL です。`catalog`・`archive` は `allowUserCatalogs` が `true` のときだけ使えます。

## 利用者が追加するもの

`allowUserArchives` が `true` のとき、利用者は一覧の画面で次のものを追加できます。追加したものはブラウザ（IndexedDB）に保存され、次回も使えます。

- シナリオの zip（`allowUserScenarios`）: 一覧から選んで遊ぶ
- 素材の zip（`allowUserAssets`）: どのシナリオでも読み込む。シナリオが求める素材（`requires`）の後に読み込むので、同じ名前のファイルは利用者のものが使われる

シナリオ 1 本だけを配信するサイトの例（利用者の追加を許さず、開いたらすぐに始める）:

```json
{
  "title": "○○",
  "catalogs": ["scenarios/catalog.json"],
  "start": "my-scenario",
  "allowUserArchives": false,
  "allowUserCatalogs": false
}
```

利用者に手元の BGM などで差し替えさせたいときは、`"allowUserArchives": true, "allowUserScenarios": false` にする（素材だけを追加できる）。

`allowUserCatalogs` が `true` のとき、カタログの URL を追加できます（localStorage に保存）。

## 安全性

シナリオは SRC のイベントとして解釈されるだけで、スクリプト（JavaScript）は実行されません。zip の展開では、展開先の外を指すパスを無視します。
