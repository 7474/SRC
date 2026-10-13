# Static Web Apps のワークフロー

Azure Static Web Apps（SWA）へのデプロイと、その後始末に使うワークフローです。

| ワークフロー | 役割 |
| --- | --- |
| `.github/workflows/CD-swa-dataviewer.yml` | SRC#DataViewer（`SRC.Sharp/SRCTestBlazor`）を SWA にデプロイする。PR ではプレビュー環境を作り、E2E を流す。PR が閉じたらプレビュー環境を閉じる |
| `.github/workflows/CD-swa-help.yml` | SRC#Help（`SRC.Sharp.Help`）を SWA にデプロイする。PR のプレビュー環境の扱いは上と同じ |
| `.github/workflows/CD-swa-cleanup-previews.yml` | PR がクローズ済みなのに残っているプレビュー環境を毎日削除する |

## プレビュー環境の定期削除

SWA のプレビュー環境の数には上限があります（[Preview environments](https://learn.microsoft.com/azure/static-web-apps/preview-environments)）。PR を閉じたときの削除が失敗したり、ワークフローが動かなかったりして環境が残ると、新しい PR のデプロイが次のエラーで失敗します。

```
This Static Web App already has the maximum number of staging environments
```

`CD-swa-cleanup-previews.yml` は毎日、次の処理を行います。手動でも実行できます（`dry_run` で対象を確認するだけにもできます）。

1. 既定のホスト名から Static Web App を探す。
2. プレビュー環境を一覧する（[`az staticwebapp environment list`](https://learn.microsoft.com/cli/azure/staticwebapp/environment)）。
3. 環境名（PR 番号）の PR がクローズ済み・マージ済みなら削除する（`az staticwebapp environment delete`）。

デプロイ用のトークンでは環境の一覧と削除ができないため、Azure へは OpenID Connect でサインインします。

### 設定

1. Microsoft Entra ID にアプリ登録（またはユーザー割り当てマネージド ID）を作り、このリポジトリのフェデレーション資格情報を追加する。
   - エンティティの種類は「ブランチ」、ブランチは `master`（スケジュール実行と手動実行は既定のブランチで動くため）
   - 手順: [Use the Azure Login action with OpenID Connect](https://learn.microsoft.com/azure/developer/github/connect-from-azure-openid-connect)
2. 2 つの Static Web App があるリソースグループ（またはそれぞれ）に、そのアプリへ「Website Contributor」などの環境を削除できるロールを割り当てる。
3. リポジトリの Secrets に次を登録する。

| Secret | 値 |
| --- | --- |
| `AZURE_CLIENT_ID` | アプリ（クライアント）ID |
| `AZURE_TENANT_ID` | テナント ID |
| `AZURE_SUBSCRIPTION_ID` | サブスクリプション ID |

未設定の間は、ワークフローは通知を出して何もせずに終わります。
