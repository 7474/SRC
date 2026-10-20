# SRCLintWeb

`srcsharp lint` と同じ検査（SRCAuthoring の `DataLinter`）を、ブラウザの中で動かす WebAssembly。シナリオの zip を、どこにも送らずに検査する。

- スレッドを使わない（SRC#Web と違ってクロスオリジン分離（COOP/COEP）が要らない）ので、普通の頁から読み込める。
- zip の目次を読み、`*.eve` と `*.txt` だけを展開して検査する。画像・音は読まない。
- 汎用データ（`Data/System` の `sp.txt` など）を別の zip から読み込める。シナリオに含まれないスペシャルパワーなどを参照しても、問題にしない。
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
const result = await lintZip(file, { systemArchives: ['/files/packs/src-basic/xxxx.zip'] });
// result.root: zip の最上位のフォルダ（シナリオのフォルダ。イベントの include の基準）
// result.checkedFiles: 検査したファイルの数
// result.diagnostics: [{ file, line, severity: 'error' | 'warning', message, source }]
// result.systemErrors: 読めなかった汎用データの zip
```

- `systemArchives` は汎用データの zip。URL（Range 要求に応えること）か Blob。中の `Data/System/*.txt` だけを読む。読めなかった zip は飛ばし、`result.systemErrors` に理由を入れる。
- イベントの問題（`If` に対応する `EndIf` が無いなど）は、`srcsharp lint` と同じく `warning` になる。

## テスト

`SRC.Sharp/SRCSharpWeb.E2E/tests/lint.spec.js`（Playwright）。CI（`test-web`）は発行物を配信のルートの `/lint/` に置いて確かめる。手元では次のとおり。

```sh
mkdir -p /tmp/lintroot/lint && cp -r out/lint/wwwroot/. /tmp/lintroot/lint/
cd SRC.Sharp/SRCSharpWeb.E2E
SRCWEB_ROOT=/tmp/lintroot SRCWEB_BASE=/lint/ npx playwright test tests/lint.spec.js
```
