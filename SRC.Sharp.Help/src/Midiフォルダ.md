---
layout: default
title: Midiフォルダ
---
**Midiフォルダ**

戦闘BGMや、イベントに用いるためのMIDIファイル及びMP3ファイルを格納するフォルダです。同じ名前（拡張子だけが違う）のMP3ファイルがある場合は、MIDIファイルの代わりにMP3ファイルを演奏します。

データや画像と同様に汎用のものとシナリオ専用のローカルのものに分けて配置することができます。

・シナリオのあるフォルダ\Midi

・ExtDataPathで設定されたフォルダ\Midi

・ExtDataPath2で設定されたフォルダ\Midi

・SRC のフォルダ（SRC# の実行ファイルを置いたフォルダ）\Midi

の順で検索されます。SRC#Avalonia では、SRC のフォルダの `Packs` フォルダに置いた素材パック（zip）の中の Midi フォルダも、SRC のフォルダの Midi フォルダと同じように検索されます（[SRC#Avalonia の使い方](SRCSharpAvaloniaの使い方.md)）。

MIDIファイルの鳴らし方（SRC#Form は Windows の MIDI 出力、SRC#Avalonia・SRC#Web は SoundFont）は [MIDIファイルのインストール](MIDIファイルのインストール.md) を見てください。

それぞれのフォルダでは、ファイル名に「_」を含むファイル（例えば `abc_battle.mid`）なら Midi フォルダの中の「_」より前の名前のサブフォルダ（`Midi\abc`）を先に探し、次に Midi フォルダの直下を探します。同じ名前の MP3 ファイルはそのフォルダの MIDI ファイルより先に探しますが、前のフォルダにある MIDI ファイルは後のフォルダにある MP3 ファイルより優先します。

> **元の SRC との違い**: MP3ファイルを演奏するのに `VBMP3.dll` は要りません。
