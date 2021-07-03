---
layout: default
title: CallIntermissionCommandコマンド
---
** 内容は2.3系のものです **

**CallIntermissionCommandコマンド**

本体に実装されたインターミッションコマンドの機能を呼び出す

**書式**

**CallIntermissionCommand** *command*

**指定項目説明**

*command*	呼び出すインターミッションコマンド

**解説**

本体に実装されたインターミッションコマンドの機能を呼び出します。command には以下のコマンドが指定出来ます。

ユニットの強化
乗り換え
アイテム交換
換装
パイロットステータス
ユニットステータス
データセーブ

**例**
```sh
#簡易版インターミッション
プロローグ:

StartBGM "Intrermission.eve"

command[1] = "ユニットの強化"
command[2] = "乗り換え"
command[3] = "アイテム交換"
command[4] = "換装"
command[5] = "パイロットステータス"
command[6] = "ユニットステータス"
command[7] = "データセーブ"
command[8] = "次のステージへ"
command[9] = "終了"

Do
	Ask command "インターミッション"
	
	If command[選択] = "次のステージへ" Then
		Exec 無限Exec.eve "通常ステージ"
	ElseIf command[選択] = "終了" Then
		Quit
	Else
		CallIntermissionCommand command[選択]
	EndIf
Loop While 1

Exit
```