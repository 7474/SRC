# CI の必須チェックを ci-passed にまとめ、Dependabot の更新を自動でマージする

- 日付: 2026-10-20
- 状態: 採用

## 背景

`CI.yml` と `CD-swa-dataviewer.yml` の直近それぞれ 300 回の実行（2026-10-13〜10-20）を調べた。失敗したジョブとテストは次のとおり（実行数は省いたジョブを除く）。

| ジョブ | 失敗 / 実行 | 主な原因 |
|---|---|---|
| `build-and-test-srcs`（Windows） | 21 / 299 | シナリオ E2E のタイムアウト（`E2ECorpusTests.Play`・`RightButtonStopsAutoPlay`・`Dap_HotReloadKeepsStateAndReloadsEventsAndData`）、Windows でだけ落ちるテスト（`PathResolverTests`）、NU1605 |
| `test-web`（Playwright） | 14 / 162 | 読み込みの待ちのタイムアウト（`シナリオを読み込んでいる間はほかの操作を受け付けない`・`アーカイブを追加し、再読み込み後もプレイできる` など） |
| `test-srccore-linux` | 6 / 299 | 多くは SRC#Web の E2E がこのジョブにあった頃のもの |
| `CD-swa-dataviewer.yml` の配信 | 45 / 217 | Dependabot の PR でトークンが無い（25）、プレビュー環境の数の上限（5）、本番への配信の失敗（15。多くは数秒おきに続けてマージして配信が重なったとき） |

そのほかに次のことが分かった。

- 必須チェックは Windows の `build-and-test-srcs` だけで、Linux のテスト（シナリオ E2E・VRT・SRC#Web）が失敗していてもマージできた
- Windows のシナリオ E2E のタイムアウトは、同じテストが Linux では通っていた。タイムアウトのほかに Windows でだけ落ちたのは `PathResolverTests`（大文字・小文字を区別しないファイルシステムでの期待値）だけだった。VRT は Windows では Inconclusive になる
- `SRCSharpFormTestWinAppDriver` は、ソリューションの Any CPU の構成でビルドの対象外で、CI で一度も動いていなかった。WinAppDriver は保守されていない。同じファイル（`.csproj`・`app.config`・`packages.config`）を書き換える Dependabot の PR が並び、1 本マージするたびに残りが競合していた
- Mackerel に送っていたカバレッジは、Windows で集めた `coverage.cobertura.xml` のうち最後に書かれた 1 つ（`SRCCore` か `SRCRender` のどちらか）の値だった
- Dependabot の PR では Actions のシークレットが渡されず、Static Web Apps への配信が必ず失敗していた。SRC#Help（bundler）の更新は、配信のワークフローが止まるため何にもビルドされていなかった
- Jekyll のビルドを CI で確かめたところ、`SRC.Sharp.Help/Gemfile.lock` には Ruby 3.1 では入れられない版（`public_suffix` 7.0.5・`google-protobuf` 4.36 など、Ruby 3.2 以上が要るもの）が入っていた。Static Web Apps（Oryx）の `bundle install` は黙って入れられる版に解決し直してビルドしていたため、配信は成功していた

## 判断

- すべてのジョブの結果をまとめる `ci-passed` を作り、ブランチ保護の必須チェックはこれだけにする
- 文書だけの PR では、ジョブの中で判定してビルドとテストを省く（ワークフローの `paths` では絞らない）
- シナリオ E2E・VRT は Linux を正とし、Windows のジョブは Windows でしか確かめられないもの（ソリューション全体のビルドと SRC#Form の発行、`SRCCoreTests`、Windows のフォントと SkiaSharp のテスト）に絞る
- カバレッジは master への push のときだけ動く `coverage` ジョブで、`SRCCore` と `SRCRender` を合わせたものを送る。マージの可否には使わない
- SRC#Help の Jekyll のビルドを CI（`build-help`）で確かめる。Oryx と同じく `Gemfile.lock` を固定せずに入れる
- Playwright は CI で 1 度だけやり直し、通ったものは flaky として注釈に残す。MSTest はやり直さない
- Static Web Apps の配信は `concurrency` で本番を 1 つずつにし、デプロイ用のトークンが無い PR では配信を省く
- Dependabot のマイナー・パッチの更新はエコシステムごとにまとめ、CI でビルドしているものは自動でマージする
- `SRCSharpFormTestWinAppDriver` を消す。SRC#Form の UI テストが要る場合は FlaUI で作り直す

## 理由

- 必須チェックを 1 つにすると、ジョブを足しても減らしてもブランチ保護の設定を変えなくてよい。`paths` で絞ると、必須チェックが報告されずにマージできなくなる
- マージを止めるテストが時々落ちると、自動マージは止まり、人は失敗を読まなくなる。待ちの長いテストを遅いランナーで重ねて動かしても、見つかるものは無かった
- CI で動いていないテストは品質を保証せず、依存の更新の手間だけがかかる
- 同じファイルを書き換える更新を 1 つにまとめると、競合と作り直しが無くなる

## 影響

- 管理者が master のブランチ保護の必須チェックを `build-and-test-srcs` から `ci-passed` に変える必要がある（[Managing a branch protection rule](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/managing-a-branch-protection-rule)）
- Mackerel のカバレッジの値の意味が変わる（2 つのプロジェクトを合わせた値になる）
- `GITHUB_TOKEN` で設定した自動マージによる push では、push で動くワークフローが動かない（[GITHUB_TOKEN](https://docs.github.com/en/actions/concepts/security/github_token)）。Dependabot のシークレットに `AUTOMERGE_TOKEN` を登録すればそれを使う
- `SRC.Sharp.Help/Gemfile.lock` と Ruby の版の食い違いは残っている

## 参照

- [テストと CI の設計](../testing.md)
- [Static Web Apps のワークフロー](../swa-workflows.md)
- 7474/SRC#1299・7474/SRC#1301
- [Automating Dependabot with GitHub Actions](https://docs.github.com/en/code-security/dependabot/working-with-dependabot/automating-dependabot-with-github-actions)
- [Troubleshooting Dependabot on GitHub Actions](https://docs.github.com/en/code-security/dependabot/troubleshooting-dependabot/troubleshooting-dependabot-on-github-actions)
- [Playwright: Retries](https://playwright.dev/docs/test-retries)
- [Control the concurrency of workflows and jobs](https://docs.github.com/en/actions/how-tos/write-workflows/choose-when-workflows-run/control-workflow-concurrency)
