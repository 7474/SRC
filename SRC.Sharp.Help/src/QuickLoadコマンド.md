---
layout: default
title: QuickLoadコマンド
---
** 内容はSRC2.2.33のものです **

**QuickLoadコマンド**

クイックセーブ時からプレイを再開

**書式**

**QuickLoad**

**解説**

ゲームオーバー時のコンティニューを実装するために[Systemフォルダ](Systemフォルダ.md)のGameOver.eveから実行することを想定して作られたコマンドです。最後にクイックセーブした時点からプレイを再開します。クイックセーブされていなければ[スタートイベント](スタートイベント.md)開始時、もしくは一時中断セーブデータからのプレイ開始時(一時中断用セーブデータからプレイを再開した場合)からプレイが再開されます。

**QuickLoad**コマンド実行時には[再開イベント](再開イベント.md)が実行されます。

**QuickLoad**コマンドは[プロローグイベント](プロローグイベント.md)、[スタートイベント](スタートイベント.md)、[エピローグイベント](エピローグイベント.md)から実行する事は出来ません。
