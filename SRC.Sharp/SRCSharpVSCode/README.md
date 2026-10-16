# SRC# Authoring（VS Code 拡張）

SRC のシナリオ（`*.eve`）の編集支援と、イベントのデバッガです。処理は .NET の `srcsharp`（[SRCSharpTools](../SRCSharpTools)）が行い、この拡張は Language Server と Debug Adapter として呼び出します。設計は [docs/tools/authoring.md](../../docs/tools/authoring.md) を参照してください。

## できること

- 構文の色分け（コマンド・ラベル・include・文字列・`$(...)`）
- データファイル（`robot.txt`・`unit.txt`・`pilot.txt` など SRC のデータのファイル名）の `#` のコメントの色分け。ほかのファイルは `files.associations` で `src-data` に結び付ける（[Language identifiers](https://code.visualstudio.com/docs/languages/identifiers)）
- 診断（イベントの読み込みエラー、Call・Goto の飛び先のラベルが無い、include するファイルが無い）。保存前の内容で調べる
- アウトライン（ラベル）、定義へ移動（ラベル・include）、ホバー、補完（コマンド・ラベル）
- デバッガ：ブレークポイント、ステップ実行、Call のスタック、変数、式の評価、イベントのエラーで止める。シナリオは画面なしで動き、入力は `.play` スクリプト（無ければメッセージを送り、選択肢は先頭を選ぶ）で与える
- 画面：デバッグ中のシナリオの画面を「SRC# 画面」のビューに出す。止まった時と終わった時に取り直し、実行中は 0.5 秒ごとに取り直す
- 操作パネル：アクティビティバーの「SRC#」。srcsharp・Language Server・SRC 本体のフォルダの状態と、実行・デバッグ・画面・lint・バランスレポート・設定を並べる
- 設定画面：srcsharp の場所・引数・SRC 本体のフォルダ・表示言語をフォームで設定する（「SRC#: 設定を開く」）
- コマンド：ワークスペースの lint、バランスレポート、このイベントの自動プレイ

## 導入

Marketplace の [koudenpa.src-vscode](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode) の更新として出します。0.0.5 は色分けだけの旧版です。0.5.0 はプレリリースで出すため、拡張機能ビューでこの拡張の「Switch to Pre-Release Version」を選ぶと入ります（[Pre-release extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension#prerelease-extensions)）。正式版は、`srcsharp` が NuGet に出てから偶数マイナー（0.6.0 など）で出します（7474/SRC#1175）。`.vsix` から入れることもできます。

### かんたんな手順

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) を入れる
2. この拡張を入れる
3. `*.eve` を開くか、アクティビティバーの「SRC#」を開き、「srcsharp が見つかりません」の通知か操作パネルの「srcsharp を入れる」を押す

3 は `dotnet tool update -g SRCSharpTools`（入っていなければ入れる）を動かします（[dotnet tool update](https://learn.microsoft.com/dotnet/core/tools/dotnet-tool-update)）。入れた後は Language Server をすぐ起動し、VS Code の再起動は要りません。更新も操作パネルの「srcsharp を入れる／更新する」で行います。

拡張は srcsharp を PATH と `~/.dotnet/tools`（Windows は `%USERPROFILE%\.dotnet\tools`）から探します。dotnet が既定以外の場所（`~/.dotnet` など）にあれば、`DOTNET_ROOT` を設定して srcsharp を動かします。

ワークスペースの中（2 階層下まで）に SRC 本体のフォルダ（`Lib` か `Data/System` のあるフォルダ）があり、シナリオのフォルダの親からは見つからない場合は、`srcsharp.appPath` に設定するか尋ねます。

### 1. srcsharp（手で入れる場合）

[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) を入れ、.NET tool として入れます（[.NET tools](https://learn.microsoft.com/dotnet/core/tools/global-tools)、[dotnet tool install](https://learn.microsoft.com/dotnet/core/tools/dotnet-tool-install)）。

```sh
dotnet tool install -g SRCSharpTools
```

NuGet に無い版（公開前や開発中の版）は、リポジトリから作って入れます（[dotnet pack](https://learn.microsoft.com/dotnet/core/tools/dotnet-pack)）。

```sh
dotnet pack SRC.Sharp/SRCSharpTools/SRCSharpTools.csproj -c Release -o out/srcsharp
dotnet tool install -g SRCSharpTools --add-source out/srcsharp
```

入れたら、新しい端末で `srcsharp --help` が動くことを確かめます。グローバルツールは `~/.dotnet/tools`（Windows は `%USERPROFILE%\.dotnet\tools`）に入ります。拡張はここも探すため、PATH に通っていなくても動きます。

### 2. 拡張

`.vsix` は、CI（`CI.yml`）の成果物 `src-vscode-vsix` から取るか、手元で作ります。

```sh
cd SRC.Sharp/SRCSharpVSCode
npm ci
npm run package   # esbuild でまとめ、src-vscode-<バージョン>.vsix を作る
code --install-extension src-vscode-0.5.1.vsix
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

設定画面（「SRC#: 設定を開く」、操作パネルの「設定」、状態バーの「SRC#」）で、srcsharp の状態（見つかった場所・版）を見ながら設定できます。保存先はワークスペースかユーザーを選びます。SRC 本体のフォルダは「SRC#: SRC 本体のフォルダを選ぶ」でも選べます（ワークスペースで見つかったフォルダが候補に出る）。

| 設定 | 既定 | 内容 |
|---|---|---|
| `srcsharp.serverPath` | `srcsharp` | 動かすコマンド。既定なら PATH と `~/.dotnet/tools` から探す。dll を動かす場合は `dotnet` |
| `srcsharp.serverArgs` | `[]` | `lsp`・`dap` などのコマンドの前に付ける引数（dll のパスなど） |
| `srcsharp.appPath` | 空 | SRC 本体のフォルダ。Lib や共通の Data を探す。空ならシナリオのフォルダの親から探す |
| `srcsharp.language` | 空 | srcsharp の表示言語（`ja`・`en`・`auto` など）。環境変数 `SRCSHARP_LANG` として渡す |
| `srcsharp.screen.autoOpen` | `true` | デバッグを始めたら画面のビューを開く |

`serverPath`・`serverArgs`・`appPath`・`language` を変えると、Language Server を起動し直します（ウィンドウの再読み込みは要りません）。デバッガとコマンドは、実行のたびに設定を読みます。

## デバッガ

`*.eve` を開いて、エディタの右上の実行ボタン（「開いているイベントを実行（画面付き）」「開いているイベントをデバッグ」）か操作パネルから始められます。launch.json が無くても、F5 で開いているイベントを実行します（最初のコマンドで止める）。

細かく指定する場合は、実行とデバッグのビューで launch.json を作り、デバッガに「SRC# イベント」を選ぶと、1 つ目の構成（開いているファイルを実行）ができます。2 つ目は「構成の追加...」のスニペット「SRC#: .play スクリプトで実行」です（[Debugging](https://code.visualstudio.com/docs/debugtest/debugging)、[Debug configuration](https://code.visualstudio.com/docs/debugtest/debugging-configuration)）。

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

## 画面

SRC# のデバッグを始めると「SRC# 画面」のビューを横に開き、シナリオの画面（最後に表示したフレーム）を出します。開かない場合は `srcsharp.screen.autoOpen` を `true` にするか、「SRC#: 画面を表示」で開きます。

- 止まった時（ブレークポイント・ステップ・エラー）と、シナリオが終わった時に取り直す
- 実行中は 0.5 秒ごとに取り直す（ビューが見えている間だけ）
- 「PNG で保存」で、表示中の画面をファイルに保存する

画面はデバッグアダプタの独自のリクエスト `srcsharpScreen` で取ります。0.5.1 までの srcsharp は対応していないため、「srcsharp を入れる／更新する」で更新してください（NuGet にまだ無い場合は、リポジトリから作って入れる）。画面は表示だけで、クリックなどの入力は `.play` スクリプトか自動の入力で与えます。

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

`srcsharp` の表示言語（[多言語対応](../../docs/i18n.md)）は、`srcsharp.language`（設定画面の「表示言語」）で切り替えます。`srcsharp.serverArgs` に `--lang` を入れても、環境変数 `SRCSHARP_LANG` を設定して VS Code を起動してもよいです（`--lang` が優先）。

```json
{
  "srcsharp.language": "en"
}
```

切り替わるのは lint・バランスレポート・自動プレイのコマンドの出力（出力パネルの「SRC#」）だけです。エディタの診断とデバッガのメッセージは日本語のままです。

## トラブルシューティング

| 症状 | 確かめること |
|---|---|
| 拡張の起動時にエラーが出て、診断やアウトラインが出ない | 操作パネルの「状態」と設定画面で、srcsharp が見つかっているか、Language Server が動いているか。見つからなければ「srcsharp を入れる」か、`srcsharp.serverPath` に絶対パスを書く |
| `You must install .NET to run this application` と出る | .NET 10 のランタイムがあるか。拡張は PATH・`DOTNET_ROOT`・`~/.dotnet` から dotnet を探して `DOTNET_ROOT` を渡す。ほかの場所に入れた場合は環境変数 `DOTNET_ROOT` を設定する（[.NET environment variables](https://learn.microsoft.com/dotnet/core/tools/dotnet-environment-variables)） |
| 画面のビューに「この srcsharp は画面の取得に対応していません」と出る | srcsharp が 0.5.1 以前。「srcsharp を入れる／更新する」で更新する |
| `Cannot find module 'vscode-languageclient/node'` と出る | esbuild でまとめる前の古い `.vsix`（v0.5.0 までの `npm run package` で作ったもの）。`npm run package` で作り直す |
| 日本語が文字化けする | ファイルの文字コード。ステータスバーの文字コードから「エンコード付きで再度開く」で Shift_JIS か UTF-8 を選ぶ |
| システムのラベル（汎用戦闘アニメなど）が見つからないと警告される | 「SRC#: SRC 本体のフォルダを選ぶ」で Lib のある SRC 本体のフォルダを設定する |
| デバッガが止まらない・すぐ終わる | `program`・`script` が絶対パスか。ブレークポイントがコマンドの行にあるか |
| 診断の動きを詳しく見たい | 「Developer: Set Log Level...」で「SRC#」を Trace にし、出力パネルの「SRC#」で LSP のやり取りを見る |

## ビルド

```sh
npm ci
npm run compile
npm run package   # .vsix を作る（vsce は devDependencies に入っている）
```

CI（`.github/workflows/CI.yml`）は `.vsix` を作り、成果物 `src-vscode-vsix` として残します。拡張のバージョン（`package.json`）は SRC# 本体とは別に上げます。公開は `.github/workflows/Publish-VSCode.yml` が行います（[バージョンの管理](../../docs/versioning.md#vs-code-拡張)）。
