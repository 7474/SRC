---
name: release
description: SRC# のリリースを準備する（版を上げる・リリースの案内と VS Code 拡張の CHANGELOG を書く・タグを打つ前の確かめ）。「リリースの準備」「バージョンを上げる」「パッチを出す」「リリースノートを書く」と言われたときに使う。
---

# SRC# のリリースの準備

手順の正は [docs/versioning.md](../../../docs/versioning.md) の「リリース」、案内の書き方は [docs/releases/README.md](../../../docs/releases/README.md)。ここでは作業の順と、確かめることをまとめる。

版を上げる・案内を書くのは 1 つの PR にする。タグは、その PR が master に入ってから打つ（公開はタグで動き、取り消しにくい）。タグを打つのは、利用者に頼まれたときだけ。

## 1. 上げる版を決める

```sh
git fetch origin master --tags
python3 tools/release.py version
git describe --tags --abbrev=0 --match 'v3.*' origin/master
```

- パッチを上げるのが普通。マイナーは奇数を開発版とする（docs/versioning.md）。マイナーを上げるかは利用者に確かめる。
- いまの版のタグがまだ無ければ（前のリリースの後に版を上げただけなら）、版は上げずに案内を書き足す。

## 2. 版を上げ、ひな形を作る

`origin/master` から作ったブランチで動かす。

```sh
python3 tools/release.py bump              # パッチを 1 つ上げる
python3 tools/release.py bump --minor 7    # マイナーを上げる（パッチは 0）
```

次のものが変わる。

- `SRC.Sharp/Directory.Build.props` のマイナー・パッチ
- `SRC.Sharp/SRCSharpVSCode/package.json` と `package-lock.json` の版（ライブラリと同じ `0.<マイナー>.<パッチ>`）
- `SRC.Sharp/SRCSharpVSCode/CHANGELOG.md` に新しい版の節（`<!-- TODO` 付き）
- `docs/releases/v<版>.md`（`template.md` から。「主な変更」に PR の下書きが入る）

## 3. 案内を書く

`docs/releases/v<版>.md` の `<!-- TODO` をすべて埋めるか消す。

1. 下書きの PR を 1 つずつ読む。PR の題だけで書かない。
   - `git log --format='%s%n%b' <マージ>^1..<マージ>^2` で、PR の中のコミットの説明が読める。
   - 説明が足りなければ PR の本文を読む（GitHub の MCP の `pull_request_read`）。
2. 利用者に見えない変更（テスト・CI・文書・依存パッケージ）は「その他」に寄せる。
3. 既存のシナリオの見え方や結果が変わるもの（元の SRC に合わせた修正を含む）は「動きが変わる点」に書く。
4. 書いたことを、コード・ヘルプ・文書と突き合わせる。
   - 効く範囲（どのフロントエンドか、どの設定のときか）。
   - コマンド名・設定名・ヘルプの URL（`SRC.Sharp.Help/src/<題>.md` があるか）。
   - 数字（速さなど）は PR に書かれた値だけを使う。
5. VS Code 拡張の CHANGELOG（英語）を書く。拡張そのものに変更が無ければ、そう書く。

過去の案内（`docs/releases/v3.5.*.md`）を文体の手本にする。

## 4. 確かめる

```sh
python3 tools/release.py check
```

`OK` になること。CI の `release-check` ジョブも同じことを確かめる。PR を作り、CI が通ってからマージする。

## 5. タグを打つ（利用者に頼まれたら）

master に入った後で、版の PR のマージコミットに打つ。

```sh
git fetch origin master --tags
python3 tools/release.py check
git tag v<版> origin/master
git push origin v<版>
```

タグの push で次のワークフローが動く。

| ワークフロー | 内容 |
|---|---|
| `Publish-Package.yml` | 版の確かめ（`check --tag`）→ NuGet・SRC#Form・SRC#Web の zip・Docker Hub。リリースの本文は `docs/releases/v<版>.md` |
| `Publish-Avalonia.yml` | SRC#Avalonia の OS ごとの zip |
| `Publish-VSCode.yml` | VS Code 拡張（Marketplace・Open VSX・.vsix） |

終わったら、リリースの頁で本文と添付（`SRCSharpForm.zip`・`SRCSharpWeb.zip`・`SRCSharpAvalonia-<RID>.zip` 4 つ・`src-vscode-<版>.vsix`・`LICENSE`）を確かめる。

案内をリリースの後に直すときは、`docs/releases/v<版>.md` を直す PR を作り、GitHub のリリースの本文にも同じものを貼る。
