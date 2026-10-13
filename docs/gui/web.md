# SRC#Web（ブラウザ版）

> 更新日: 2026-10-13

ブラウザで動く SRC#。データビューアの `SRCTestBlazor` とは別のプロジェクト `SRCSharpWeb` として作る。

## 方針

- **静的サイトで完結させる**: GitHub Pages などの静的ホスティングに置くだけで動く。サーバ側の処理は持たない
- **永続化はブラウザ内**: シナリオのアーカイブ・セーブデータ・設定は、ブラウザのストレージ（IndexedDB / localStorage）に保存する
- **描画とゲーム処理は SRC#Avalonia と共通**: `SRCRender`（SkiaSharp で画面を描く `ScreenGUI`）と `SRCCore` をそのまま使う

## 構成

```mermaid
flowchart LR
  subgraph Browser[ブラウザ]
    subgraph Main[メインスレッド]
      UI[Blazor の画面<br/>シナリオ選択・アーカイブ追加]
      Canvas[canvas<br/>画面の表示・マウス/キー入力]
      Store[IndexedDB / localStorage<br/>アーカイブ・セーブ・設定]
    end
    subgraph Workers[Web Worker（.NET のスレッド）]
      Game[ゲームスレッド<br/>SRCCore + ScreenGUI]
      Skia[SkiaSharp で描画]
    end
    SW[coi-serviceworker<br/>クロスオリジン分離]
  end
  Static[静的サイト<br/>アプリ・登録済シナリオ・フォント] --> SW --> Main
  UI --> Game
  Game --> Skia -->|フレーム| Canvas
  Canvas -->|入力| Game
  Game <-->|ファイル| Store
```

### スレッド

SRCCore はゲームスレッドで処理を止めて待つ（メッセージ送り・選択待ちなど）作りのため、.NET の WebAssembly マルチスレッド（`WasmEnableThreads`）を使い、ゲームスレッドを Web Worker で動かす。メインスレッドは止めないので、待っている間も画面と入力は動く。

- マルチスレッドには SharedArrayBuffer が必要で、ページがクロスオリジン分離されている（`Cross-Origin-Opener-Policy: same-origin` と `Cross-Origin-Embedder-Policy: require-corp`）必要がある
- GitHub Pages はヘッダを設定できないため、[coi-serviceworker](https://github.com/gzuidhof/coi-serviceworker)（MIT）でヘッダを付ける。初回は Service Worker を有効にするために 1 回再読み込みする。分離が有効になるまで .NET を起動しない
- 確認済み（Chromium、ローカル）: ヘッダ付きの配信と coi-serviceworker のどちらでも、裏のスレッドが `Thread.Sleep` で待つ間もメインスレッドの画面が更新される。SkiaSharp（`SkiaSharp.NativeAssets.WebAssembly` のマルチスレッド版）で裏のスレッドから描画できる
- ビルドには `wasm-tools` ワークロードが必要（SkiaSharp のネイティブライブラリのリンクのため）

### ファイル

- SRCCore の `LocalFileSystem` はフォルダと zip アーカイブを重ねて扱える（`AddPath` / `AddAchive`）。ブラウザでは .NET のメモリ上のファイルシステムに zip を置いて使う
- 重ねる順: シナリオのアーカイブ → 共通素材のアーカイブ（汎用グラフィック集など、利用者が追加した場合） → なし（素材なしでも動く。`SRC.AllowMissingAssets`）

### シナリオ

| 種類 | 置き場所 | 内容 |
|---|---|---|
| 登録済シナリオ | 静的サイト `scenarios/catalog.json` と zip | 再配布できるシナリオ（E2E のコーパスなど）。一覧から選んでプレイする |
| 追加したアーカイブ | IndexedDB | 利用者が zip を選んで追加する。シナリオの zip と共通素材の zip を追加できる。ブラウザに残り、次回も選べる |

### 永続化

| データ | 保存先 | 備考 |
|---|---|---|
| アーカイブ（zip） | IndexedDB | 大きいため IndexedDB に Blob で保存する |
| セーブデータ（通常・中断・クイック） | IndexedDB | シナリオごとに保存する。ファイルとして書き出す・読み込む操作も用意する |
| 設定（`srcs.json` 相当） | localStorage | |

ブラウザのストレージは利用者やブラウザの判断で消えることがあるため、セーブデータはファイルとして書き出せるようにする。

### フォント

ブラウザには日本語フォントが無い前提で、OFL のフォントを静的サイトに置いて読み込む。等幅の指定（ＭＳ ゴシック・ＭＳ 明朝）には等幅のフォントを使う（空白で桁を揃える表示のため）。縦の寸法は `WindowsFontMetrics` で元の SRC に合わせる。

### 音声

Web Audio API で鳴らす（`WebSoundPlayer`）。

| 形式 | 方法 |
|---|---|
| WAV・MP3 | ブラウザで復号する（`decodeAudioData`） |
| MIDI | SRC#Avalonia と同じく [MeltySynth](https://github.com/sinshu/meltysynth)（MIT）で合成する。別のスレッドで少し先まで合成し、描画の機会ごとに約 1 秒先まで PCM を送る |

- ブラウザは利用者の操作があるまで音を鳴らさないため、画面の最初のクリック・キー入力で開始する（[Autoplay policy](https://developer.chrome.com/blog/autoplay)）
- SoundFont（.sf2）は同梱しない。追加したアーカイブに含まれる .sf2 を使う。無ければ MIDI は鳴らさない
- 和音 3 つの短い MIDI で、合成が実時間に間に合うことを確認した。重い曲で間に合わない場合は AOT コンパイル（`RunAOTCompilation`）を検討する

### 文字入力・ファイル選択

`IScreenDialogHost` をページ上のモーダルで実装する（`WebScreenHost`）。JS を呼べるのは UI 側のスレッドだけのため、ゲームスレッドからの依頼は画面の書き込みと同じ機会（requestAnimationFrame）に始め、結果をゲームスレッドへ返す。ファイル選択はシナリオのフォルダの中に限る。

### セーブデータ

ゲームはシナリオのフォルダ（.NET のメモリ上のファイルシステム）にセーブデータ（.srcs・.srcq）を書く。プレイ中は 1 秒ごとに変化を調べ、書き込まれたものを IndexedDB に保存する（`SaveSync`）。シナリオを開くときに保存しておいたものをフォルダに戻し、「セーブデータから再開」に並べる。書き出し（ダウンロード）・読み込み・削除もここで行う。

## 段階

| 段階 | 内容 | 状況 |
|---|---|---|
| 1 | 登録済シナリオを選んでプレイできる（メッセージ・選択肢・マップ・戦闘） | 済 |
| 2 | zip のアーカイブを追加してプレイできる（ブラウザに保存し、次回も選べる。Shift_JIS のファイル名） | 済 |
| 3 | セーブ・ロード（ブラウザ内保存、ファイルの書き出し・読み込み）、文字入力 | 済 |
| 4 | 音声（WAV・MP3・MIDI） | 済 |
| 5 | GitHub Pages への配置（CI） | 未 |

## テスト

`SRC.Sharp/SRCSharpWeb.E2E`（Playwright）。発行した静的ファイルをヘッダなしで配信し（coi-serviceworker でクロスオリジン分離）、Chromium で次を確かめる。CI の Linux ジョブで実行する。

- 登録済シナリオを選んでプレイする
- アーカイブ（日本語のファイル名）を追加し、再読み込み後も残っていてプレイできる。削除できる
- インターミッションでデータセーブし、再読み込み後にセーブデータから再開して次のステージへ進む
- 効果音（WAV）が鳴る。SoundFont が無ければ MIDI は鳴らさずに進む
- アーカイブに含めた SoundFont で MIDI を合成して鳴らす（テストの中で zip を作る。SoundFont は CI で入れる TimGM6mb）

画面の状態は `WebInterop.DebugState`（実行中のイベントの行・リスト・ダイアログ・メッセージの JSON）で調べる。

```sh
dotnet workload install wasm-tools
dotnet publish SRC.Sharp/SRCSharpWeb -c Release -o SRC.Sharp/SRCSharpWeb/bin/web
cd SRC.Sharp/SRCSharpWeb.E2E && npm ci && npx playwright install chromium && npx playwright test
```
