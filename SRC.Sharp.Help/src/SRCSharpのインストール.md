---
layout: default
title: SRC#のインストール
---

# SRC#のインストール

SRC# の入手と起動の手順です。動く環境は [動作環境](動作環境.md) を見てください。

シナリオを遊ぶには、元の SRC と同じく、シナリオが求めるデータ（汎用グラフィック集などの素材）を置いた SRC のフォルダが要ります。フォルダの作り方は [SRCのインストール](SRCのインストール.md) と同じです。SRC# は、そのフォルダにある元の SRC のシナリオとデータをそのまま動かします。

## SRC#Web（ブラウザ）

1. https://7474.github.io/SRC/web/ を開きます。
2. 一覧からシナリオを選ぶか、シナリオと素材の zip を追加して始めます。
    - 追加した zip とセーブデータはブラウザに保存され、次に開いたときも使えます。ブラウザのデータを消すと消えるため、大事なセーブデータはファイルに書き出してください。
    - 音はブラウザの決まりにより、画面を最初にクリック（またはキーを押す）してから鳴ります。

## SRC#Avalonia（Windows・macOS・Linux）

1. [Releases](https://github.com/7474/SRC/releases) から、OS に合った zip を取って展開します。.NET を入れる必要はありません。
    - Windows（x64）: `SRCSharpAvalonia-win-x64.zip`（`SRCSharpAvalonia.exe`）
    - Windows（Arm64）: `SRCSharpAvalonia-win-arm64.zip`（`SRCSharpAvalonia.exe`）
    - Linux: `SRCSharpAvalonia-linux-x64.zip`
    - macOS: `SRCSharpAvalonia-osx-arm64.zip`（Apple シリコン）・`SRCSharpAvalonia-osx-x64.zip`（Intel）。署名していないため、初回は Finder で右クリックして「開く」で起動します。
2. 展開した実行ファイルを SRC のフォルダにコピーします。SRC#Avalonia は、実行ファイルを置いたフォルダを SRC のフォルダとして使います。
3. 実行ファイルを起動して、シナリオ（.eve）を選びます（[SRCの起動](SRCの起動.md)）。

MIDI を鳴らすには SoundFont（.sf2）が要ります。次の順に探し、最初に見つかったものを使います。

1. 設定ファイル `srcs.json` の `Option` セクションの `SoundFont`（パス）
2. 環境変数 `SRC_SOUNDFONT`
3. SRC のフォルダ直下、`Midi` フォルダの `*.sf2`
4. Linux の標準の場所（`/usr/share/sounds/sf2/default-GM.sf2` など。Debian・Ubuntu では `fluid-soundfont-gm` や `timgm6mb-soundfont` パッケージで入ります）

## SRC#Form（Windows）

1. [.NET 10 デスクトップ ランタイム](https://dotnet.microsoft.com/download/dotnet/10.0) の x64 版を入れます。
2. [Releases](https://github.com/7474/SRC/releases) から `SRCSharpForm.zip` を取って展開します。
3. 展開した実行ファイル（`SRCSharpForm.exe`）と DLL を、SRC のフォルダにコピーします。
4. `SRCSharpForm.exe` を実行して、シナリオ（.eve）を選びます（[SRCの起動](SRCの起動.md)）。

## シナリオを作る人へ

- データの検査（lint）・シナリオの自動プレイ・バランスの集計には、.NET tool の `srcsharp` を使えます（`dotnet tool install -g SRCSharpTools`）。
- シナリオを VS Code で書くときは、拡張 [SRC](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode)（`koudenpa.src-vscode`）を使えます。イベントの診断・補完・デバッガ・画面での実行ができます。

詳しくは [リポジトリの README](https://github.com/7474/SRC#readme) を見てください。
