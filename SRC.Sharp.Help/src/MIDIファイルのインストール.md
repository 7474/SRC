---
layout: default
title: MIDIファイルのインストール
---
** 内容はSRC2.2.33のものです **

**MIDIファイルのインストール**

SRCのプレイ中にＢＧＭを鳴らすにはMIDIファイルが必要です。MIDIファイルが添付されているシナリオもありますが、多くのシナリオではあらかじめMIDIファイルを入手し、インストールしておく必要があります。

ただし、MIDIファイルがなくてもプレイ自体には支障ありません。とりあえずSRCをプレイしてみたいという方はMIDIファイルのインストールを後回しにしていただいても結構です。

MIDIファイルを入手するにはSRC公式ホームページのSRC WebからMIDIを公開しているホームページを訪れてみるといいでしょう。入手したMIDIファイルはMidiフォルダにコピーします。SRCとは関係なくMIDIを公開しているHPからMIDIファイルを入手した場合、ファイル名をSRC用に変更しなければならないので注意して下さい。SRC用ファイル名はSRC向けにMIDIを公開しているホームページやシナリオの注意書きなどを参照すれば分かると思います。

![](./images/bm0.gif)SRCに添付されているMIDIファイルは音源リセット用のものです。

これだけではBGMの演奏はできません。

SRCでは特に設定を行わない限り、DirectMusicによりMIDI音源を自動選択してMIDIファイルの再生を行います。しかし、PCによってはMIDI音源を変更したり、DirectMusicを使わないように設定を変更しないとうまくMIDIファイルを再生できない場合があります。このような場合はマップコマンド「[設定変更](設定変更.md)」を使ってMIDI関連の設定を変更したり、[設定変更](設定変更.md)のページを参照して**Src.ini**ファイルの内容を書き換えて下さい。

MIDIファイルをソフトウェアシンセサイザー使って再生するとシステムにかける負担が大きすぎてSRCの実行を正しく行えないことがあります。SRC実行中にエラーが発生した場合はWindowsのスタートメニューから「コントロールパネル => マルチメディア => MIDI」を選択し、MIDI再生に使用するデバイスを変更して下さい。

なお、一つの曲に複数のMIDIファイルが見つかった場合、MIDIファイル名の「.mid」の前に(2)や(3)といった文字をつけておいておけば、SRCはそれらの中からランダムにファイルを選択して演奏を行います。

**例** 標準のファイル名がHero.midなら Hero(2).mid, Hero(3).mid, …
