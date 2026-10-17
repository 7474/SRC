---
layout: default
title: MIDIファイルのインストール
---

# MIDIファイルのインストール

プレイ中に BGM を鳴らすには MIDI ファイルが要ります。MIDI ファイルが添付されているシナリオもありますが、多くのシナリオでは、あらかじめ MIDI ファイルを入手して SRC のフォルダの `Midi` フォルダに置いておく必要があります。MIDI ファイルが無くても遊ぶことはできます。

SRC 用に MIDI を公開しているサイトから入手してください。SRC と関係なく公開されている MIDI ファイルを使う場合は、ファイル名を SRC 用の名前に変える必要があります。SRC 用のファイル名は、SRC 向けに MIDI を公開しているサイトやシナリオの注意書きなどで確かめてください。

![](./images/bm0.gif)元の SRC に添付されている MIDI ファイルは音源リセット用のものです。これだけでは BGM は鳴りません。

一つの曲に複数の MIDI ファイルがある場合、ファイル名の「.mid」の前に (2) や (3) を付けて置いておくと、その中からランダムに選んで演奏します。

**例** 標準のファイル名が Hero.mid なら Hero(2).mid, Hero(3).mid, …

## 版ごとの鳴らし方

| 版 | MIDI の鳴らし方 |
|---|---|
| SRC#Form | Windows の MIDI 出力（最初に見つかった出力デバイス）で鳴らします |
| SRC#Avalonia | SoundFont（.sf2）で鳴らします。探す順番は [SRC#のインストール](SRCSharpのインストール.md) を見てください |
| SRC#Web | シナリオ・素材の zip に含まれる SoundFont（.sf2）で鳴らします。SoundFont が無ければ MIDI は鳴りません。一覧の「灯の巡航船ランタン号」は、SoundFont（TimGM6mb）を自動で重ねて鳴らします |

> **元の SRC との違い**: DirectMusic や MIDI 音源の選択、`Src.ini` の MIDI 関連の設定は使いません。設定は SRC のフォルダの `srcs.json` に保存されます。
