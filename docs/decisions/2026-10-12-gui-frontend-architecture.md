# GUI を SRCRender と新しいフロントエンド（Avalonia・Web）に移し、WinForms 版は残す

- 日付: 2026-10-12
- 状態: 採用

## 背景

GUI の実装は Windows Forms の SRC#Form（`SRCSharpForm`）だけで、Windows でしか動かなかった。

- `SRCCore` が `System.Drawing.Common` を参照していた。.NET 6 以降は Windows 専用で、`IGUI.CurrentPaintFont` の `Font` 型などは Windows 以外では実行時例外になる（[MS Learn](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only)）
- `IGUI`（約 150 メンバー）は `ListBox()` が結果を返すようなモーダルの同期 API で、`Sleep`・`DoEvents` は `Application.DoEvents()` に頼っていた。Avalonia・Uno・Blazor には `DoEvents` に当たるものが無い
- 描画は GDI+ の直描き、音声は NAudio（出力は Windows 依存）だった。WinForms の実装は約 28,800 行（うち参照用の `GUI.ref.cs` が 11,640 行）

## 判断

- `SRCCore` から `System.Drawing.Common` を外し、`IGUI` の境界は同期のまま残す
- シナリオをゲームスレッドで実行し、待ちはゲームスレッドで行う。UI からの入力はゲームスレッドへ渡す
- 画面全体を SkiaSharp で 1 枚に描く `SRCRender`（GUI の実装 `ScreenGUI` を含む）を作り、フロントエンドから共用する。表示と操作は WinForms 版に合わせる
- デスクトップのフロントエンドは Avalonia（SRC#Avalonia）。ブラウザ版（SRC#Web）は `IGUI` を非同期にせず、.NET の WebAssembly マルチスレッドでゲームスレッドを Web Worker で動かす
- 音声は OpenAL（OpenAL Soft）で鳴らす `SRCSound` を作る
- SRC#Form は残し、`IGUI` の実装として並べて動かす

## 理由

候補の比較（2026-10-12 時点）:

| 候補 | 対応 OS | SRC との相性 | 主なリスク |
|---|---|---|---|
| **Avalonia 12** | Windows / macOS / Linux / WASM | カスタム描画で GDI+ の描画を置き換えやすい。`Avalonia.Headless` で UI テストを CI で動かせる。MIT | `DoEvents` が無い。Linux の Wayland 対応は private preview（[Avalonia 12](https://avaloniaui.net/blog/avalonia-12)） |
| Uno Platform 6.6 | Windows / macOS / Linux / WASM / モバイル | Skia の描画に統一 | WinUI / XAML の流儀で学ぶことが多い。モバイル対応は過剰（[Uno 6.6](https://infoq.com/news/2026/08/uno-platform-6-6)） |
| MonoGame 3.8.5 | Windows / macOS / Linux | マップ描画・アニメーションは得意 | リスト・入力・設定画面などの UI 部品を自作する必要がある（[MonoGame 3.8.5](https://gamefromscratch.com/monogame-3-8-5-released/)） |
| Blazor WASM（`SRCTestBlazor` を拡張） | ブラウザ | 配布が要らない | メインスレッドを止められないため `IGUI` の非同期化が前提。`async` が Core 全体に広がる |
| .NET MAUI 10 / Blazor Hybrid | Windows / macOS | — | 公式は Linux 非対応（[The Register](https://theregister.com/2026/03/23/maui_linux_avalonia)） |
| WPF / WinUI 3 | Windows のみ | — | WinForms からの改善が小さく、Windows 専用のまま |

- 移行の費用の大半は、どのフレームワークでも必要な `SRCCore` の整理（`System.Drawing.Common` の除去と実行モデルの変更）だった。ゲームスレッド方式なら `IGUI` のシグネチャを変えずに済み、Core の変更が最小になる
- 描画をフロントエンドに依存しないライブラリにすると、Avalonia とブラウザ（canvas への転送）で共用でき、VRT もヘッドレスで撮れる
- WinForms 版は新しいフロントエンドの機能が揃うまでの比較対象・利用者の移行先として残す

## 影響

- `SRCCore` は netstandard2.1 のまま Windows 以外で動き、CI の Linux ジョブでテストする
- SRC#Form は既定では UI スレッドで実行し、`--game-thread` でゲームスレッド方式を試せる
- フロントエンドの UI テストは表示と入力の経路だけにし、挙動と見た目は `SRCRender` のヘッドレスなテストで確かめる（[ヘッドレスな E2E・VRT](./2026-10-12-headless-e2e-vrt.md)）

## 参照

- [GUI の構成](../gui/README.md)・[SRC#Avalonia](../gui/avalonia.md)・[SRC#Web](../gui/web.md)
- [GUI 実装移行の検討（記録）](../history/gui/gui-migration-study.md)
