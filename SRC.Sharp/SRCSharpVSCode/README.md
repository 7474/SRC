# SRC# Authoring（VS Code 拡張）

SRC のシナリオ（`*.eve`）の編集支援と、イベントのデバッガです。処理は .NET の `srcsharp`（[SRCSharpTools](../SRCSharpTools)）が行い、この拡張は Language Server と Debug Adapter として呼び出します。設計は [docs/tools/authoring.md](../../docs/tools/authoring.md) を参照してください。

## できること

- 構文の色分け（コマンド・ラベル・include・文字列・`$(...)`）
- 診断（イベントの読み込みエラー、Call・Goto の飛び先のラベルが無い、include するファイルが無い）。保存前の内容で調べる
- アウトライン（ラベル）、定義へ移動（ラベル・include）、ホバー、補完（コマンド・ラベル）
- デバッガ：ブレークポイント、ステップ実行、Call のスタック、変数、式の評価、イベントのエラーで止める。シナリオは画面なしで動き、入力は `.play` スクリプト（無ければメッセージを送り、選択肢は先頭を選ぶ）で与える
- コマンド：ワークスペースの lint、バランスレポート、このイベントの自動プレイ

## 準備

```sh
dotnet tool install -g SRCSharpTools   # srcsharp コマンド
```

リポジトリから動かす場合は `srcsharp.serverPath` に `dotnet`、`srcsharp.serverArgs` に `["<リポジトリ>/SRC.Sharp/SRCSharpTools/bin/Debug/net10.0/srcsharp.dll"]` を設定します。

文字コードは、`*.eve` の既定を Shift_JIS（自動判別あり）にしています。UTF-8 のシナリオでは `files.encoding` を設定してください。

## ビルド

```sh
npm ci
npm run compile
npm run package   # .vsix を作る（@vscode/vsce が必要）
```
