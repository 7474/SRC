**LevelUpコマンド**

パイロットをレベルアップ

**書式**

**LevelUp** [*pilot*] *level*

**指定項目説明**

*pilot*レベルアップするパイロットの名称

省略時にはデフォルトユニットのメインパイロット

*level*レベルの増加量

**解説**

指定したパイロット*pilot* のレベルを*level* 分だけ上げます。ただしパイロットのレベルは最大でも 99 までです。(Optionコマンドの「レベル限界突破」オプション使用時は999まで)

現在のパイロットのレベルはLevel関数を使って参照できます。

レベルを直接上げるのではなく経験値を上げたい場合は[**ExpUp**コマンド](ExpUpコマンド.md)を使ってください。

**例**

#ジェイのレベルを1上げる

LevelUp ジェイ 1
