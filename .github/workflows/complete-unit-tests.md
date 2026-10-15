---
description: |
  SRC# のテストカバレッジを、実測で網羅率の低いファイルから補強するワークフロー。
  SRCCoreTests と SRCRenderTests（シナリオをヘッドレスで動かすテスト）の行カバレッジを合わせて測り、
  未カバーの行が多い SRCCore のファイルを選んで、元の VB（SRC/SRC_20121125）を仕様とする挙動のテストを追加する。
  移植の不具合が見つかれば、正しい期待値のテストを [Ignore] にして残し、Issue にする。
  オープンな [unit-tests] PR があっても止まらず、その PR が触れている対象を避けて進める。

  Raises SRC# test coverage, driven by measured line coverage (SRCCoreTests + SRCRenderTests combined).
  Picks the SRCCore files with the most uncovered lines, writes behavior-level tests using the original
  VB source as the specification, and keeps porting bugs as [Ignore]d tests plus issues.
  Does not stop when an open [unit-tests] PR exists; it avoids the files that PR touches.

  NOTE: SRC.Sharp contains Windows-targeting projects (SRCSharpForm, SRCTestForm); the build uses
  -p:EnableWindowsTargeting=true so it also works on Linux runners.

on:
  schedule: daily
  workflow_dispatch:

# windowsターゲットのプロジェクトも対象とする際にはランナーをWindowsにする必要がある
# runs-on: windows-latest

permissions: read-all

timeout-minutes: 60

network:
  allowed:
    - node
    - github

steps:
  - name: Checkout repository
    uses: actions/checkout@3d3c42e5aac5ba805825da76410c181273ba90b1 # v7.0.1
    with:
      fetch-depth: 0
      persist-credentials: false

  - name: Setup .NET
    uses: actions/setup-dotnet@a98b56852c35b8e3190ac28c8c2271da59106c68 # v6.0.0
    with:
      dotnet-version: 10.0.x

  # -p:EnableWindowsTargeting=true は Linux ランナーで SRCSharpForm / SRCTestForm など
  # net10.0-windows ターゲットのプロジェクトをビルドするために必要。
  # windows-latest ランナーでは不要だが、ランナー変更時の互換性のために明示的に指定する。
  # This flag is required when building net10.0-windows targeting projects on Linux runners.
  # It is redundant on windows-latest but kept explicitly for cross-platform compatibility.
  - name: Restore and Build
    run: |
      dotnet restore SRC.Sharp/SRC.Sharp.sln -p:EnableWindowsTargeting=true
      dotnet build SRC.Sharp/SRC.Sharp.sln --no-restore -p:EnableWindowsTargeting=true

  - name: Measure coverage (baseline)
    run: |
      mkdir -p /tmp/gh-aw/coverage
      dotnet test SRC.Sharp/SRCCoreTests/SRCCoreTests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory /tmp/gh-aw/coverage/core 2>&1 | tee /tmp/gh-aw/test-baseline.txt || true
      dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory /tmp/gh-aw/coverage/render 2>&1 | tail -5 | tee -a /tmp/gh-aw/test-baseline.txt || true
      # SRCCore のファイルごとに、2 つのテストの実行で 1 回でも通った行を合わせて数える（部分クラスの重複は行番号で除く）
      python3 - <<'EOF' | tee /tmp/gh-aw/coverage-summary.txt
      import glob, re, collections, xml.etree.ElementTree as ET
      hits = collections.defaultdict(dict)
      for path in glob.glob('/tmp/gh-aw/coverage/**/coverage.cobertura.xml', recursive=True):
          root = ET.parse(path).getroot()
          source = (root.findtext('.//source') or '').rstrip('/')
          for c in root.iter('class'):
              full = (source + '/' + c.get('filename')).replace('\\', '/')
              m = re.search(r'/SRCCore/(.*)', full)
              if not m or '/TestLib/' in full:
                  continue
              for line in c.iter('line'):
                  n = int(line.get('number'))
                  hits[m.group(1)][n] = max(hits[m.group(1)].get(n, 0), int(line.get('hits')))
      rows = [(f, sum(1 for v in ls.values() if v > 0), len(ls)) for f, ls in hits.items()]
      total = sum(r[2] for r in rows); covered = sum(r[1] for r in rows)
      print(f'SRCCore line coverage (SRCCoreTests + SRCRenderTests): {covered}/{total} ({covered / max(total, 1) * 100:.1f}%)')
      print('Files by uncovered lines (path, covered/total, uncovered):')
      for f, h, n in sorted(rows, key=lambda r: r[1] - r[2])[:60]:
          print(f'{f}\t{h}/{n}\t{n - h}')
      EOF

tools:
  github:
    toolsets: [default]
  bash:
    - "*"
  edit:

safe-outputs:
  create-pull-request:
    title-prefix: "[unit-tests] "
    labels: [automation, testing]
    draft: false
    if-no-changes: warn
    github-token-for-extra-empty-commit: ${{ secrets.CI_GITHUB_TOKEN }}
  create-issue:
    max: 3
    labels: [automation, testing, bug]
  noop:

engine: copilot
---

# カバレッジ補強エージェント / Coverage Reinforcement Agent

あなたは SRC# 移植プロジェクト (`${{ github.repository }}`) の自動テストエンジニアです。
実測したカバレッジをもとに、テストの足りないファイルへ、挙動を確かめるテストを追加してください。
1 回の実行で、できるだけ多く・意味のあるテストを追加してください。

You are an automated test engineer for `${{ github.repository }}`. Use the measured coverage to pick
under-tested files and add meaningful, behavior-level tests. Add as much as you can per run.

## Step 0: 進行中の作業を避ける / Avoid in-flight work

オープンな [unit-tests] PR があっても**止まらずに続けます**。その PR が変更しているテストの対象（ファイル名・テストクラスのコメントから読み取る）を、今回の対象から外してください。
Do NOT stop when an open [unit-tests] PR exists. Exclude the targets it covers and continue.

```bash
gh pr list --repo "${{ github.repository }}" --state open --json number,title,files \
  --jq '[.[] | select(.title | startswith("[unit-tests]")) | {number, title, files: [.files[].path]}]'
```

オープンな [unit-tests] PR が 3 件以上あるときだけ、`noop` で終えてください（レビュー待ちが溜まりすぎないように）。
Only if 3 or more [unit-tests] PRs are open, finish with `noop`.

## Step 1: 対象を選ぶ / Pick targets

ワークフローの前段で測ったカバレッジが `/tmp/gh-aw/coverage-summary.txt` にあります（SRCCoreTests と SRCRenderTests を合わせた、SRCCore のファイルごとの行カバレッジ。未カバー行の多い順）。
The baseline coverage is in `/tmp/gh-aw/coverage-summary.txt` (sorted by uncovered lines).

```bash
cat /tmp/gh-aw/coverage-summary.txt
```

- 未カバー行の多いファイルから、Step 0 で除いたものを飛ばして **3〜5 ファイル**を選ぶ
- 純粋な描画・GUI だけのコードや、`TestLib/` は対象にしない
- 既存のテストは `SRC.Sharp/SRCCoreTests/` にある（`Coverage/` 以下は、カバレッジ補強で足したもの）。重複しないように確認する

## Step 2: 仕様を読む / Read the specification

- **元の VB のソース `SRC/SRC_20121125/*.bas`・`*.cls` が仕様**です。C# は VB から関数単位で移植されているので、同じ名前の関数を探して読み比べてください
- コマンド・関数・特殊能力の仕様はヘルプ `SRC.Sharp.Help/src/*.md` にもある
- テストの基盤: `SRC.Sharp/SRCCore/TestLib/`（MockGUI はハンドラを設定して GUI 呼び出しを捕まえられる）
- データ（ユニット・パイロットなど）は、テキストからデータを読み込んで実物のユニットを組み立てるとよい。`SRC.Sharp/SRCCoreTests/Coverage/` の既存のテストに例がある

## Step 3: テストを書く / Write tests

- 新しいファイルを `SRC.Sharp/SRCCoreTests/Coverage/<対象>/` に置き、名前空間は `SRCCore.Coverage.Tests.<対象>Coverage` にする（`SRCCore.Coverage.Tests.Units` のように SRCCore の名前空間と同じ名前の末尾にすると、他のテストの `Units.Unit` の解決を壊すため）
- **挙動を確かめる**: 計算結果・状態の変化・GUI に渡した引数・読み込んだデータなど、ゲームにとって意味のある結果を検証する
- **書かないテスト**: 設定した値をそのまま読み返すだけのもの、モックが返す値の確認、「例外が出ない」だけのもの（例外が出ないこと自体が仕様の場合を除く）、プロパティ・列挙値を並べるだけのもの
- **決定的にする**: 時刻・本物の乱数・テスト順・静的な状態の持ち越しに依存しない（乱数を使う処理は、乱数の影響を受けない入力を選ぶか、既存テストの方法で制御する）
- コメントは日本語で、確かめている規則と、対応する VB の関数を短く書く

## Step 4: 移植の不具合 / Porting bugs

C# の挙動が VB と異なる場合（移植の不具合）:
- 不具合の挙動を期待値にしない
- VB どおりの期待値でテストを書き、`[Ignore("移植の不具合: <1 行の説明>")]` を付ける
- `create-issue` で Issue を作る（1 回の実行で最大 3 件。既存の Issue と重複しないよう検索する）。本文に C# と VB の箇所（file:line）と、テストの名前を書く
- 既に見つかっている不具合の一覧: `docs/porting/coverage-porting-bugs.md`（ここにあるものは Issue を作らない）

## Step 5: 実行して確かめる / Run and validate

```bash
cd SRC.Sharp
dotnet test SRCCoreTests/SRCCoreTests.csproj -p:EnableWindowsTargeting=true 2>&1 | tail -5
dotnet test SRCCoreTests/SRCCoreTests.csproj --collect:"XPlat Code Coverage" --results-directory /tmp/gh-aw/coverage-after 2>&1 | tail -2
```

- [Ignore] 以外のテストはすべて通すこと。全体のテストも通すこと
- 対象ファイルのカバレッジを、前段の数字と比べて報告する

## Step 6: PR を作る / Create the PR

`create-pull-request` で PR を作る。タイトル: `[unit-tests] カバレッジ補強: <対象のファイル>`。本文:

```
## 対象 / Targets
| ファイル | 前 | 後 |
|---|---|---|

## 追加したテスト / Tests added
- <テストクラス>: <確かめている挙動>（N 件）

## 移植の不具合 / Porting bugs
- <概要> → Issue #N（テスト: <名前>、[Ignore]）
- なし / None

## テスト結果 / Test results
Passed: N, Skipped: M, Failed: 0
```

追加できるものが無いときは `noop` で理由を書く。

## 重要な注意事項 / Important notes

- 本体（`SRC.Sharp/SRCCore/`）のコードは変更しない。既存のテストも消さない
- テストはすべて通してから PR を作る（[Ignore] を除く）
- 数より質: 弱いテストを大量に足すより、主要な分岐を確かめる強いテストを優先する
