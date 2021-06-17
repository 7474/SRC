---
layout: default
title: Doコマンド
---
** 内容はSRC2.2.33のものです **

**Doコマンド**

指定した条件が真の間、一連のイベントコマンドを繰り返し実行

**書式１**

**Do While** *expression*

*commands*

**Loop**

**書式２**

**Do**

*commands*

**Loop While** *expression*

**指定項目説明**

*expression*条件式

*commands*実行するイベントコマンド。複数行指定可能

**解説**

条件式 *expression* の値が 0 でない間、**Do**行から**Loop**行までのイベントコマンド列 *commands* を繰り返し実行します。式の値の評価は**While**行を実行するたびに行われます。[**Skip**コマンド](Skipコマンド.md)や[**Break**コマンド](Breakコマンド.md)を使えば実行の流れを変えることができます。

![](./images/bm0.gif) イベントファイルが見やすいように*commands* にはインデントをつけるようにして下さい。

**例**
```sh
Talk コーネリアス
よし！ ドール隊１２機、全機発進！
End
#コーネリアスの乗るユニットの周りに12機のドールを出撃させる
#(説明用にDoコマンドを使ってますが、実はForコマンドのほうが簡単…)
i = 0
Do While (i &lt; 12)
Incr i
Create 敵 ドール 0 一般兵士 18 X(コーネリアス) Y(コーネリアス)
Loop
```

