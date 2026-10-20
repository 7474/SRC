# リリースのタグをアプリの版にし、VS Code 拡張の版をライブラリと合わせる

- 日付: 2026-10-16
- 状態: 採用

## 背景

- `v0.5.0` までは、リリースのタグをライブラリの版（`0.x.x`）で打っていた。SRCSharpTools（`srcsharp`）と SRC#Web のパッケージは `v0.5.0` の後に加えたもので、`v3.5.1` から公開している
- VS Code 拡張の旧版（0.0.5）は 7474/SRC-VSCode から出していた（7474/SRC#1175）。このリポジトリに移した 0.5.0 はプレリリースで出したが、プレリリースのままでは既存の利用者（0.0.5）の版が切り替わらない
- 拡張の版は SRC# 本体とは別に上げていて、どの srcsharp に対応する拡張かが版から分からなかった

## 判断

- リリースのタグはアプリの版（`v3.x.x`）で打つ
- VS Code 拡張は 0.5.1（`v3.5.2` のタグで公開）から正式版で出す。版のマイナーが 7 以上の奇数ならプレリリースにする
- 拡張の版は 0.5.2 からライブラリ（srcsharp）と同じにし、CI で `Directory.Build.props` と同じかを確かめる

## 理由

- 利用者が見るのはアプリの版で、1 つのタグでアプリ・ライブラリ・拡張・Docker イメージをまとめて出せる
- 正式版にしないと、既存の利用者に新しい拡張が届かない
- 拡張と srcsharp の版が同じなら、組み合わせを確かめやすい

## 影響

- 拡張を出すときは `package.json` の `version` と `CHANGELOG.md` を上げてからタグを打つ。公開済みの版なら `--skip-duplicate` で飛ばす

## 参照

- [バージョンの管理](../versioning.md)
- 7474/SRC#1175
- [Pre-release extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension#prerelease-extensions)
