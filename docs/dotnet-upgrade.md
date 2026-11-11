# .NET のメジャーの更新

SRC# のアプリ・ツール・テストが使う .NET のメジャーを上げる手順と、.NET 11 に上げたときに変えたこと。今は .NET 11（11.0.0、2026-11-10 の正式版）で動かしている。

## 今の構成

| 何 | どこ | 内容 |
|---|---|---|
| ターゲットフレームワーク | `SRC.Sharp/Directory.Build.props` の `SrcSharpTargetFramework` | 既定は `net11.0`。各プロジェクトは `$(SrcSharpTargetFramework)` を使う（SRC#Form は `$(SrcSharpTargetFramework)-windows`）。ライブラリの SRCCore・SRCRender・SRCSound は `netstandard2.1` のまま |
| パッケージの版 | `SRC.Sharp/Directory.Packages.props` | .NET の版（`Microsoft.AspNetCore.Components.WebAssembly*`・`Microsoft.Extensions.*`・`System.*`）は 11.0.0。SRC#Web だけ SkiaSharp を 4.156 の RC にする（下の「.NET 11 で変えたこと」） |
| SDK | ワークフローの `dotnet-version: 11.0.x` | CI・Packages・Publish-*・CD-* |
| Docker | `SRC.Sharp/SRCDataLinter/Dockerfile` | `mcr.microsoft.com/dotnet/*:11.0-resolute`（Ubuntu 26.04。.NET 11 には noble 版のイメージが無い） |

次の .NET を手元で試すときは、その SDK を入れて `-p:SrcSharpTargetFramework=<TFM>` を付ける。

```bash
dotnet build SRC.Sharp/SRC.Sharp.sln -p:SrcSharpTargetFramework=net12.0 -p:EnableWindowsTargeting=true
```

.NET 11 のときは、プレビュー・RC の SDK で毎週ビルドとテストを流すワークフロー（`dotnet-next.yml`）を置いていた。.NET 11 に上げたので消した。次の .NET のプレビューが出たら、[PR #1452](https://github.com/7474/SRC/pull/1452) の `dotnet-next.yml` を元に作り直す。

## .NET 11 で変えたこと

| 何 | 内容 | 対応 |
|---|---|---|
| SRC#Web の SkiaSharp | SkiaSharp の WebAssembly のネイティブライブラリは Emscripten の版ごとに入っている。.NET 11 は Emscripten 6.0.2 を使うが、正式版の 4.155.0 には 3.1.34・3.1.56・5.0.6 向けしか無く、リンクできない（`undefined symbol: saveSetjmp`） | SRC#Web だけ 4.156.0-rc.1（6.0.2 向けが入っている）を使う（`Directory.Packages.props` の `MSBuildProjectName == 'SRCSharpWeb'` の `ItemGroup`）。デスクトップは 4.155.0 のまま。4.156.0 の正式版が出たら全体をそれにして、この `ItemGroup` を消す |
| SRC#Web が始まらない | マルチスレッドの WebAssembly で、起動の後に作ったスレッドが読み込み前の Web Worker に当たると止まる。SRC#Web では最初のタイマー（`Task.Delay` など）で止まり、そのあとは JS の呼び出しの結果も戻らず「読み込み中」のままになる。.NET 11.0.0 の正式版でも直っていない（[dotnet/runtime#136492](https://github.com/dotnet/runtime/issues/136492)。Emscripten の更新で `worker.loaded` が立たなくなった） | `index.html` の `Blazor.start` で、起動時に読み込んでおく Web Worker を増やす（`withConfig({ pthreadPoolInitialSize: 24 })`。既定は 7）。上流で直ったら消す |
| Blazor WebAssembly のパッケージ | `Microsoft.AspNetCore.Components.WebAssembly`（`.DevServer`）は SDK とメジャーをそろえる | 11.0.0 にした |
| SRC#Form の WinForms のアナライザー | Designer のファイルに手で書いたメンバー（VB の WithEvents から変換したプロパティ）が WFO3001 のエラーになる | プロパティを `*.Events.cs` に移した（動きは変わらない） |
| SRC#Form の警告 | WFO3002（Designer のフィールドはメソッドの後ろに置く）・WFO2000（`CurrentPaintFont` が毎回新しいものを返す） | WFO3002 はフィールドの宣言を `InitializeComponent` の後ろに移した。WFO2000 は写しを返すのが意図なので、理由を書いて抑えた |

ほかの依存パッケージは版を変えずに .NET 11 で動く。

## サポートの期間

.NET 11 は STS で、STS のサポートは 24 か月になった（[.NET STS releases supported for 24 months](https://devblogs.microsoft.com/dotnet/dotnet-sts-releases-supported-for-24-months/)）。.NET 10（LTS）と同じ 2028 年 11 月に終わる（[releases.json](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/11.0/releases.json) の `eol-date` は 2028-11-14。[.NET のサポートポリシー](https://dotnet.microsoft.com/platform/support/policy/dotnet-core)）。

上げると、SRC#Form を使う人は .NET 11 のデスクトップ ランタイムを入れ直すことになり、VS Code 拡張の `srcsharp` は .NET 11 の SDK が要る。

## 上げる手順

次の .NET の正式版が出てから行う（[.NET のリリース](https://github.com/dotnet/core/tree/main/release-notes)）。

1. `SRC.Sharp/Directory.Build.props` の `SrcSharpTargetFramework` の既定を上げる。ターゲットを直に書いているプロジェクト（`SRC.Sharp` の外の `tools/pack-analysis`・`tools/pack-stats`）も上げる
2. `SRC.Sharp/Directory.Packages.props` の .NET の版（`Microsoft.AspNetCore.Components.WebAssembly*`・`Microsoft.Extensions.*`・`System.*`）を新しい正式版にする。SRC#Web の SkiaSharp は、その .NET の Emscripten の版のネイティブライブラリが入っているかを確かめる（`skiasharp.nativeassets.webassembly` のパッケージの `buildTransitive/netstandard1.0/libSkiaSharp.a/<Emscripten の版>/`）
3. ワークフローの `dotnet-version` を上げる（CI・Packages・Publish-*・CD-*）
4. `SRC.Sharp/SRCDataLinter/Dockerfile` の土台のイメージを上げる（Dependabot の対象外。`.github/dependabot.yml`）。Ubuntu の版のタグがあるかを [MCR](https://mcr.microsoft.com/v2/dotnet/runtime/tags/list) で確かめる
5. 版を書いている所を直す: `README.md`、ヘルプ（動作環境・SRCSharpのインストール・SRCSharpFormの使い方・インストール時のエラーへの対処）、VS Code 拡張（`src/server.ts` の `DOTNET_DOWNLOAD_URL`・`src/settings.ts`・`package.json`・`README.md`）、`docs/logging.md`、`docs/releases/template.md`、出力先のパス（`bin/*/net11.0/`）を書いた文書とスクリプト（`tools/compat_play.py`・`SRC.Sharp/Compat`・`tools/pack-*`）
6. 新しい警告を直すか、理由を書いて抑える
7. SRC#Web を発行して E2E を流す（マルチスレッドの WebAssembly は .NET の版ごとに挙動が変わりやすい）。AOT 版の起動時間（`.github/perf-baseline.json`）を見直す
8. リリースの案内に、要るランタイム・SDK が変わることを書く（[リリースの案内](./releases/README.md)）
