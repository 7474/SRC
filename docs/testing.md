# テストと CI の設計 / Testing and CI

「CI が通れば人が見なくてもマージしてよい」と言える状態を保つための、テストの責務の分け方と CI の構成。シナリオの E2E・VRT の詳細は [E2E テスト方針](./gui/e2e-testing.md) にある。経緯は [判断の記録](./decisions/2026-10-20-ci-gate-and-auto-merge.md) にある。

## 方針

- **1 つのことは 1 つの層で確かめる**。シナリオの挙動と見た目は `SRCRender` のヘッドレスな E2E・VRT で確かめ、フロントエンドのテストは「画面が出て、入力が `ScreenGUI` に届くか」とそのフロントエンドにしか無い機能に絞る
- **正とする環境は Linux**。VRT の基準画像は Ubuntu 24.04 + `fonts-noto-cjk` で作っている。Windows では Windows でしか確かめられないものだけを動かす
- **マージを止めるのは、決定的に結果が出るテストだけ**。計測（カバレッジなど）と、配信した後の確認はマージを止めない
- **必須チェックは `ci-passed` の 1 つだけ**。ジョブを足しても減らしても、ブランチ保護の設定を変えなくてよい

## テストの層と責務

| 層 | 保証すること | 保証しないこと | プロジェクト | CI のジョブ |
|---|---|---|---|---|
| ユニット | 関数・式・イベントコマンド・データ読み込みの仕様。速く、待ちや時刻に依存しない | 画面・入力・スレッドの待ち合わせ | `SRCCoreTests`・`SRCAuthoringTests`・`SRCSoundTests` | `test-srccore-linux`・`test-tools-linux`（`SRCCoreTests` は `build-and-test-srcs` でも） |
| シナリオ E2E | コーパス（`SRC.Sharp/E2E/`）を入力スクリプトでプレイしたときの挙動。イベントコマンドの網羅 | フロントエンドごとの見た目・入力の仕組み | `SRCRenderTests`（`E2ECorpusTests`・画面のシナリオのテスト） | `test-srccore-linux` |
| VRT | `SRCRender` が描いた画面が基準画像と一致すること | フロントエンドの部品（ボタン・メニュー）の見た目 | `SRCRenderTests`（`Baselines/`） | `test-srccore-linux` |
| フロントエンド E2E | 起動・描画・入力の経路と、そのフロントエンドにしか無い機能 | シナリオの挙動（シナリオ E2E が担う） | `SRCSharpAvaloniaTests`（Avalonia.Headless）・`SRCSharpWeb.E2E`（Playwright。読み込み・ブラウザ内保存・URL・タッチ・カタログ） | `test-tools-linux`・`test-web` |
| 配布物 | 配る形でビルド・発行でき、最低限動くこと | 機能の正しさ | Docker イメージ（`srcsharp`）・VS Code 拡張・SRC#Form の発行・SRC#Help（Jekyll） | `docker-srcdatalinter`・`test-tools-linux`・`build-and-test-srcs`・`build-help` |
| 版の揃い | アプリ・ライブラリ・VS Code 拡張の版、拡張の CHANGELOG、リリースの案内（`docs/releases`）が揃っていること | 公開できること（タグの push のときに公開ワークフローが確かめ直す） | `tools/release.py check` | `release-check`（文書だけの PR でも動く） |
| Windows 固有 | WinForms の SRC#Form を含むソリューション全体のビルド、大文字・小文字を区別しないファイルシステムでの `SRCCore`、Windows の実フォントの寸法、SkiaSharp のネイティブライブラリ | シナリオ E2E・VRT（Linux を正とする） | `SRCCoreTests`・`SRCRenderTests` の `WindowsFontMetricsTests`・`SkiaFontsTests` | `build-and-test-srcs` |
| 配信後の確認 | 配信したデータビューアが開き、データを読めること | マージの可否（マージの後に動く） | `SRCTestBlazor/E2E`（Cypress） | `CD-swa-dataviewer.yml` の `e2e` |
| 計測 | カバレッジ（`SRCCore`・`SRCRender` を合わせたもの）、シナリオで実行したコマンドの網羅率 | マージの可否 | — | `CI.yml` の `coverage`（master への push のみ） |

テストを足すときは、上の表で「保証すること」に当たる層に足す。シナリオの挙動を Playwright や Avalonia のテストで確かめたくなったら、まずコーパスにケースを足せないかを考える。

## CI の構成

```mermaid
flowchart LR
  PR[PR / master への push] --> changes
  changes -->|コードの変更あり| core[test-srccore-linux<br/>ユニット・シナリオ E2E・VRT]
  changes -->|コードの変更あり| tools[test-tools-linux<br/>Avalonia・作成支援ツール・音声・VS Code 拡張]
  changes -->|コードの変更あり| web[test-web<br/>SRC#Web の Playwright]
  changes -->|コードの変更あり| docker[docker-srcdatalinter]
  changes -->|コードの変更あり| win[build-and-test-srcs<br/>Windows 固有]
  changes -->|ヘルプの変更あり| help[build-help]
  PR -->|常に| release[release-check<br/>版・CHANGELOG・リリースの案内]
  core & tools & web & docker & win & help & release --> gate{{ci-passed<br/>必須チェック}}
  PR -->|push のみ| coverage[coverage<br/>計測・Mackerel]
```

- `changes` は PR の変更されたファイルを見て、文書だけ（`docs/`・ルートの `*.md`・Issue のテンプレート）の PR ではビルドとテストを省く。ワークフローの `paths` で絞ると必須チェックが報告されずにマージできなくなるため、ジョブの中で判定している。`if` で省いたジョブは成功として扱われる（[Using conditions to control job execution](https://docs.github.com/en/actions/how-tos/write-workflows/choose-when-workflows-run/control-jobs-with-conditions)）
- `ci-passed` は、すべてのジョブが成功か省略なら成功する。失敗・取り消しが 1 つでもあれば失敗する
- PR と master への push で動かすテストは同じ。違うのは SRC#Web の発行（PR はインタープリター、push は AOT）と、push のときだけの `coverage`

### ブランチ保護

master のブランチ保護の必須チェック（Require status checks to pass）は `ci-passed` だけにする。ほかのジョブを必須チェックにすると、そのジョブを省いたり名前を変えたりしたときにマージできなくなる。

設定は Settings → Branches → master のルール → Require status checks to pass before merging で行う（[Managing a branch protection rule](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/managing-a-branch-protection-rule)）。

## 自動マージ

Dependabot のマイナー・パッチの更新は、ディレクトリ（エコシステム）ごとに 1 つの PR にまとめ（`.github/dependabot.yml` の `groups`）、`.github/workflows/dependabot-auto-merge.yml` が自動マージを設定する。マージされるのは `ci-passed` が通ったときだけ。

| 更新 | 扱い |
|---|---|
| `SRC.Sharp/` の NuGet（`nuget-minor-patch`。版は `SRC.Sharp/Directory.Packages.props` の 1 か所。ソリューションに無い `SRCSharpWeb`・`SRCLintWeb` もディレクトリを並べて対象にする） | 自動マージ |
| npm（`npm-minor-patch`。VS Code 拡張・SRC#Web の E2E・データビューアの E2E） | 自動マージ |
| `SRC.Sharp.Help/` の bundler（`bundler-minor-patch`） | 自動マージ（`build-help` で Jekyll のビルドを確かめる） |
| GitHub Actions（`actions-minor-patch`） | 自動マージ |
| SkiaSharp のメジャーの更新（`skiasharp-major`） | `SkiaSharp*` をまとめて 1 つの PR にする（管理側とネイティブの版が揃わないと動かないため）。人が確かめてマージする |
| そのほかのメジャーの更新・セキュリティの更新 | 個別の PR。人が確かめてマージする |

- まとめるのは、同じファイル（`.csproj`・`packages.config` など）を書き換える PR が何本も並び、1 本マージするたびに残りが競合して作り直しになるのを避けるため
- `GITHUB_TOKEN` で設定した自動マージによる push では、push で動くワークフロー（GitHub Pages・Static Web Apps への配信、`coverage`）が動かない（[GITHUB_TOKEN](https://docs.github.com/en/actions/concepts/security/github_token)）。次に人がマージしたときにまとめて配信される。すぐに配信したい場合は、Dependabot のシークレットに `AUTOMERGE_TOKEN`（このリポジトリの contents・pull requests に書き込める fine-grained personal access token）を登録すると、それで自動マージを設定する（[Dependabot のシークレット](https://docs.github.com/en/code-security/dependabot/troubleshooting-dependabot/troubleshooting-dependabot-on-github-actions)）
- Dependabot の PR では Actions のシークレットが渡されないため、Static Web Apps のプレビュー環境への配信と、その後の Cypress は省く（[Static Web Apps のワークフロー](./swa-workflows.md)）

## 不安定なテスト（flaky）の扱い

マージを止めるテストが時々落ちると、自動マージは止まり、人は「また flaky だろう」と失敗を読まなくなる。そのため次のようにする。

- **待ちの長いシナリオ E2E は Linux だけで動かす**。Windows のランナーでは同じテストがタイムアウトしやすい
- **Playwright は CI で 1 度だけやり直す**（`retries`）。やり直して通ったテストは結果に `flaky` と出る（[Retries](https://playwright.dev/docs/test-retries)）。`test-web` の「Summarize SRC#Web tests」が、ジョブのサマリに flaky のテストの一覧を出し、テストの行に warning の注釈を付ける（`.github/scripts/playwright-summary.mjs`）。flaky が出たら、待ちの条件か前提を直す
- **MSTest はやり直さない**。落ちたら、待ちの条件（`wait`・`auto` の秒数、`[DoNotParallelize]`）か、テストの前提（静的な状態・プロセス全体の設定）を直す
- ジョブの再実行で通すのは、ランナーの不調（取得・インストールの失敗）のときだけにする

## リグレッションが起きたら

当面は自動化を優先し、人のレビューを必須にしたゲートは増やさない。その代わり、前の版で動いていたものが壊れたら（リグレッション）、その領域の守りを厚くする（[判断の記録](./decisions/2026-10-31-regression-first-qa-plan.md)、#1350）。エラーバジェットは設けないが、考え方は同じ。

1. 不具合の報告で「前の版では動いていたか」に「はい」と答えると、`regression-label.yml` がラベル `regression` を付ける。人が後から付けてもよい
2. 直す PR で、同じ壊れ方を CI で見つけられるテストを、上の表の該当する層に足す
3. その領域でマージを止める位置（マージの前・後・リリース）が足りていたかを見直し、足りなければ #1350 の段階に足す

`regression` の Issue の数を、改善の効き目の物差しにする。

## 残っている課題

- Playwright の flaky なテスト（`シナリオを読み込んでいる間はほかの操作を受け付けない`・`アーカイブを追加し、再読み込み後もプレイできる` など、読み込みの待ちのタイムアウト）の待ちの条件を直す
- `SRCTestBlazor/E2E`（Cypress）は配信した後に動き、外部（`7474/SRC-Data`）のデータを読む。マージの可否には使わない
- SRC#Help の Ruby は Static Web Apps（Oryx）に合わせて 3.1 のままで、`Gemfile.lock` には Ruby 3.2 以上が要る版（`public_suffix` 7.0.5・`google-protobuf` 4.36 など）が入っている。Oryx と `build-help` は `Gemfile.lock` を固定せずに入れられる版に解決し直してビルドしている。Ruby を上げるか、Ruby 3.1 で `Gemfile.lock` を作り直す
- npm の依存（`SRCSharpWeb.E2E`・`SRCSharpVSCode`・`SRCTestBlazor/E2E`）は Dependabot の対象に入っていない
