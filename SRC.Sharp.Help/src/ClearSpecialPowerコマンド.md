---
layout: default
title: ClearSpecialPowerコマンド
---
** 内容はSRC2.2.33のものです **

**ClearSpecialPowerコマンド**

ユニットからスペシャルパワーの効果を削除

**書式**

**ClearSpecialPower** [*unit*] *mind*

**指定項目説明**

*unit*スペシャルパワーの効果を削除するユニットの[メインパイロット名](メインパイロット名.md)（省略可）

*mind*効果を削除するスペシャルパワー

**解説**

指定した*unit* からスペシャルパワー*mind* の効果を削除します。指定可能なスペシャルパワーに関しては[**SpecialPower**コマンド](SpecialPowerコマンド.md)を参照してください。

(SRC Ver.1.6までのClearMindコマンドに相当するコマンドです。)

**例**
```sh
ClearSpecialPower ジェイ 必中
```

