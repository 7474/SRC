# Static Web Apps のワークフロー

Azure Static Web Apps（SWA）へのデプロイと、その後始末に使うワークフローです。実行の重なりとトークンの無い PR の扱いを決めた経緯は [判断の記録](./decisions/2026-10-20-ci-gate-and-auto-merge.md) にあります。

| ワークフロー | 役割 |
| --- | --- |
| `.github/workflows/CD-swa-dataviewer.yml` | SRC#DataViewer（`SRC.Sharp/SRCTestBlazor`）を SWA にデプロイする。PR ではプレビュー環境を作り、E2E を流す。PR が閉じたらプレビュー環境を閉じる |
| `.github/workflows/CD-swa-help.yml` | SRC#Help（`SRC.Sharp.Help`）を SWA にデプロイする。PR のプレビュー環境の扱いは上と同じ |
| `.github/workflows/CD-swa-cleanup-previews.yml` | PR がクローズ済みなのに残っているプレビュー環境を毎日閉じる |

## データビューアのパック閲覧

`SRCTestBlazor` の「パック」メニュー（`/packs`）では、format-1 カタログの `kind: asset` を選び、
ZIP 内の全ファイルを名前で検索できます。選択中の SRC データから参照されているかどうかには依存しません。
パックは重ね合わせず、`author`・`license`・`description`・`version` を個別に表示します。
これらは配信元の記載であり、ビューアが権利や利用可否を判定するものではありません。

- 既定のカタログは**同一オリジンの** `/api/packs.json`。取得ボタンを押すまで通信しません。API を持たない静的配信では、URL を変更するかローカル ZIP を開きます。ローカル ZIP はアップロードしません。
- ディープリンクは `packs?catalog=<URLエンコードしたカタログURL>&pack=<URLエンコードしたid>`。クエリがある場合は自動で読み込みます。SRC-Site の `/viewer/` 配置では `/viewer/packs?pack=<id>` で同一オリジンの API を参照できます。配置先の `<base href>` と SPA のフォールバック設定は配信側で合わせます。
- カタログは `{ "format": 1, "name": "...", "entries": [...] }`。各エントリの `id`・`kind`・`title`・`archive`・`version` と、任意の `root`・`author`・`license`・`description` を使います。`archive` はカタログ URL からの相対 URL または HTTP(S) 絶対 URL。同一オリジン以外は配信元の CORS 許可が必要です。取得失敗は画面に表示します。
- SRC-Site は `/files/packs/{id}/{version}.zip` を配信します。ZIP は全体をブラウザーで取得し、`root` があればそのフォルダー以下だけを列挙します（Range を使った部分取得・永続キャッシュは未対応）。
- PNG・JPEG・GIF・BMP・WebP の画像と、WAV・MP3・Ogg・M4A・FLAC の音声をブラウザーの標準機能で表示・再生します。実際のコーデック対応はブラウザーに依存します。MIDI 等の未対応形式やデータファイルは個別ダウンロードが可能です。パックのデータを既存の Titles 表示に取り込む機能はありません。
- ZIP は 64 MiB・10,000 エントリまで、個別の展開・プレビュー・ダウンロードは 16 MiB まで。UTF-8 のフラグがない ZIP のファイル名は Shift_JIS として読みます。選択変更時・画面を離れた際に ZIP と Blob URL を解放します。

回帰テストは `SRC.Sharp/SRCTestBlazor/E2E/cypress/e2e/packs.cy.js`。外部データに依存せず、
合成した画像・音声の ZIP とカタログで root・メタデータ・プレビュー・検索・ローカル ZIP・失敗表示を確認します。

## 実行の重なりとトークンの無い PR

- `concurrency` で、本番（master への push）への配信は 1 つずつ動かし（待っているものは最新だけが残る）、PR では同じ PR の古い実行を取り消す。本番への配信が重なると、後から始めたものが「No matching Static Web App environment was found」などで失敗するため（[Control the concurrency of workflows and jobs](https://docs.github.com/en/actions/how-tos/write-workflows/choose-when-workflows-run/control-workflow-concurrency)）
- Dependabot やフォークからの PR では Actions のシークレットが渡されない（[Troubleshooting Dependabot on GitHub Actions](https://docs.github.com/en/code-security/dependabot/troubleshooting-dependabot/troubleshooting-dependabot-on-github-actions)）。デプロイ用のトークンが空のときは、プレビュー環境への配信と Cypress を省く。SRC#DataViewer は配信の前のビルドまでは行い、SRC#Help のビルドは CI の `build-help` で確かめる（[テストと CI の設計](./testing.md)）

## プレビュー環境の定期削除

SWA のプレビュー環境の数には上限があります（[Preview environments](https://learn.microsoft.com/azure/static-web-apps/preview-environments)）。PR を閉じたときの削除が失敗したり、ワークフローが動かなかったりして環境が残ると、新しい PR のデプロイが次のエラーで失敗します。

```
This Static Web App already has the maximum number of staging environments
```

`CD-swa-cleanup-previews.yml` は毎日、次の処理を行います。手動でも実行でき、閉じる PR の番号を指定することもできます。

1. 直近（既定は 30 日）にクローズ・マージされた PR を一覧する。
2. それぞれの PR について、2 つの Static Web App のプレビュー環境を閉じる。

閉じ方は、PR を閉じたときの Close Pull Request Job と同じです。デプロイ用のトークン（`AZURE_STATIC_WEB_APPS_API_TOKEN_…`）で StaticSitesClient の `close` を実行するため、Azure への追加の認証情報は要りません。

- `Azure/static-web-apps-deploy` は、実行中のイベント（PR）のプレビュー環境を閉じる作りです。そのため、同じイメージ（`mcr.microsoft.com/appsvc/staticappsclient:stable`）を `docker run` で直接動かし、閉じる PR のイベントのペイロードを渡しています。
- Docker を使うため、ジョブは `ubuntu-slim`（非特権のコンテナで、Docker を使えない）ではなく `ubuntu-24.04` で動かします（[GitHub-hosted runners](https://docs.github.com/en/actions/reference/runners/github-hosted-runners)）。
- 既に無い環境を閉じようとした場合は警告を出して続けます。
