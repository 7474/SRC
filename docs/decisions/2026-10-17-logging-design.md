# ログを Microsoft.Extensions.Logging にまとめ、デスクトップでは既定でファイルに残す

- 日付: 2026-10-17
- 状態: 採用

## 背景

7474/SRC#358 の時点（3.5.2）では次の問題があった。

- 設定ファイル（`srcs.json`）でログを設定できなかった。レベルを変えられたのは SRC#Form の引数 `-v`・`-vv` だけだった
- SRC#Form 以外はファイルに残らず、デバッガを付けていないと SRCCore のログを見られなかった。SRC#Form は `<exe のフォルダ>/logs` に書いていた
- `SRC.LogError`・`SRC.LogWarn` が `Log.LogDebug` で出していたため、Information 以上の設定ではエラーが記録されなかった
- SRCCore のカテゴリが `SRCCore` の 1 つだけだった。メッセージを文字列連結で作っており、`{}` を含む値で書式が崩れうる箇所もあった
- SRC#Web・SRCDebugAdapter・SRCAuthoring に `Console.WriteLine`・`Console.Error.WriteLine` が散らばっていた
- ログの場所・取り方がヘルプになかった

## 判断

- すべて `Microsoft.Extensions.Logging`（`ILogger`）で書き、出力先はホスト（各アプリ）が決める
- 設定は「引数 > 環境変数 > `srcs.json` > 既定値」の順に効かせ、項目名はすべてのアプリで共通にする
- デスクトップ（SRC#Form・SRC#Avalonia・`srcsharp`）は既定で Information 以上をファイルに残す。場所は実行ファイルのフォルダではなく、OS ごとのユーザーの領域にする
- LSP・DAP では stdout にログを書かず、プロトコルのログ通知で送る
- カテゴリを `SRCCore.Event`・`SRCCore.Expression`・`SRCCore.Command` などに分け、メッセージはテンプレートで書く
- ログの場所・取り方をヘルプ・`srcsharp --help`・VS Code 拡張の README・Issue テンプレートに書く

## 理由

- 不具合報告で、プレイヤーが何もしなくても版・OS・シナリオ・例外が残っているようにするため
- macOS の `.app` や `Program Files`・`dotnet tool` の置き場所には書けないことがあるため
- プロトコルが使う stdout にログが混ざると、LSP・DAP の通信が壊れるため

## 影響

- SRC#Form のログの場所が `<exe のフォルダ>/logs` から OS ごとのユーザーの領域に変わった
- SRC#Form・SRC#Avalonia の設定ダイアログにはログの項目を足していない（画面のテストの基準が変わるため）。`srcs.json` か引数で設定する

## 参照

- [ログの設計](../logging.md)
- 7474/SRC#358
- [.NET のログ](https://learn.microsoft.com/dotnet/core/extensions/logging)
