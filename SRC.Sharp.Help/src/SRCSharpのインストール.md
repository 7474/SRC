---
layout: default
title: SRC#のインストール
---

# SRC#のインストール

SRC# の入手と起動の手順です。動く環境は [動作環境](動作環境.md) を見てください。

シナリオを遊ぶには、元の SRC と同じく、シナリオが求めるデータ（汎用グラフィック集などの素材）を置いた SRC のフォルダが要ります。フォルダの作り方は [SRCのインストール](SRCのインストール.md) と同じです。SRC# は、そのフォルダにある元の SRC のシナリオとデータをそのまま動かします。

## SRC#Web（ブラウザ）

1. https://src.koudenpa.net/ を開きます。
2. 一覧からシナリオを選ぶか、シナリオと素材の zip を追加して始めます。
    - 追加した zip とセーブデータはブラウザに保存され、次に開いたときも使えます。ブラウザのデータを消すと消えるため、大事なセーブデータはファイルに書き出してください。
    - 音はブラウザの決まりにより、画面を最初にクリック（またはキーを押す）してから鳴ります。

使い方は [SRC#Webの使い方](SRCSharpWebの使い方.md) を見てください。

## SRC#Avalonia（Windows・macOS・Linux）

1. [Releases](https://github.com/7474/SRC/releases) から、OS に合った zip を取って展開します。.NET を入れる必要はありません。
    - Windows（x64）: `SRCSharpAvalonia-win-x64.zip`（`SRCSharpAvalonia.exe`）
    - Windows（Arm64）: `SRCSharpAvalonia-win-arm64.zip`（`SRCSharpAvalonia.exe`）
    - Linux: `SRCSharpAvalonia-linux-x64.zip`
    - macOS: `SRCSharpAvalonia-osx-arm64.zip`（Apple シリコン）・`SRCSharpAvalonia-osx-x64.zip`（Intel）。署名していないため、初回は Finder で右クリックして「開く」で起動します。
2. SRC 本体（汎用グラフィック集などを入れたフォルダ）を使う場合は、次のどちらかにします。SRC#Avalonia は、実行ファイルを置いたフォルダを SRC のフォルダとして使います。
    - zip の中身（実行ファイルと `SoundFont` フォルダ）を、SRC 本体のフォルダにコピーする。
    - 展開したフォルダのまま使い、メニューの「設定」→「設定...」の「拡張データフォルダ 1」に SRC 本体のフォルダを指定する（[SRC#Avaloniaの使い方](SRCSharpAvaloniaの使い方.md)）。
3. 実行ファイルを起動して、シナリオ（.eve）を選びます（[SRCの起動](SRCの起動.md)）。

MIDI を鳴らすには SoundFont（.sf2）が要ります。SRC#Avalonia には SoundFont（TimGM6mb、GPL-2.0）が付いているので、そのまま鳴ります。別の SoundFont を使うときは、次の順に探し、最初に見つかったものを使います。

1. メニューの「設定」の「MIDI の SoundFont」（設定ファイル `srcs.json` の `Option` セクションの `SoundFont`）
2. 環境変数 `SRC_SOUNDFONT`
3. SRC のフォルダ直下、`Midi` フォルダの `*.sf2`
4. Linux の標準の場所（`/usr/share/sounds/sf2/default-GM.sf2` など。Debian・Ubuntu では `fluid-soundfont-gm` や `timgm6mb-soundfont` パッケージで入ります）
5. SRC#Avalonia に付いている SoundFont（実行ファイルと同じ場所の `SoundFont` フォルダ）

## SRC#Form（Windows）

1. [.NET 11 デスクトップ ランタイム](https://dotnet.microsoft.com/download/dotnet/11.0) の x64 版を入れます。
2. [Releases](https://github.com/7474/SRC/releases) から `SRCSharpForm.zip` を取って展開します。
3. 展開した実行ファイル（`SRCSharpForm.exe`）と DLL を、SRC のフォルダにコピーします。
4. `SRCSharpForm.exe` を実行して、シナリオ（.eve）を選びます（[SRCの起動](SRCの起動.md)）。

使い方は [SRC#Formの使い方](SRCSharpFormの使い方.md) を見てください。

## シナリオを作る人へ

- データの検査（lint）・シナリオの自動プレイ・バランスの集計には、.NET tool の `srcsharp` を使えます（`dotnet tool install -g SRCSharpTools`）。
- シナリオを VS Code で書くときは、拡張 [SRC](https://marketplace.visualstudio.com/items?itemName=koudenpa.src-vscode)（`koudenpa.src-vscode`）を使えます。イベントの診断・補完・デバッガ・画面での実行ができます。

詳しくは [リポジトリの README](https://github.com/7474/SRC#readme) を見てください。
