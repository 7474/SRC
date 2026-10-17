---
layout: default
title: SRCのインストール
---

# SRCのインストール（SRC のフォルダの用意）

シナリオは、汎用のデータ・グラフィック・効果音を置いた **SRC のフォルダ**（[SRCのルートフォルダ](SRCのルートフォルダ.md)）を前提に作られています。SRC# でも元の SRC と同じフォルダを使います。

## フォルダの場所

| 版 | SRC のフォルダ |
|---|---|
| SRC#Form・SRC#Avalonia | 実行ファイル（`SRCSharpForm.exe`・`SRCSharpAvalonia`）を置いたフォルダ。実行ファイルを SRC のフォルダにコピーして使います |
| SRC#Web | 一覧のシナリオは、必要な素材を自動で重ねます。追加するシナリオは、シナリオと素材をまとめた zip を追加します |

設定ファイル `srcs.json` も、このフォルダに作られます。

## フォルダの中身

```
【SRC のフォルダ】
├ SRCSharpForm.exe など（SRC# の実行ファイル）
├ srcs.json（設定。SRC#Form は初回の起動で、SRC#Avalonia は設定を変えたときに作られます）
├── Bitmap
│     ├── Anime   汎用戦闘アニメ用の画像
│     ├── Event   イベント用の画像
│     └── Map     マップの画像
├── Data
│     └── System
│            ├── alias.txt
│            ├── sp.txt
│            └── terrain.txt
├── Lib
│     ├── 汎用戦闘アニメ
│     ├── スペシャルパワー.eve
│     ├── パイロットステータス表示.eve
│     └── ユニットステータス表示.eve
├── Midi       MIDI ファイル（[MIDIファイルのインストール](MIDIファイルのインストール.md)）
└── Sound      効果音（*.wav）
```

## 素材の入手

- **元の SRC の素材**: [SRC 公式サイト](http://www.src-srpg.jpn.org/) で配布されている、SRC 本体（`Data`・`Lib`・`Midi`）・汎用グラフィック集（`Bitmap`）・汎用効果音集（`Sound`）・汎用戦闘アニメ（`Bitmap`・`Lib`）を展開して、上の形に置きます。元の SRC をすでに入れているなら、そのフォルダをそのまま使えます。
- **SRC# の最小の汎用データ**: 再配布できる自作の素材（地形データ・スペシャルパワーデータ・地形とユニットの画像）を [SRC.Sharp/Content/basic-data](https://github.com/7474/SRC/tree/master/SRC.Sharp/Content/basic-data) に置いています（CC0）。サンプルシナリオ「灯の巡航船ランタン号」が使う分だけで、効果音などはありません。

> **元の SRC との違い**
> - SRC#Avalonia・SRC#Web は、素材が足りなくても止まらずに動きます（無い画像は表示されません）。SRC#Form は元の SRC と同じく、`Bitmap`・`Sound` などが無いとエラーになります。
> - PNG 用の Susie プラグイン（`ifpng.spi`）・MP3 用の `VBMP3.dll`・`FlashControl.ocx` は要りません。
