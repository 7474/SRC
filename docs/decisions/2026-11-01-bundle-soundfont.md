# SRC#Avalonia に SoundFont（TimGM6mb）を同梱する

- 日付: 2026-11-01
- 状態: 採用
- 関連: [SRC フルパッケージの素材を本リポジトリで再配布しない](./2026-10-12-asset-redistribution.md)（「SoundFont も同梱しない」を、SRC#Avalonia の配布物については改める）

## 背景

SRC#Avalonia は MIDI を SoundFont（.sf2）で合成する（MeltySynth）。利用者が SoundFont を用意しない限り、起動しても BGM が鳴らなかった。

OS の標準の MIDI 音源は使えない。

| OS | 標準の音源 | 使えない理由 |
|---|---|---|
| Windows | Microsoft GS Wavetable Synth（`C:\Windows\System32\drivers\gm.dls`） | 音色が DLS 形式で、MeltySynth は SoundFont 2 だけを読む。OS の MIDI 出力に送る方式は Windows でしか使えず、別の再生経路が要る |
| macOS | Core Audio 内蔵の DLS シンセ（`gs_instruments.dls`） | 同じく DLS。AudioToolbox を直接呼ぶ実装が要る |
| Linux | なし（ディストリビューションのパッケージを入れたときだけ） | — |

SRC#Web は、組み込みのカタログの素材として TimGM6mb を既に配信している（[SRC#Web の音声](../gui/web.md#音声)）。

## 判断

- SRC#Avalonia のビルド・配布物に TimGM6mb を同梱する（実行ファイルの隣の `SoundFont` フォルダ）
- 探す順の最後の候補にする。利用者が指定したもの・SRC のフォルダのもの・OS（Linux）のものがあれば、そちらを使う（[SoundFont](../gui/avalonia.md#soundfont)）
- 取得元は SRC#Web と同じ（`SRC.Sharp/Directory.Build.props`）。ライセンスの全文（`COPYING`）と出典（`README.txt`）を添える

## ライセンス

- TimGM6mb は GPL-2.0（Debian の `timgm6mb-soundfont` も GPL-2）。SRC# 本体（GPL-3.0）とは別のファイルとして同梱し、実行時にデータとして読むだけなので、GPL-2.0 の「単なる集積（mere aggregation）」にあたり、本体のライセンスには影響しない
- GPL-2.0 の条件として、ライセンスの全文と出典を添える。SoundFont のファイル（.sf2）はそのまま編集できる形式で、それ自体がソースにあたる
- 汎用データ（フルパッケージの素材）は従来どおり同梱しない

## 理由

- 起動してすぐ BGM が鳴ることを優先する。サイズは約 6 MB で、配布物（自己完結の単一ファイル）に比べて小さい
- SRC#Web と同じ素材なので、Web とデスクトップで同じ音になる
- OS の音源を使う案（Windows の Microsoft GS Wavetable Synth など）は、元の SRC と同じ音色になる利点があるが、OS ごとの実装が要る。必要になれば、同梱とは別に設定で選べるようにする
