# SRCLintWeb

`srcsharp lint` と同じ検査（SRCAuthoring の `DataLinter`）と静的解析（`ScenarioAnalyzer`。VS Code 拡張の LSP と同じ）を、ブラウザの中で動かす WebAssembly。シナリオの zip を、どこにも送らずに検査する。

| 検査 | 内容 |
|---|---|
| データ | `*.txt`（ユニット・パイロット・アイテム・メッセージ・戦闘アニメなど）を SRCCore の読み込み処理で読む。存在しないスペシャルパワーの参照など |
| イベント | `*.eve` の読み込み（`If` と `EndIf` の対応など） |
| 静的解析 | `Call`・`Goto` の飛び先のラベルが無い、include（`<ファイル>`）するファイルが無い。入口のイベントファイル（ほかから include されず、シナリオの `Lib` の下でないもの）ごとに解析する |
| 素材の参照（`assets: true` のとき） | イベントとデータ（`robot.txt`・`pilot.txt`・`animation.txt` など）に書かれた画像・音・マップのファイル名（`.bmp`・`.png`・`.mid`・`.mp3`・`.wav`・`.map` など）が、シナリオの zip か汎用データの zip の目次にあるか。SRC は `Bitmap\Unit` などのフォルダを順に探すので、パスの末尾が一致すればあるものとする。式（`$(...)`）で組み立てる名前とダミー（`-.bmp`）は見ない。見つからないものは `info`（素材を同梱せずに公開するシナリオは多いため）。SRCAuthoring の `AssetReferenceChecker` |

- スレッドを使わない（SRC#Web と違ってクロスオリジン分離（COOP/COEP）が要らない）ので、普通の頁から読み込める。
- 検査は Web Worker（`srclint-worker.js`）で動かす。頁は固まらず、進み具合を表示でき、中止できる。module の Worker を作れない環境では頁のスレッドで動かす（そのあいだ頁は固まる）。

| ファイル | 役割 |
|---|---|
| `srclint.js` | 使う側が読み込む入口。Worker を作り、検査を頼んで進み具合と結果を受け取る |
| `srclint-worker.js` | Worker の中身。`srclint-core.js` を呼ぶ |
| `srclint-core.js` | zip の読み取りと .NET の実行環境の呼び出し（`Program.cs` の `LintExports`）。進み具合は C# から `[JSImport("progress", "srclint")]` で受け取る |
- zip の目次を読み、`*.eve` と `*.txt` だけを展開して検査する。画像・音は展開しない（素材の参照は目次の名前で確かめる）。
- 汎用データを別の zip から読み込める。`Data/System/*.txt`（`sp.txt` など）と `Lib/**/*.eve`（汎用戦闘アニメなど）。シナリオに含まれないスペシャルパワーや、汎用戦闘アニメのラベル（`AttackDemo` など）を参照しても問題にしない。`Lib` のイベントはラベルと include の行だけを読む（汎用戦闘アニメは 8 万行あるため）。
- 文字コードの警告（Shift_JIS のデータ）は出さない。SRC# は Shift_JIS も読めるため。

## 発行

```sh
dotnet workload install wasm-tools
dotnet publish SRC.Sharp/SRCLintWeb/SRCLintWeb.csproj -c Release -o out/lint
```

`out/lint/wwwroot` を配信する。`index.html` は zip を選んで結果を表にする頁。

## 使い方（JavaScript）

```js
import { lintZip, preload } from '/lint/srclint.js';

preload(); // 実行環境を先に読み込む（任意）
const controller = new AbortController(); // 中止する場合（任意）
const result = await lintZip(file, {
  systemArchives: ['/files/packs/src-basic/xxxx.zip'],
  assets: true,
  onProgress: p => console.log(p.phase, p.file, p.done, p.total),
  signal: controller.signal,
});
// result.root: zip の最上位のフォルダ（シナリオのフォルダ。イベントの include の基準）
// result.checkedFiles: 検査したファイルの数
// result.diagnostics: [{ file, line, severity: 'error' | 'warning' | 'info', message, source }]
// result.systemErrors: 読めなかった汎用データの zip
```

- `systemArchives` は汎用データの zip。URL（Range 要求に応えること）か Blob。中の `Data/System/*.txt` と `Lib/**/*.eve` だけを読む。読めなかった zip は飛ばし、`result.systemErrors` に理由を入れる。
- イベントの問題（`If` に対応する `EndIf` が無いなど）は、`srcsharp lint` と同じく `warning` になる。
- `onProgress` は進み具合を受け取る。`phase` は `scenario`（zip からテキストを取り出す）→ `system`（汎用データの zip を読む）→ `runtime`（実行環境の読み込みを待つ）→ `data`（データとイベントの検査）→ `analyze`（静的解析）→ `assets`（素材の参照）の順に進む（`PHASES`）。`file` はそのとき扱っているもの、`done` は済んだ件数、`total` は全体の件数。
- `signal` で中止すると `AbortError` で終わる。C# の検査は途中で止められないので Worker ごと止め、次の検査で作り直す（実行環境の読み込みからやり直す）。
- 検査は 1 つずつ順に行う（続けて呼ぶと前の検査が終わってから始まる）。
- `assets: true` なら素材の参照も確かめる（見つからないものは `info`）。汎用データの zip は、`Data/System` と `Lib` だけでなく目次のすべての名前を素材として使う。読めなかった汎用データの zip にある素材は「見つからない」になる。

## テスト

`SRC.Sharp/SRCSharpWeb.E2E/tests/lint.spec.js`（Playwright）。CI（`test-web`）は発行物を配信のルートの `/lint/` に置いて確かめる。手元では次のとおり。

```sh
mkdir -p /tmp/lintroot/lint && cp -r out/lint/wwwroot/. /tmp/lintroot/lint/
cd SRC.Sharp/SRCSharpWeb.E2E
SRCWEB_ROOT=/tmp/lintroot SRCWEB_BASE=/lint/ npx playwright test tests/lint.spec.js
```
