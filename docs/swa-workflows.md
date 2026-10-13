# Static Web Apps のワークフロー

Azure Static Web Apps（SWA）へのデプロイと、その後始末に使うワークフローです。

| ワークフロー | 役割 |
| --- | --- |
| `.github/workflows/CD-swa-dataviewer.yml` | SRC#DataViewer（`SRC.Sharp/SRCTestBlazor`）を SWA にデプロイする。PR ではプレビュー環境を作り、E2E を流す。PR が閉じたらプレビュー環境を閉じる |
| `.github/workflows/CD-swa-help.yml` | SRC#Help（`SRC.Sharp.Help`）を SWA にデプロイする。PR のプレビュー環境の扱いは上と同じ |
| `.github/workflows/CD-swa-cleanup-previews.yml` | PR がクローズ済みなのに残っているプレビュー環境を毎日閉じる |

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
- Docker を使うため、ジョブは `ubuntu-slim`（非特権のコンテナで、Docker を使えない）ではなく `ubuntu-latest` で動かします（[GitHub-hosted runners](https://docs.github.com/en/actions/reference/runners/github-hosted-runners)）。
- 既に無い環境を閉じようとした場合は警告を出して続けます。
