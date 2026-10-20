# Contributing to SRC#

適当にフォークして変更、プルリクエストしてください。

- 変更部分はローカルマシンでざっと動作確認お願いします。
- 可能なら @7474 が分かりやすい単位でお願いします。
- 作業が被ると哀しいので、IssueやTwitterでこの辺やりますと事前に声掛けしていただけるとよいと思います。
- プルリクエストは CI（必須チェック `ci-passed`）が通るとマージできます。どのテストを足せばよいかは [テストと CI の設計](./docs/testing.md) を参照してください。

SRCSharpFormに関して、当面はSRC2.2の機能を動作させることを目標としています。
2.3以降（特に新GUI）の機能や、新規の機能はリジェクトするかもしれません。
その辺りは特に事前に声掛けしていただけるとよいと思います。

## ドキュメント

- `docs/` の各ドキュメントには現状だけを書きます。経緯や判断の理由は [判断の記録](./docs/decisions/README.md) に、過去の計画・調査は [履歴](./docs/history/README.md) に分けます。
- 一覧は [docs/README.md](./docs/README.md) にあります。

## 免責

本ドキュメント内容は予告なく変更される場合があります。

## コードの規約

SRC# は元の SRC（VB6。`SRC/SRC_20121125`）を C# に移植したものです（移植は完了しています。[移植の記録](./docs/history/porting/README.md)）。コードには移植の名残があり、次の規約で書いています。

- 元の SRC の動作を仕様とする。動作を確かめるときは `SRC/SRC_20121125` の該当するコードと照らし合わせる
- VB のモジュール（static な空間）は、C# ではインスタンスにする
- 実行環境に依存する処理（画面・入力・音声・ファイルの選択など）は `SRCCore` に入れず、GUI のプロジェクト（`SRCRender`・`SRCSharpForm` など）に実装する。`SRCCore` からは `IGUI` などのインタフェースを通して呼ぶ
- VB の `Integer`（16 ビット）は `int` にする
- 座標は 15 Twip = 1 ピクセルで換算する
- VB で Double を Integer・Long に代入する箇所は銀行丸め（偶数丸め）になる。C# の `(int)` キャスト（切り捨て）ではなく、`SRCCore/VB/VBMath.cs`・`SRCCore/VB/Conversions.cs` の関数（`VBMath.CLng` など）を使う

元の SRC と動作が違う箇所を見つけたら、元の SRC の動作を期待値にしたテストを `SRC.Sharp/SRCCoreTests` に書いて直します。すぐに直せない場合は、そのテストに `[Ignore("移植の不具合: <概要>")]` を付けて残します（`grep -rn 'Ignore("移植の不具合' SRC.Sharp/SRCCoreTests` で一覧できます）。
