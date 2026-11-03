---
layout: default
title: SRCとの互換性
---
# SRCとの互換性

SRCとSRC#の互換性について。

## プレイ
### インストール

- VB5 ランタイムライブラリ・PNG 用の Susie プラグイン・MP3 用の `VBMP3.dll` は要りません（[インストールの手順](インストールの手順.md)）。
- SRC のフォルダは、SRC# の実行ファイルを置いたフォルダです。SRC#Avalonia・SRC#Web は素材が足りなくても止まらずに動きます（[SRCのインストール](SRCのインストール.md)）。
- MIDI は、SRC#Form は Windows の MIDI 出力、SRC#Avalonia・SRC#Web は SoundFont で鳴らします（[MIDIファイルのインストール](MIDIファイルのインストール.md)）。MIDI 音源リセット（`MidiReset`）・DirectMusic・`MIDIPortID` の設定は使いません（[設定変更](設定変更.md)）。

### セーブファイル

セーブファイルにSRCとの互換性はありません。拡張子は `.srcs`（セーブ）・`.srcq`（中断）です（[SRCの起動](SRCの起動.md)）。

### 設定ファイル

設定ファイルにSRCとの互換性はありません。設定は SRC のフォルダの `srcs.json` に保存されます。

## データの作成

- ＥＣＭ のレベルがマイナスの場合、能力の説明どおり、半径３マス以内の味方ユニットに対する攻撃の命中率が増加します（元の SRC では増加しませんでした）。
- ユニット・パイロット・非戦闘員データの画像（ビットマップ）と ChangeUnitBitmap コマンドに、`.bmp` のほか `.png`・`.jpg`・`.gif` を指定できます。元の SRC では読み込めないデータになります。
- 画像がアルファチャネルで透過を指定している場合（PNG など）、透過して表示するときは白を透過色にせず、アルファチャネルの透過を使います。

（未実装機能や不具合はあります）

## シナリオの作成
### 文字コード
SRCは内部処理、各種ファイルなど全般で `Shift_JIS` コードでしたが、SRC#では全般に `Unicode` ファイルは `UTF-8` コードです。

ただし、イベントコマンドでファイルを読み書きする場合、デフォルトでは `Shift_JIS` コードで読み書きするように設定されています。

`UTF-8` コードでファイルの読み書きを行う場合はデータを含むシナリオに関するすべてのファイルの文字コードを `UTF-8` とした上で、シナリオのフォルダの設定ファイル `srcs.json`（下記）に `"SRCCompatibilityMode": false` を設定してください。

```json
{
  "SRCCompatibilityMode": false
}
```

`SRCCompatibilityMode` には `false`（読み書きとも `UTF-8`）・`true`（読み書きとも `Shift_JIS`）のほか、`"Read"`（読み込みだけ `Shift_JIS`）・`"Write"`（書き込みだけ `Shift_JIS`）も指定できます。

### シナリオのフォルダの設定ファイル

元の SRC と同じく、シナリオのフォルダに置いた設定ファイルの項目は、SRC# の設定より優先します（[設定変更](設定変更.md)）。SRC# は次の 2 つを読みます。両方にある項目は `srcs.json` を使います。

| ファイル | 形式 |
|---|---|
| `Src.ini` | 元の SRC の設定ファイルの形式です（`Shift_JIS`。`UTF-8` でも読めます）。`[Option]` の `BattleAnimation` などと `[BGM]` を使います |
| `srcs.json` | SRC# の設定ファイル（SRC# のフォルダの `srcs.json`）と同じ形式です。決めたい項目だけを書きます |

```json
{
  "SRCCompatibilityMode": false,
  "BattleAnimation": false,
  "Sections": [
    { "Name": "BGM", "Items": [ { "Name": "Opening", "Value": "はじまり.mid" } ] }
  ]
}
```

- `ExtDataPath`・`ExtDataPath2`（拡張データフォルダ）は、シナリオのフォルダの設定ファイルでは変えられません。
- プレイ中に[設定変更](設定変更.md)や自動反撃モードの切り替えで、シナリオの設定ファイルにある項目を変えると、元の SRC と同じくシナリオの設定ファイルに書き込みます。ほかの項目は SRC# の設定に書き込みます。シナリオの設定ファイルに書き込めない場合（SRC#Web など）は、変えた設定はそのプレイの間だけ効きます。
- SRC#・SRC#Web の設定の画面（プレイの外）で変えるのは SRC# の設定です。シナリオの設定ファイルにある項目は、そのシナリオではシナリオの設定が優先します。

### コマンド

SRCではヘルプから記載が削除されているものの、後方互換のために機能していたコマンドがあります。
それらのうち以下はSRC#では実装されていないため機能しません。

- ShowCharacterコマンド
- ShowImageコマンド
- ClearImageコマンド

また、以下のコマンドはSRC#ではサポートされていません。

- [ClearFlashコマンド](ClearFlashコマンド.md)
- [PlayFlashコマンド](PlayFlashコマンド.md)
