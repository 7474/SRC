# バージョンの管理

> 更新日: 2026-10-13

SRC# のバージョンの付け方と、リリースの手順。

## バージョン

| 系統 | 対象 | 形式 | 例 |
|---|---|---|---|
| アプリ（プレイできる SRC としての版） | SRC#Form（`SRCSharpForm`）・SRC#Avalonia・SRC#Web・データビューア（`SRCTestBlazor`） | `3.<マイナー>.<パッチ>` | 3.5.0 |
| ライブラリ・ツール | SRCCore・SRCDataLinter・SRCRender・SRCSound・SRCPlay・SRCAuthoring・SRCDebugAdapter・srcsharp（`SRCSharpTools`） | `0.<マイナー>.<パッチ>` | 0.5.0 |
| VS Code 拡張 | `SRCSharpVSCode`（Marketplace の `koudenpa.src-vscode`） | `0.<マイナー>.<パッチ>`（`package.json` の `version`。ほかとは別に上げる） | 0.6.0 |

- **メジャー**: 2 までを元の SRC（VB 版）とみなし、3 を SRC# のメジャーバージョンとする。ライブラリは API が固まるまで 0
- **マイナー**: 元の SRC にならい、奇数を開発版とする。5 が実質の初版（4 はそれを参照しているシナリオなどがあるため使わない）
- **パッチ**: 上げる時期は定めない
- **マイナーとパッチは両系統で共通**（アプリ 3.5.0 ↔ ライブラリ 0.5.0）

値は `SRC.Sharp/Directory.Build.props` の 1 か所で決める（`SrcSharpVersionMinor`・`SrcSharpVersionPatch`）。各プロジェクトは `SrcSharpAppVersion` か `SrcSharpLibraryVersion` を参照する。

### シナリオから見えるバージョン

`AppVersion`（システム変数）は SRC# のバージョンではなく、互換対象の元の SRC のバージョン（Ver.2.2.33 → 20233）を返す（`SRC.CompatibleVersion`）。シナリオは本体が十分新しいかをこれで確かめるため。

### 表示

元の SRC と同じく、起動時のタイトル（frmTitle）にだけ表示する。

| アプリ | 表示する場所 |
|---|---|
| SRC#Form | タイトル画面（`Forms/Title.cs`） |
| SRC#Avalonia・SRC#Web | 起動時のタイトル（`ScreenGUI.OpenTitleForm`。配置は SRC#Form と同じ） |

## リリース

リリースは、タグを明示的に打った場合のみ行う。

1. `SRC.Sharp/Directory.Build.props` のマイナー・パッチを決めて master に入れる
2. アプリのバージョンでタグを打つ（`v3.<マイナー>.<パッチ>`）
3. `.github/workflows/Publish-Package.yml` が、タグとバージョンが一致することを確かめてから公開する

| 公開先 | 内容 | バージョン |
|---|---|---|
| NuGet | SRCCore・SRCDataLinter・SRCSharpTools（.NET tool の `srcsharp`） | ライブラリ（0.x） |
| GitHub Releases | SRC#Form（Windows 向け `SRCSharpForm.zip`）・SRC#Web（外部のサイトに置くための `SRCSharpWeb.zip`、[SRC#Web を外部のサイトに置く](./gui/web-hosting.md)） | アプリ（3.x） |
| Docker Hub | `koudenpa/srcdatalinter`（SRCDataLinter と srcsharp） | イメージのタグはリリースのタグ名（`v3.x.x`）と `latest` |

ジョブは `check-version` の後に、`push-nuget-package`（NuGet）・`release-github-artifacts`（SRC#Form）・`release-web`（SRC#Web）・`push-to-dockerhub`（Docker Hub）が並んで動く。

GitHub Pages（データビューア・SRC#Web）は master に入るたびに更新する（開発版）。

SRCSharpTools と SRC#Web のパッケージは `v0.5.0` の後に加えたため、次のリリースから公開される。それまでは手元で作る（[作成支援ツール](./tools/authoring.md#cli)・[SRC#Web を外部のサイトに置く](./gui/web-hosting.md)）。

### VS Code 拡張

VS Code 拡張（`SRCSharpVSCode`）は、Marketplace の `koudenpa.src-vscode` の更新として 0.6.0 から出す。旧版（0.0.5）は 7474/SRC-VSCode から出していた（7474/SRC#1175）。

`.github/workflows/Publish-VSCode.yml` が次のことをする。

- タグ（`v*`）の push で動く。
  - .vsix を作り、GitHub Release に添付する。
  - [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode) と Open VSX に出す。
  - 拡張の版（`package.json` の `version`）が公開済みなら、出さずに飛ばす（`--skip-duplicate`）。拡張を出すときは、`version` と `CHANGELOG.md` を上げてからタグを打つ。
- 手動実行もできる。
  - `publish` は、タグを打たずに出す。`pre_release` を付けるとプレリリースになる。プレリリースはマイナーを奇数にする（[Pre-release extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension#prerelease-extensions)）。
  - `profile-id` は、Marketplace の publisher に加える ID を出す。初回に 1 度だけ使う。
- Marketplace への認証には、Azure のユーザー割り当てマネージド ID（`mi-src-vscode-publish`）を使う。GitHub Actions の OIDC でログインし、`vsce publish --azure-credential` で出す。
  - フェデレーション資格情報は、environment `vscode-marketplace` のジョブだけを信頼する。
  - PAT は使わない（Azure DevOps のグローバル PAT は 2026-12-01 に廃止）。
- Open VSX には、Secrets に `OVSX_PAT` があるときだけ出す。

`.vsix` の README の相対リンクは、`npm run package` の `--baseContentUrl` で `SRC.Sharp/SRCSharpVSCode` 基準の GitHub の URL に書き換わる。vsce は `repository.directory` を見ないため。

`v0.5.0` まではライブラリのバージョンでタグを打っていた。以降はアプリのバージョン（`v3.x.x`）で打つ。
