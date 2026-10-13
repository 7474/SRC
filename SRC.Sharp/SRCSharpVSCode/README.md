# SRC# Authoring（VS Code 拡張）

SRC のシナリオ（`*.eve`）の編集支援と、イベントのデバッガです。処理は .NET の `srcsharp`（[SRCSharpTools](../SRCSharpTools)）が行い、この拡張は Language Server と Debug Adapter として呼び出します。設計は [docs/tools/authoring.md](../../docs/tools/authoring.md) を参照してください。

## できること

- 構文の色分け（コマンド・ラベル・include・文字列・`$(...)`）
- データファイル（`robot.txt`・`unit.txt`・`pilot.txt` など SRC のデータのファイル名）の `#` のコメントの色分け。ほかのファイルは `files.associations` で `src-data` に結び付ける（[Language identifiers](https://code.visualstudio.com/docs/languages/identifiers)）
- 診断（イベントの読み込みエラー、Call・Goto の飛び先のラベルが無い、include するファイルが無い）。保存前の内容で調べる
- アウトライン（ラベル）、定義へ移動（ラベル・include）、ホバー、補完（コマンド・ラベル）
- デバッガ：ブレークポイント、ステップ実行、Call のスタック、変数、式の評価、イベントのエラーで止める。シナリオは画面なしで動き、入力は `.play` スクリプト（無ければメッセージを送り、選択肢は先頭を選ぶ）で与える
- コマンド：ワークスペースの lint、バランスレポート、このイベントの自動プレイ

## 導入

この版（0.6.0）は Marketplace にまだ出していません（7474/SRC#1175）。Marketplace の [koudenpa.src-vscode](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode)（0.0.5）は色分けだけの旧版で、0.6.0 はその更新として出します。それまでは `srcsharp` と `.vsix` を入れます。

### 1. srcsharp

[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) を入れ、.NET tool として入れます（[.NET tools](https://learn.microsoft.com/dotnet/core/tools/global-tools)、[dotnet tool install](https://learn.microsoft.com/dotnet/core/tools/dotnet-tool-install)）。

```sh
dotnet tool install -g SRCSharpTools
```

NuGet に無い版（公開前や開発中の版）は、リポジトリから作って入れます（[dotnet pack](https://learn.microsoft.com/dotnet/core/tools/dotnet-pack)）。

```sh
dotnet pack SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj -c Release -o out/srcsharp
dotnet tool install -g SRCSharpTools --add-source out/srcsharp
```

入れたら、新しい端末で `srcsharp --help` が動くことを確かめます。グローバルツールは `~/.dotnet/tools`（Windows は `%USERPROFILE%\.dotnet\tools`）に入り、PATH に通っている必要があります。

### 2. 拡張

`.vsix` は、CI（`CI.yml`）の成果物 `src-vscode-vsix` から取るか、手元で作ります。

```sh
cd SRC.Sharp/SRCSharpVSCode
npm ci
npm run package   # esbuild でまとめ、src-vscode-<バージョン>.vsix を作る
code --install-extension src-vscode-0.6.0.vsix
```

VS Code の拡張機能ビューの「…」→「Install from VSIX...」でも入れられます（[Install from a VSIX](https://code.visualstudio.com/docs/configure/extensions/extension-marketplace#_install-from-a-vsix)）。

拡張は esbuild で 1 ファイル（`out/extension.js`）にまとめるため、`.vsix` に `node_modules` は要りません（[Bundling Extensions](https://code.visualstudio.com/api/working-with-extensions/bundling-extension)）。開発中は `npm run compile`（型の確認とまとめ）か `npm run watch` を使います。

### リポジトリの srcsharp を使う

ビルドした dll を直接動かす場合は、設定を次のようにします。

```json
{
  "srcsharp.serverPath": "dotnet",
  "srcsharp.serverArgs": ["<リポジトリ>/SRC.Sharp/SRCSharpTools/bin/Debug/net10.0/srcsharp.dll"]
}
```

## 設定

| 設定 | 既定 | 内容 |
|---|---|---|
| `srcsharp.serverPath` | `srcsharp` | 動かすコマンド。dll を動かす場合は `dotnet` |
| `srcsharp.serverArgs` | `[]` | `lsp`・`dap` などのコマンドの前に付ける引数（dll のパス、`--lang` など） |
| `srcsharp.appPath` | 空 | SRC 本体のフォルダ。Lib や共通の Data を探す。空ならシナリオのフォルダの親から探す |

設定は拡張の起動時に読みます。Language Server に関わる設定（3 つとも）を変えたら、ウィンドウを再読み込み（「Developer: Reload Window」）してください。デバッガとコマンドは、実行のたびに `serverPath`・`serverArgs` を読みます。

## デバッガ

実行とデバッグのビューで launch.json を作り、デバッガに「SRC# イベント」を選ぶと、1 つ目の構成（開いているファイルを実行）ができます。2 つ目は「構成の追加...」のスニペット「SRC#: .play スクリプトで実行」です（[Debugging](https://code.visualstudio.com/docs/debugtest/debugging)、[Debug configuration](https://code.visualstudio.com/docs/debugtest/debugging-configuration)）。

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "srcsharp",
      "request": "launch",
      "name": "SRC# イベントを実行",
      "program": "${file}",
      "stopOnEntry": true
    },
    {
      "type": "srcsharp",
      "request": "launch",
      "name": "SRC# スクリプトで実行",
      "program": "${workspaceFolder}/test.eve",
      "script": "${workspaceFolder}/test.play"
    }
  ]
}
```

| 属性 | 既定 | 内容 |
|---|---|---|
| `program` | （必須） | 実行するイベントファイル |
| `script` | なし | 入力を与える `.play` スクリプト（書式は [E2E テスト方針](../../docs/gui/e2e-testing.md) の PlayScript） |
| `appPath` | シナリオのフォルダ | SRC 本体のフォルダ |
| `stopOnEntry` | `false` | 最初のコマンドで止める |
| `maxSteps` | `100000` | `script` が無いときに自動で進める操作の上限 |

パスは `${workspaceFolder}` などで絶対パスにしてください。`request` は `launch` だけです（`attach` はありません）。

- **ブレークポイント**：`*.eve` の行に置く（include 先も）。条件・ヒット数・ログポイントには対応していない
- **ステップ**：ステップイン（次のコマンド）、ステップオーバー（同じか浅い Call の深さの次のコマンド）、ステップアウト（Call から戻るまで）。一時停止は次のコマンドで止める
- **エラー**：イベントのエラーで止まり、メッセージをデバッグコンソールに出す
- **呼び出し履歴**：現在の行と、Call した行
- **変数**：「ローカル」（Local の変数とイベントのローカル変数）と「グローバル」
- **式の評価**：止まっている間、デバッグコンソールとホバーで SRC の式を評価する
- **出力**：`.play` スクリプトの実行の記録と、進めなくなったときの記録をデバッグコンソールに出す

止めている間は、自動の入力も止まります。

## 文字コード

`*.eve` の既定の文字コードを Shift_JIS にし、自動判別（`files.autoGuessEncoding`）を有効にしています（`configurationDefaults`）。UTF-8 のシナリオでは、`[src-eve]` の `files.encoding` を `utf8` にしてください（[File encoding support](https://code.visualstudio.com/docs/editing/codebasics#_file-encoding-support)）。

```json
{
  "[src-eve]": {
    "files.encoding": "utf8"
  }
}
```

Language Server はエディタで開いたファイルの内容を受け取るため、文字コードの影響を受けません。include 先など開いていないファイルは、SRC 本体と同じ読み込み処理で読みます（Shift_JIS も読める）。

## 表示言語

`srcsharp` の表示言語（[多言語対応](../../docs/i18n.md)）は、`srcsharp.serverArgs` に `--lang` を入れて切り替えます。環境変数 `SRCSHARP_LANG` を設定して VS Code を起動してもよいです。

```json
{
  "srcsharp.serverArgs": ["--lang", "en"]
}
```

切り替わるのは lint・バランスレポート・自動プレイのコマンドの出力（出力パネルの「SRC#」）だけです。エディタの診断とデバッガのメッセージは日本語のままです。

## トラブルシューティング

| 症状 | 確かめること |
|---|---|
| 拡張の起動時にエラーが出て、診断やアウトラインが出ない | 端末で `srcsharp --help` が動くか。PATH に `~/.dotnet/tools` が無ければ通すか、`srcsharp.serverPath` に絶対パスを書く。tool を入れた後は VS Code を起動し直す |
| `You must install .NET to run this application` と出る | .NET 10 のランタイムがあるか。既定以外の場所に入れた場合は環境変数 `DOTNET_ROOT` を設定する（[.NET environment variables](https://learn.microsoft.com/dotnet/core/tools/dotnet-environment-variables)） |
| `Cannot find module 'vscode-languageclient/node'` と出る | esbuild でまとめる前の古い `.vsix`（v0.5.0 までの `npm run package` で作ったもの）。`npm run package` で作り直す |
| 日本語が文字化けする | ファイルの文字コード。ステータスバーの文字コードから「エンコード付きで再度開く」で Shift_JIS か UTF-8 を選ぶ |
| システムのラベル（汎用戦闘アニメなど）が見つからないと警告される | `srcsharp.appPath` に Lib のある SRC 本体のフォルダを設定する |
| デバッガが止まらない・すぐ終わる | `program`・`script` が絶対パスか。ブレークポイントがコマンドの行にあるか |
| 診断の動きを詳しく見たい | 「Developer: Set Log Level...」で「SRC#」を Trace にし、出力パネルの「SRC#」で LSP のやり取りを見る |

## ビルド

```sh
npm ci
npm run compile
npm run package   # .vsix を作る（vsce が必要。無ければ npx @vscode/vsce package）
```

CI（`.github/workflows/CI.yml`）はコンパイルだけを確かめ、`.vsix` は作りません。拡張のバージョン（`package.json`）は `SRC.Sharp/Directory.Build.props` のライブラリのバージョンに手で合わせます（[バージョンの管理](../../docs/versioning.md)）。
