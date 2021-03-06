---
layout: default
title: RenameBGMコマンド
---
** 内容はSRC2.2.33のものです **

**RenameBGMコマンド**

BGMに使用されるMIDIファイル名を変更

**書式**

**RenameBGM** *bgm file*

**指定項目説明**

*bgm*ファイル名を変更するBGM

*file*使用するMIDIファイル名

**解説**

SRCでは起動時や味方フェイズ開始時など、いくつかの場面で自動的にMIDIファイルがBGMとして再生されます。この際に使用されるMIDIファイルのファイル名をRenameBGMコマンドで変更することが出来ます。

*bgm* に指定可能なBGMのシチュエーションは以下のものがあります。

**Map1**味方フェイズ開始時

**Map2**敵フェイズ開始時

**Map3**屋内マップでの味方フェイズ開始時

**Map4**屋内マップでの敵フェイズ開始時

**Map5**宇宙マップでの味方フェイズ開始時

**Map6**宇宙マップでの敵フェイズ開始時

**Briefing プロローグ・エピローグ開始時**

**Intermissionインターミッション時**

**SubtitleTelopコマンドによるテロップ表示時**

**Endゲームオーバー時**

**default戦闘時にパイロットデータに設定されたMIDIファイルが見つからない際**

**RenameBGMコマンドでSRC起動時に再生されるOpening.midを別のファイルに変更することは出来ません。これはRenameBGMコマンドによる設定が読み込まれる前にOpening.midが再生されるためです。SRC起動時のBGMを別のファイルに変更したい場合はシナリオのフォルダ内に[Src.ini](設定変更.md)を作り、その中で[BGM]のOpeningの項目のみを設定して下さい。**

**RenameBGMコマンドによる設定はセーブデータに記録され、RenameBGMコマンドを実行した後のステージにも継続して使用されます。**

**例**
```sh
```

**RenameBGM Intermission intermission2.mid**
