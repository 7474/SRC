**RecoverENコマンド**

ＥＮを回復

**書式**

**RecoverEN** [*unit*] *rate*

**指定項目説明**

*unit*ＥＮを回復するユニットの[メインパイロット名](メインパイロット名.md)または[ユニットＩＤ](ユニットＩＤ.md)（省略可）

*rate*ＥＮの回復率（浮動小数点数も指定可）

**解説**

*unit* のＥＮを*rate* ％だけ回復します。マイナスの値を指定してＥＮを減少させることもできます。

![](../images/bm0.gif) ＥＮの増減の度合いを割合でなく一定値で指定したい場合はEN関数を使ってください

**RecoverEN**コマンドによってＥＮが最大ＥＮを越えたり、0未満になることはありません。ＥＮが0になったユニットは行動不能に陥るので注意してください([**Option**コマンド](Optionコマンド.md)で「ＥＮ０時行動可」オプションを設定している場合は例外)。

**例**

Talk ファントム

クッ、クッ、クッ、召喚獣ボイドよ！ 奴らの力を奪い取ってしまえ！！

End

#味方ユニット全てのＥＮを0にする

ForEach 味方

RecoverEN -100

Next
