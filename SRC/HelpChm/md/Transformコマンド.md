**Transformコマンド**

ユニットを変形

**書式**

**Transform** [*unit*] *mode*

**指定項目説明**

*unit*変形させるユニットの[メインパイロット名](メインパイロット名.md)または[ユニットＩＤ](ユニットＩＤ.md)（省略可）

*mode*変形先のユニット名称

**解説**

*unit* を*mode* へと変形させます。

ハイパーモードや変身、能力コピーをイベントで解除するのにも**Transform**コマンドを使います。ハイパーモードや変身、能力コピーを行う前の形態へ**Transform**コマンドで変形させてください。ハイパーモードや変身、能力コピーが解除されます。

**例**

Talk 龍神機

龍神機の本当の力を見せてやるぜ！！

End

#烈の乗っているユニットを真・龍神機に変形させる

Transform 烈 真・龍神機
