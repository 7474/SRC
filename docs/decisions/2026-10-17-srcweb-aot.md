# SRC#Web の発行を既定で AOT コンパイルにする

- 日付: 2026-10-17
- 状態: 採用

## 背景

SRC#Web はインタープリターで発行していて、ランタン号を選んでから最初の行を実行するまでに 4.2〜4.7 秒かかっていた。大文字・小文字を区別しないパスの解決（`PathResolver`）でフォルダの一覧をキャッシュして約 3.4 秒になったが、まだ待ち時間が長かった。

## 判断

- 発行（publish）は既定で AOT コンパイルする（`RunAOTCompilation`、[ASP.NET Core Blazor WebAssembly build tools and ahead-of-time (AOT) compilation](https://learn.microsoft.com/aspnet/core/blazor/webassembly-build-tools-and-aot)）。`-p:RunAOTCompilation=false` で切れる
- CI の PR（`test-web`、SWA のプレビュー）は待ち時間を減らすため AOT を切る。master への push の `test-web`、GitHub Pages・SWA の本番、リリースは AOT にする

## 理由

ランタン号を選んでから最初の行を実行するまで（ローカル・Chromium、3 回）:

| 版 | 時間 | ダウンロード（`_framework` の brotli） | 発行の時間 |
|---|---|---|---|
| インタープリター | 4.2〜4.7 秒 | 5.8 MB | — |
| ＋ パスの解決のキャッシュ | 約 3.4 秒 | 5.8 MB | — |
| ＋ AOT | 約 2.2 秒 | 11.0 MB | 約 8 分長い |

- 起動までの時間が約 3.4 秒から約 2.2 秒になる。ダウンロードは約 2 倍になるが、カタログのアーカイブ（シナリオ・素材）に比べれば小さい
- 発行が 8 分ほど長くなるため、PR ではインタープリターにして CI の待ちを減らし、AOT 固有の問題は master への push の `test-web` で見つける

## 影響

- 自分でビルドする場合も既定で数分かかる
- PR の CI とプレビュー環境は、公開するものと発行の方法が違う

## 参照

- [SRC#Web の速さ](../gui/web.md#速さ)
