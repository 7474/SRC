# SRC#Help

SRC#（Simulation RPG Construction Sharp）のヘルプ。

## 閲覧

https://srch.7474.jp/

## 編集方針

### 位置づけ

- **SRC# のヘルプ**とする。元の SRC（VB 版）のヘルプを土台にしているが、SRC# で遊ぶ人・データやシナリオを作る人に向けて書く。
- 開発者向けの内容（設計・実装・ビルド・リリース）は [docs](../docs/README.md) に書き、ヘルプには書かない。
- 呼び方
  - **SRC#**: このソフトウェア（各アプリ・ツールの総称）
  - **元の SRC**: VB 版の SRC（Ver.2.2.33 まで）
  - データ・イベント・マップの書式は「SRC の形式」と呼んでよい（SRC# と元の SRC で共通のため）
- **実装を正とする**。ヘルプと SRC# の動作が食い違うときは、ヘルプを実装に合わせて直す（7474/SRC#1092・7474/SRC#612）。実装を元の SRC に合わせて直すべきものは Issue にする。

### 対象のアプリ

すべて含める。

| 対象 | ヘルプでの扱い |
|---|---|
| SRC#Form（Windows）・SRC#Avalonia（Windows・macOS・Linux）・SRC#Web（ブラウザ） | 入手・起動・操作・設定を書く |
| 周辺ツール（`srcsharp`・VS Code 拡張・SRCDataLinter・データビューア） | 存在と概要（何ができるか・どこで入手するか）だけを書き、詳細は docs（[作成支援ツール](../docs/tools/authoring.md)、[VS Code 拡張](../SRC.Sharp/SRCSharpVSCode/README.md) など）へリンクする |

一部のアプリにしか無い機能・アプリで違いがある機能は、ページに対応するアプリを書く（例: 「対応: SRC#Web」）。

### ページの種類と扱い

| 種類 | 例 | 扱い |
|---|---|---|
| A. SRC# 固有 | 動作環境・インストール・起動・画面・設定 | SRC# 向けに書き直す |
| B. 言語仕様 | データ・特殊能力・武器属性・イベントコマンド・関数・式 | 残す。SRC# の動作として書き、実装と照らし合わせて確かめる |
| C. 歴史的なページ | 更新履歴（1997〜2010）・スペシャルサンクス・使用条件 | 元の SRC の資料として残す（メニューの「元の SRC について」にまとめる） |
| D. SRC# に当てはまらないもの | VB5 ランタイム・Flash・元の SRC の更新インストール | 「SRC# では不要」と注記して C に移す |

進める順番は A → B。

### 元の SRC との違いの書き方

注記とまとめの両方に書く。

- **注記**: そのページの該当箇所に書く。

  ```markdown
  > **元の SRC との違い**: 〜。（7474/SRC#番号）
  ```

- **まとめ**: [SRCとの互換性](src/SRCとの互換性.md) の該当する節（データの作成・シナリオの作成など）に 1 行で書き、注記のあるページへリンクする。

### 確認の状態

元の SRC から移したままのページは、本文の先頭に「内容はSRC2.2.33のものです」と書いてある。SRC# の動作と照らし合わせて直したページは、この一文を消す。

元の SRC の資料として残すページ（C・D の種類。更新履歴・スペシャルサンクスなど）は、この一文を残し、元の SRC の資料であることを先頭の注記に書く（7474/SRC#1446）。

### 文体

- です・ます調。
- 能力名・コマンド名など、データやイベントに書く名前は元の SRC の表記（全角の英数字など）のままにする。

## srcsharp への同梱

`src` の `*.md` は CLI `srcsharp` に埋め込まれ、`srcsharp help <項目>`・`--search`・`--export` で引ける（[作成支援ツール](../docs/tools/authoring.md#cli)）。項目の名前はファイル名なので、ファイル名を変えると `srcsharp help` で引く名前も変わる。

## Develop

```sh
bundle install
jekyll serve --incremental
```

## Theme

https://github.com/pmarsceill/just-the-docs

## 留意点

- メニューは [menu.yml](src/_data/menu.yml)（サイトの左のナビゲーション）を正とする。メニューの頁 [menu.md](src/menu.md) は `python3 tools/help_menu.py` で menu.yml から作るので、手で直さない。CI（`build-help`）は `--check` で、menu.md が合っているか、リンク先の頁があるか、頁の `title` とメニューの名前が同じか（違うと今いる頁の印が付かない）を確かめる。
- [src/vb](src/vb) は元の SRC（Ver2.2.33）のヘルプのアーカイブ（https://srch.7474.jp/vb/ ）。`python3 tools/vb_help_archive.py` で [SRC/HelpChm](../SRC/HelpChm) から作るので、手で直さない。見た目（Windows 95 のヘルプに似せた枠）と左の欄（目次・キーワード・検索）は [tools/vb_help_archive](../tools/vb_help_archive) の `winhelp.css`・`winhelp.js` を直してから作り直す。front matter が無いため Jekyll はそのまま配信し、SRC# のヘルプの見た目・検索・`srcsharp help` とは独立する。
- 元の SRC のヘルプ（WinHelp）のポップアップは、次のように扱う。
  - 本文の中の用語（ユニットＩＤ・気力など 20 語）: SRC# のヘルプでは頁へのリンクのまま、リンクに説明の最初の一文を `title` で付ける（指すと出る）。アーカイブでは押すと小さな窓で説明を出す。
  - 図の押せる場所（ステータスウインドウ・MapEdit の図）: SRC# のヘルプでは、図に押せる場所（image map）と、図の下の「項目と説明」の表を呼び出し元の頁に置く（説明の頁 `HLP000xxx` は独立した頁にしない）。アーカイブでは押すと小さな窓で説明を出す。押せる場所の座標は `SRC/Help/*.shg` から読む（`tools/vb_help_archive.py` の `hotspots`）。
