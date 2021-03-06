---
layout: default
title: LineReadコマンド
---
** 内容はSRC2.2.33のものです **

**LineReadコマンド**

ファイルから文字列を読みだす

**書式**

**LineRead ***handle var*

**指定項目説明**

*handle*ファイルハンドル

*var*読み出した文字列を格納する変数

**解説**

[**Open**コマンド](Openコマンド.md)で開いたファイルから一行分の文字列を読み出し、変数*var*に格納します。どのファイルから読みこむかを指定するため、**Open**コマンドで得たファイルハンドルを*handle*に指定します。

ファイルの長さを超えてファイルを読み出すことはできません。ファイルの最後まで来たかどうかはEOF関数を使って確認してください。

ファイルからデータを読み出した後は[**Close**コマンド](Closeコマンド.md)を使い、ファイルを閉じてください。

**例**
```sh
#主人公に関する設定をセーブするサブルーチン「セーブパーソナルデータ」
セーブパーソナルデータ:
#ファイルハンドル用変数
Local F
#パーソナルデータ.txtを出力モードで開く
Open パーソナルデータ.txt For 出力 As F
#各変数の値をファイルに書き込み
Print F 主人公苗字
Print F 主人公名前
Print F 主人公愛称
#ファイルを閉じる
Close F
Talk システム
パーソナルデータをパーソナルデータ.txtにセーブしました
End
#サブルーチンを終了
Return
#主人公に関する設定をロードするサブルーチン「ロードパーソナルデータ」
ロードパーソナルデータ:
#ファイルハンドル用変数
Local F
#パーソナルデータ.txtを入力モードで開く
Open パーソナルデータ.txt For 入力 As F
#各変数の値をファイルから読み込み
LineRead F 主人公苗字
LineRead F 主人公名前
LineRead F 主人公愛称
#ファイルを閉じる
Close F
Talk システム
パーソナルデータをパーソナルデータ.txtからロードしました
End
#サブルーチンを終了
Return
```

