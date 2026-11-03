# .NET のメジャーの更新

SRC# のアプリ・ツール・テストが使う .NET のメジャーを上げる手順。今は .NET 10 で動かし、次の .NET 11 で動くかどうかを別のワークフローで確かめている。

## 今の構成

| 何 | どこ | 内容 |
|---|---|---|
| ターゲットフレームワーク | `SRC.Sharp/Directory.Build.props` の `SrcSharpTargetFramework` | 既定は `net10.0`。各プロジェクトは `$(SrcSharpTargetFramework)` を使う（SRC#Form は `$(SrcSharpTargetFramework)-windows`）。ライブラリの SRCCore・SRCRender・SRCSound は `netstandard2.1` のまま |
| 次の .NET の版 | `SRC.Sharp/Directory.Packages.props` の `SrcSharpTargetFramework == 'net11.0'` の `ItemGroup` | `-p:SrcSharpTargetFramework=net11.0` のときだけ使うパッケージの版 |
| 次の .NET で確かめる | `.github/workflows/dotnet-next.yml` | .NET 11 のプレビュー・RC の SDK でビルドとテスト（SRC#Web の E2E まで）をする。手動・毎週・`Directory.*.props` を変えた PR で動く。必須チェックではない |

手元で試すときは、.NET 11 の SDK を入れて次のように実行する。

```bash
dotnet build SRC.Sharp/SRC.Sharp.sln -p:SrcSharpTargetFramework=net11.0 -p:EnableWindowsTargeting=true
```

```bash
dotnet test SRC.Sharp/SRCCoreTests/SRCCoreTests.csproj -p:SrcSharpTargetFramework=net11.0
```

## .NET 11 で変わること（2026-11 の RC3 で確かめたこと）

| 何 | 内容 | 対応 |
|---|---|---|
| SRC#Web の SkiaSharp | SkiaSharp の WebAssembly のネイティブライブラリは Emscripten の版ごとに入っている。.NET 11 は Emscripten 6.0.2 を使い、4.154.0（Emscripten 3.1.34・3.1.56）ではリンクできない（`undefined symbol: saveSetjmp`） | Emscripten 6.0.2 のライブラリが入った 4.156.0 以降を使う。今は 4.156.0-preview.1 を .NET 11 のときだけ使う |
| Blazor WebAssembly のパッケージ | `Microsoft.AspNetCore.Components.WebAssembly`（`.DevServer`）は SDK とメジャーをそろえる | .NET 11 のときだけ 11 の版を使う |
| SRC#Form の WinForms のアナライザー | Designer のファイルに手で書いたメンバー（VB の WithEvents から変換したプロパティ）が WFO3001 のエラーになる | プロパティを `*.Events.cs` に移した（.NET 10 でも同じ動き） |
| 警告（エラーではない） | SRC#Form の WFO3002（Designer のメンバーの並び、約 100）・WFO2000、SRC#Web・データビューアの BL0016（JS の呼び出しを try/catch で囲む）・BL0012 | 切り替えるときに直すか、理由を書いて抑える |

ほかの Microsoft.Extensions.*・System.* のパッケージは 10 の版のまま .NET 11 で動く。.NET 11 でもユニットテスト・VRT・SRC#Web の E2E は通る。

## サポートの期間

.NET 11 は STS で、STS のサポートは 24 か月になった（[.NET STS releases supported for 24 months](https://devblogs.microsoft.com/dotnet/dotnet-sts-releases-supported-for-24-months/)）。.NET 10（LTS）と同じ 2028 年 11 月に終わる見込み（[.NET のサポートポリシー](https://dotnet.microsoft.com/platform/support/policy/dotnet-core)）。上げても、サポートの期間は延びない。

上げると、SRC#Form を使う人は .NET 11 のデスクトップ ランタイムを入れ直すことになり、VS Code 拡張の `srcsharp` は .NET 11 の SDK が要る。

## 切り替える手順

.NET 11 の正式版が出てから行う（[.NET 11 のリリース](https://github.com/dotnet/core/tree/main/release-notes/11.0)）。

1. `SRC.Sharp/Directory.Build.props` の `SrcSharpTargetFramework` の既定を `net11.0` にする
2. `SRC.Sharp/Directory.Packages.props` の .NET の版（`Microsoft.AspNetCore.Components.WebAssembly*`・`Microsoft.Extensions.*`・`System.*`）を 11 の正式版に、SkiaSharp を Emscripten 6.0.2 の入った正式版にそろえ、`net11.0` の `ItemGroup` を消す
3. ワークフローの `dotnet-version: 10.0.x` を `11.0.x` にする（CI・Packages・Publish-*・CD-*）。`dotnet-next.yml` は次の .NET 用にするか消す
4. `SRC.Sharp/SRCDataLinter/Dockerfile` の土台のイメージを `11.0-noble` にする（Dependabot の対象外。`.github/dependabot.yml`）
5. `tools/pack-analysis`・`tools/pack-stats` のターゲットを上げる
6. 「.NET 10」と書いている所を直す: `README.md`、ヘルプ（動作環境・SRCSharpのインストール・SRCSharpFormの使い方・インストール時のエラーへの対処）、VS Code 拡張（`src/server.ts` の `DOTNET_DOWNLOAD_URL`・`src/settings.ts`・`package.json`・`README.md`）、`docs/logging.md`
7. 上の表の警告を直す
8. SRC#Web の AOT 版の起動時間（`.github/perf-baseline.json`）を見直す
9. リリースの案内に、要るランタイム・SDK が変わることを書く（[リリースの案内](./releases/README.md)）
