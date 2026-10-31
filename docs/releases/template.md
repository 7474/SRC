SRC# {app} です。アプリは `{app}`、ライブラリ・ツールと VS Code 拡張は `{library}` です（[バージョンの管理](https://github.com/7474/SRC/blob/master/docs/versioning.md)）。

<!-- TODO: 今回の要点を 1〜3 行で書く（例: 「マップコマンド『オートプレイ』を加えました。」）。無ければこのコメントごと消す -->

## 遊ぶ

| | 入手 |
|---|---|
| ブラウザ | **SRC#Web** https://src.koudenpa.net/ ・ https://7474.github.io/SRC/web/ 。自分のサイトに置くための `SRCSharpWeb.zip` も添付しています（[外部のサイトに置く](https://github.com/7474/SRC/blob/master/docs/gui/web-hosting.md)） |
| Windows・macOS・Linux | **SRC#Avalonia** `SRCSharpAvalonia-<OS>.zip` |
| Windows（x64） | **SRC#Form** `SRCSharpForm.zip`（[.NET 10 デスクトップ ランタイム](https://dotnet.microsoft.com/download/dotnet/10.0) の x64 版が必要） |
| データを見る | **データビューア** https://7474.github.io/SRC/ ・ https://srcv.7474.jp/ |

## 動きが変わる点

<!-- TODO: 既存のシナリオ・セーブデータ・設定で、見え方や結果が変わるものを書く。無ければこの節ごと消す -->

## 主な変更

{changes}
## パッケージ

- NuGet: `SRCCore` {library}・`SRCDataLinter` {library}・`SRCSharpTools` {library}（更新するには `dotnet tool update -g SRCSharpTools`）
- Docker Hub: `koudenpa/srcdatalinter:v{app}`
- VS Code: Marketplace の `koudenpa.src-vscode` {library}。`src-vscode-{library}.vsix` も添付しています。

本リポジトリで配布しているソフトウェアを使うときは、[SRC 派生版ソフトウェアの利用における基本的規則](https://github.com/7474/SRC#src-派生ソフトウェアとしての規約)を守ってください。
