---
layout: default
title: SRC#Formの使い方
---

# SRC#Formの使い方

対応: SRC#Form（Windows）

SRC#Form は、元の SRC の画面を Windows のフォームでそのまま再現した SRC# です。入手は [SRC#のインストール](SRCSharpのインストール.md) を見てください。新しい機能は [SRC#Avalonia](SRCSharpAvaloniaの使い方.md) と [SRC#Web](SRCSharpWebの使い方.md) から先に入ります。

## 起動

`SRCSharpForm.exe` を起動すると、最初の画面が出ます。

| ボタン | 動作 |
|---|---|
| ファイル選択 | シナリオ（`.eve`）・セーブデータ（`.srcs`・`.srcq`）を選んで始めます |
| 設定 | [設定変更](設定変更.md) の画面を開きます |

ファイルを `SRCSharpForm.exe` にドラッグ＆ドロップするか、コマンドラインの引数に渡すと、選ぶ画面を経ずに始まります（[SRCの起動](SRCの起動.md)）。

## 設定

ゲームの設定（戦闘アニメ・マス目・音量など）は、最初の画面の「設定」か、ゲームの中のマップコマンド「設定変更」で変えます。設定は `SRCSharpForm.exe` のあるフォルダの `srcs.json` に保存されます。

画面で変えられない設定は、`srcs.json` を直接書きます。

| 項目 | 内容 |
|---|---|
| `ExtDataPath`・`ExtDataPath2` | 拡張データフォルダ。SRC# のフォルダとシナリオのフォルダに無いデータ・グラフィック・音楽を探すフォルダです。元の SRC の `Src.ini` の `ExtDataPath`・`ExtDataPath2` にあたります |
| `Log` セクション | ログの細かさ・出す場所（[ログ](ログ.md)） |

## ログ

ログは `%LOCALAPPDATA%\SRCSharp\logs` に出ます。不具合を報告するときは、`SRCSharpForm.exe -v` で起動して詳しいログを出してください（[ログ](ログ.md)）。

## ほかの SRC# との違い

- 動くのは Windows（x64）だけです。[.NET 11 デスクトップ ランタイム](https://dotnet.microsoft.com/download/dotnet/11.0) が要ります。
- 素材（汎用グラフィック集など）が無いと動きません。SRC#Avalonia・SRC#Web は素材が無くても動きます。
- 素材パック（zip）・表示倍率・メニューからの設定は、SRC#Avalonia にだけあります。
