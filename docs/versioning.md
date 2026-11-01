# バージョンの管理

SRC# のバージョンの付け方と、リリースの手順。リリースのタグと VS Code 拡張の版の決め方の経緯は [判断の記録](./decisions/2026-10-16-release-tags-and-vscode-version.md) にある。

## バージョン

| 系統 | 対象 | 形式 | 例 |
|---|---|---|---|
| アプリ（プレイできる SRC としての版） | SRC#Form（`SRCSharpForm`）・SRC#Avalonia・SRC#Web・データビューア（`SRCDataViewer`） | `3.<マイナー>.<パッチ>` | 3.5.0 |
| ライブラリ・ツール | SRCCore・SRCDataLinter・SRCRender・SRCSound・SRCPlay・SRCAuthoring・SRCDebugAdapter・srcsharp（`SRCSharpTools`） | `0.<マイナー>.<パッチ>` | 0.5.0 |
| VS Code 拡張 | `SRCSharpVSCode`（Marketplace の `koudenpa.src-vscode`） | `0.<マイナー>.<パッチ>`（`package.json` の `version`。ライブラリと同じ版にする） | 0.5.2 |

- **メジャー**: 2 までを元の SRC（VB 版）とみなし、3 を SRC# のメジャーバージョンとする。ライブラリは API が固まるまで 0
- **マイナー**: 元の SRC にならい、奇数を開発版とする。5 が実質の初版（4 はそれを参照しているシナリオなどがあるため使わない）
- **パッチ**: 上げる時期は定めない
- **マイナーとパッチは両系統で共通**（アプリ 3.5.0 ↔ ライブラリ 0.5.0）

値は `SRC.Sharp/Directory.Build.props` の 1 か所で決める（`SrcSharpVersionMinor`・`SrcSharpVersionPatch`）。手で書き換えず、`tools/release.py bump` で上げる。各プロジェクトは `SrcSharpAppVersion` か `SrcSharpLibraryVersion` を参照する。

### シナリオから見えるバージョン

`AppVersion`（システム変数）は SRC# のバージョンではなく、互換対象の元の SRC のバージョン（Ver.2.2.33 → 20233）を返す（`SRC.CompatibleVersion`）。シナリオは本体が十分新しいかをこれで確かめるため。

### 表示

元の SRC と同じく、起動時のタイトル（frmTitle）にだけ表示する。

| アプリ | 表示する場所 |
|---|---|
| SRC#Form | タイトル画面（`Forms/Title.cs`） |
| SRC#Avalonia・SRC#Web | 起動時のタイトル（`ScreenGUI.OpenTitleForm`。配置は SRC#Form と同じ） |

## リリース

リリースは、タグを明示的に打った場合のみ行う。作業の詳しい順は `.claude/skills/release/SKILL.md`（スキル `release`）にある。

1. `python3 tools/release.py bump` で版を上げる。次のものが変わる。
   - `SRC.Sharp/Directory.Build.props` のマイナー・パッチ
   - VS Code 拡張の `package.json`・`package-lock.json` の版
   - VS Code 拡張の `CHANGELOG.md` に節を足す
   - リリースの案内 `docs/releases/v<版>.md` をひな形から作る
2. 案内と CHANGELOG を書き（[リリースの案内](./releases/README.md)）、`python3 tools/release.py saves` でその版のセーブを作って（[テストと CI の設計](./testing.md)の「セーブデータの版をまたぐ互換」）、PR にして master に入れる。CI の `release-check`（`release.py check`）が、版・CHANGELOG・案内・セーブが揃っているかを確かめる
3. アプリのバージョンでタグを打つ（`v3.<マイナー>.<パッチ>`）
4. 公開ワークフローが、タグと版・CHANGELOG・案内が揃っていることを確かめてから公開する（`release.py check --tag`）。リリースの本文は `docs/releases/v<版>.md` になる

| 公開先 | 内容 | バージョン |
|---|---|---|
| NuGet | SRCCore・SRCDataLinter・SRCSharpTools（.NET tool の `srcsharp`） | ライブラリ（0.x） |
| GitHub Releases | SRC#Form（Windows 向け `SRCSharpForm.zip`）・SRC#Avalonia（OS ごとの `SRCSharpAvalonia-<RID>.zip`、`Publish-Avalonia.yml`）・SRC#Web（外部のサイトに置くための `SRCSharpWeb.zip`、[SRC#Web を外部のサイトに置く](./gui/web-hosting.md)） | アプリ（3.x） |
| Docker Hub | `koudenpa/srcdatalinter`（SRCDataLinter と srcsharp） | イメージのタグはリリースのタグ名（`v3.x.x`）と `latest` |

ジョブは `check-version` の後に、`push-nuget-package`（NuGet）・`release-github-artifacts`（SRC#Form）・`release-web`（SRC#Web）・`push-to-dockerhub`（Docker Hub）が並んで動く。

GitHub Pages（データビューア・SRC#Web）は master に入るたびに更新する（開発版）。

## 開発版

master にマージされるたびに、配る物の開発版を出す（#1418、`.github/workflows/Packages.yml`）。

| 出す先 | 中身 |
|---|---|
| GitHub Releases の [`dev`](https://github.com/7474/SRC/releases/tag/dev)（プレリリース） | SRC#Avalonia（4 RID）・srcsharp の nupkg・VS Code 拡張の .vsix・SRCSharpWeb.zip（AOT）・SRCLintWeb.zip・SRCDataViewer.zip・`manifest.json` |
| Docker Hub | `koudenpa/srcdatalinter:dev` |

- **出すとき**：`SRC.Sharp/` を変えた master への push で、配る物がすべて作れ、起動の確かめ（SRC#Avalonia の `--smoke`、srcsharp と Docker イメージの `--version`・lint）が通ったときだけ出す。続けてマージしたときは最後のものだけを出す
- **リリースは 1 つ**：タグ `dev` のリリースを使い回し、タグをそのコミットに付け直して、添付と本文を入れ替える。前の添付は消す。作り直さないので Watch に通知は飛ばない。Latest にはしない（正式版の Latest はそのまま）
- **版**：次のパッチのプレリリースにする（3.5.5 の後なら `3.5.6-dev.<実行番号>`・`0.5.6-dev.<実行番号>`）。`dotnet build` に `-p:SrcSharpDevBuild=<番号>` を渡すと、`Directory.Build.props` がこの版にする
  - 正式版と同じ版にすると、NuGet のキャッシュ（パッケージの ID と版で覚える）で中身の違う開発版と正式版が混ざる。プレリリースは正式版より小さいので、次の正式版が出れば置き換わる
  - タイトル（`3.5.6-dev.123`）と `srcsharp --version` に出る。アセンブリの版（`3.5.6.0`）とセーブデータの版にはプレリリースの印は入らない
- **SRC#Form は出さない**（維持の階層）。Marketplace・Open VSX・NuGet にも出さない（正式版だけ）

### 他のリポジトリから使う

| 使うもの | 書き方 |
|---|---|
| ファイル | `https://github.com/7474/SRC/releases/download/dev/<ファイル名>`、または `gh release download dev -R 7474/SRC -p '<パターン>'`（公開リポジトリなので、トークンは要らない。Actions の中では `GITHUB_TOKEN` で足りる） |
| srcsharp（.NET tool） | nupkg を取って `dotnet tool install SRCSharpTools --add-source <フォルダ> --prerelease` |
| srcsharp（Docker） | `docker run --rm -v "$PWD:/s" koudenpa/srcdatalinter:dev lint /s` |

`dev` はマージのたびに中身が変わる。同じものを使い続けたいとき・何を使ったかを残したいときは、`manifest.json`（元のコミット・版・各ファイルの SHA-256・Docker イメージのダイジェスト）を記録し、ファイルは SHA-256 で確かめ、Docker はダイジェスト（`koudenpa/srcdatalinter@sha256:…`）で指す。

Docker の `dev` のタグを付け替えると、前のイメージはタグの無いイメージとして Docker Hub に残る。溜まったら Docker Hub で消す。

### VS Code 拡張

VS Code 拡張（`SRCSharpVSCode`）は、Marketplace の `koudenpa.src-vscode` として出す。版はライブラリ（srcsharp）と同じにする。`package.json` の `version` は `release.py bump` が上げ、CI（`CI.yml` の `release-check`）が `Directory.Build.props` のライブラリの版と同じかを確かめる。

`.github/workflows/Publish-VSCode.yml` が次のことをする。

- タグ（`v*`）の push で動く。
  - .vsix を作り、GitHub Release に添付する。
  - [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode) と Open VSX に出す。
  - 拡張の版（`package.json` の `version`）が公開済みなら、出さずに飛ばす（`--skip-duplicate`）。`CHANGELOG.md` に版の節が無いと、タグの公開は止まる（`release.py check --tag`）。
- 手動実行もできる。
  - `publish` は、タグを打たずに出す。
- 版のマイナーが 7 以上の奇数ならプレリリース、それ以外（0.5.x と偶数）は正式版として出す（[Pre-release extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension#prerelease-extensions)）。タグの push でも手動実行でも同じ。
  - `profile-id` は、Marketplace の publisher に加える ID を出す。初回に 1 度だけ使う。
- Marketplace への認証には、Azure のユーザー割り当てマネージド ID（`mi-src-vscode-publish`）を使う。GitHub Actions の OIDC でログインし、`vsce publish --azure-credential` で出す。
  - フェデレーション資格情報は、environment `vscode-marketplace` のジョブだけを信頼する。
  - PAT は使わない（Azure DevOps のグローバル PAT は 2026-12-01 に廃止）。
- Open VSX には、Secrets に `OVSX_PAT` があるときだけ出す。

`.vsix` の README の相対リンクは、`npm run package` の `--baseContentUrl` で `SRC.Sharp/SRCSharpVSCode` 基準の GitHub の URL に書き換わる。vsce は `repository.directory` を見ないため。
