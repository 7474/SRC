# バージョンの管理

> 更新日: 2026-10-13

SRC# のバージョンの付け方と、リリースの手順。

## バージョン

| 系統 | 対象 | 形式 | 例 |
|---|---|---|---|
| アプリ（プレイできる SRC としての版） | SRC#Form（`SRCSharpForm`）・SRC#Avalonia・SRC#Web・データビューア（`SRCTestBlazor`） | `3.<マイナー>.<パッチ>` | 3.5.0 |
| ライブラリ・ツール | SRCCore・SRCDataLinter・SRCRender・SRCSound・SRCPlay・SRCAuthoring・SRCDebugAdapter・srcsharp（`SRCSharpTools`）・VS Code 拡張（`SRCSharpVSCode`） | `0.<マイナー>.<パッチ>` | 0.5.0 |

- **メジャー**: 2 までを元の SRC（VB 版）とみなし、3 を SRC# のメジャーバージョンとする。ライブラリは API が固まるまで 0
- **マイナー**: 元の SRC にならい、奇数を開発版とする。5 が実質の初版（4 はそれを参照しているシナリオなどがあるため使わない）
- **パッチ**: 上げる時期は定めない
- **マイナーとパッチは両系統で共通**（アプリ 3.5.0 ↔ ライブラリ 0.5.0）

値は `SRC.Sharp/Directory.Build.props` の 1 か所で決める（`SrcSharpVersionMinor`・`SrcSharpVersionPatch`）。各プロジェクトは `SrcSharpAppVersion` か `SrcSharpLibraryVersion` を参照する。

### シナリオから見えるバージョン

`AppVersion`（システム変数）は SRC# のバージョンではなく、互換対象の元の SRC のバージョン（Ver.2.2.33 → 20233）を返す（`SRC.CompatibleVersion`）。シナリオは本体が十分新しいかをこれで確かめるため。

### 表示

元の SRC と同じく、起動時のタイトル（frmTitle）にだけ表示する。

| アプリ | 表示する場所 |
|---|---|
| SRC#Form | タイトル画面（`Forms/Title.cs`） |
| SRC#Avalonia・SRC#Web | 起動時のタイトル（`ScreenGUI.OpenTitleForm`。配置は SRC#Form と同じ） |

## リリース

リリースは、タグを明示的に打った場合のみ行う。

1. `SRC.Sharp/Directory.Build.props` のマイナー・パッチを決めて master に入れる
2. アプリのバージョンでタグを打つ（`v3.<マイナー>.<パッチ>`）
3. `.github/workflows/Publish-Package.yml` が、タグとバージョンが一致することを確かめてから公開する

| 公開先 | 内容 | バージョン |
|---|---|---|
| NuGet | SRCCore・SRCDataLinter・SRCSharpTools（.NET tool の `srcsharp`） | ライブラリ（0.x） |
| GitHub Releases | SRC#Form（Windows 向け zip） | アプリ（3.x） |
| Docker Hub | `koudenpa/srcdatalinter` | イメージのタグはリリースのタグ名（`v3.x.x`）と `latest` |

GitHub Pages（データビューア・SRC#Web）は master に入るたびに更新する（開発版）。

`v0.5.0` まではライブラリのバージョンでタグを打っていた。以降はアプリのバージョン（`v3.x.x`）で打つ。
