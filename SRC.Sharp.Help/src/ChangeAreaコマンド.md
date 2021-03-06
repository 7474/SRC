---
layout: default
title: ChangeAreaコマンド
---
** 内容はSRC2.2.33のものです **

**ChangeAreaコマンド**

ユニットの存在するエリアを変更

**書式**

**ChangeArea** [*unit*] *area*

**指定項目説明**

*unit*エリアを変更するユニットの[メインパイロット名](メインパイロット名.md)または[ユニットＩＤ](ユニットＩＤ.md)（省略可）

*area*移動先エリア

**解説**

*unit* を指定したエリア*area* へ移動させます。エリアには

**地上**

**空中**

**水中**

**水上**

**地中**

**宇宙**

が指定可能です。

コンピューターが操作するＮＰＣ、敵、中立に属するユニットは地形適応がもっとも高いエリアを使用します。地形適応が同じ場合には空中を優先します。また地中移動が可能でも地中を使用することはありません。コンピューターの選んだエリアを強制的に変更したい場合には**ChangeArea**コマンドを使ってください。

![](./images/bm0.gif)ユニットが本来活動できない地形にユニットを移動させた場合、動作は保証されません。

アイテムをユニットに装備させるなどして地形適応を変更して下さい。

**例**
```sh
ChangeArea もぐら兵 地中
```

