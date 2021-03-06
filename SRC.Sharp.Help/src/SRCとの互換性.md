---
layout: default
title: SRCとの互換性
---
# SRCとの互換性

SRCとSRC#の互換性について。

## プレイ
### セーブファイル

セーブファイルにSRCとの互換性はありません。

### 設定ファイル

設定ファイルにSRCとの互換性はありません。

## データの作成

既知の非互換機能はありません。

（未実装機能や不具合はあります）

## シナリオの作成
### 文字コード
SRCは内部処理、各種ファイルなど全般で `Shift_JIS` コードでしたが、SRC#では全般に `Unicode` ファイルは `UTF-8` コードです。

ただし、イベントコマンドでファイルを読み書きする場合、デフォルトでは `Shift_JIS` コードで読み書きするように設定されています。

`UTF-8` コードでファイルの読み書きを行う場合はデータを含むシナリオに関するすべてのファイルの文字コードを `UTF-8` とした上で、シナリオフォルダに含まれる設定ファイルに `"SRCCompatibilityMode": false` を設定してください。

```json
{
  "SRCCompatibilityMode": false
}
```

TODO シナリオフォルダ配下の設定ファイル読み込みを実装する（つまり今は未実装）。

### コマンド

SRCではヘルプから記載が削除されているものの、後方互換のために機能していたコマンドがあります。
それらのうち以下はSRC#では実装されていないため機能しません。

- ShowCharacterコマンド
- ShowImageコマンド
- ClearImageコマンド

また、以下のコマンドはSRC#ではサポートされていません。

- [ClearFlashコマンド](ClearFlashコマンド.md)
- [PlayFlashコマンド](PlayFlashコマンド.md)
