**Pilotコマンド**

パイロットを作成

**書式**

**Pilot** *name* *level* [*ID*]

**指定項目説明**

*name*作成するパイロットの名称

愛称も指定可能だが同名のパイロットがいる場合に注意

*rank*パイロットのレベル（1～99）

(Optionコマンドの「レベル限界突破」オプション使用時は1～999)

*ID*パイロットの[グループＩＤ](グループＩＤ.md)（通常は不要）

**解説**

パイロット*name* をレベル*level* で作成します。作成されたパイロットは味方に所属します。パイロットをすぐに出撃させずに後で[**Organize**コマンド](Organizeコマンド.md)や[**Launch**コマンド](Launchコマンド.md)を使って出撃させたいときや、複数のパイロットが乗るユニットのサブパロットやサポートパイロットを作成するときに使用します。

[**Create**コマンド](Createコマンド.md)と同様に*ID* を指定することでザコパイロットに[グループＩＤ](グループＩＤ.md)を付け、区別を可能にすることができます。

作成するパイロットのデータはあらかじめロードしておくことが必要です。イベントファイルの先頭に「@パイロットの所属する作品名」を指定し、データをロードしておいてください。以前のステージですでにデータをロードしている場合には指定は不要です。

**例**

Unit サイキックバスター 0

Pilot ジェイ 10

Ride ジェイ
