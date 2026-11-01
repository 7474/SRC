#!/usr/bin/env python3
"""開発版（タグ dev のプレリリース）の manifest.json と本文を作る（Packages.yml の dev-release。docs/versioning.md の「開発版」）。

使い方:
  python3 .github/scripts/dev-release.py <配るファイルのフォルダ> <開発版の番号>
  python3 .github/scripts/dev-release.py --version app|library <開発版の番号>   # 開発版の版だけを出す
- <フォルダ>/manifest.json: 元のコミット・版・各ファイルの SHA-256。他のリポジトリはこれで何を取ったかを記録し、確かめる
- dev-release-notes.md: リリースの本文（入手の仕方と、前の正式版からの変更の一覧）
標準ライブラリと git だけを使う。
"""
import datetime
import hashlib
import json
import os
import pathlib
import re
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parents[2]


def read_version():
    props = (ROOT / "SRC.Sharp" / "Directory.Build.props").read_text(encoding="utf-8")
    minor = int(re.search(r"<SrcSharpVersionMinor>(\d+)</SrcSharpVersionMinor>", props).group(1))
    patch = int(re.search(r"<SrcSharpVersionPatch>(\d+)</SrcSharpVersionPatch>", props).group(1))
    return minor, patch


def sha256(path):
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def dev_versions(build):
    # Directory.Build.props の SrcSharpDevBuild と同じく、次のパッチのプレリリースにする
    minor, patch = read_version()
    return f"3.{minor}.{patch + 1}-dev.{build}", f"0.{minor}.{patch + 1}-dev.{build}"


def main():
    if sys.argv[1] == "--version":
        app, library = dev_versions(sys.argv[3])
        print(app if sys.argv[2] == "app" else library)
        return
    dist = pathlib.Path(sys.argv[1])
    build = sys.argv[2]
    app, library = dev_versions(build)
    commit = os.environ.get("GITHUB_SHA") or subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
    repo = os.environ.get("GITHUB_REPOSITORY", "7474/SRC")
    run_url = f"{os.environ.get('GITHUB_SERVER_URL', 'https://github.com')}/{repo}/actions/runs/{os.environ.get('GITHUB_RUN_ID', '')}"

    files = sorted(p for p in dist.iterdir() if p.is_file() and p.name != "manifest.json")
    manifest = {
        "app": app,
        "library": library,
        "commit": commit,
        "run": run_url,
        "created": datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        # Docker イメージ（koudenpa/srcdatalinter:dev）のダイジェスト。固定して使うときはこちらを指す
        "docker": os.environ.get("DEV_DOCKER_IMAGE", ""),
        "files": [{"name": p.name, "size": p.stat().st_size, "sha256": sha256(p)} for p in files],
    }
    (dist / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    changes = subprocess.run(
        [sys.executable, str(ROOT / "tools" / "release.py"), "changes"],
        cwd=ROOT, capture_output=True, text=True,
    ).stdout.strip()
    # 下書き用の注記（HTML のコメント）は本文に要らない
    changes = re.sub(r"<!--.*?-->\s*", "", changes, flags=re.S).strip() or "（前の正式版からの PR はありません）"

    base = f"https://github.com/{repo}/releases/download/dev"
    docker_note = f"（このコミットのもの: `{manifest['docker']}`）" if manifest["docker"] else ""
    nupkg = next((p.name for p in files if p.name.startswith("SRCSharpTools.")), "SRCSharpTools.<版>.nupkg")
    notes = f"""master の最新のコミットから作った**開発版**です。master にマージされるたびに、このリリースの添付を入れ替えます（docs/versioning.md の「開発版」）。正式版は [Latest のリリース](https://github.com/{repo}/releases/latest) を使ってください。

- 版: アプリ `{app}`・ライブラリ `{library}`
- コミット: [{commit[:7]}](https://github.com/{repo}/commit/{commit})
- 作った実行: {run_url}
- 添付の一覧と SHA-256: [manifest.json]({base}/manifest.json)
- Docker イメージ: `koudenpa/srcdatalinter:dev`{docker_note}

## 入手

| 配る物 | ファイル |
|---|---|
| SRC#Avalonia | `SRCSharpAvalonia-<RID>.zip`（linux-x64・win-x64・osx-arm64・osx-x64） |
| srcsharp（.NET tool） | `{nupkg}`（下の手順） |
| VS Code 拡張 | `src-vscode-{library}.vsix`（「VSIX からのインストール」） |
| SRC#Web（外部のサイトに置く） | `SRCSharpWeb.zip` |
| ブラウザで動く lint・データビューア | `SRCLintWeb.zip`・`SRCDataViewer.zip` |

ファイルの URL は `{base}/<ファイル名>` で、マージのたびに中身が変わります。他のリポジトリから使うときは、`manifest.json` の `commit` と SHA-256 を記録してください。

```sh
# Docker（srcsharp の lint・自動プレイ・バランス集計。docs/tools/authoring.md）
docker run --rm -v "$PWD:/s" koudenpa/srcdatalinter:dev lint /s

# .NET tool
gh release download dev -R {repo} -p 'SRCSharpTools.*.nupkg' -D nupkg
dotnet tool install SRCSharpTools --add-source ./nupkg --prerelease --tool-path tools
```

## 前の正式版からの変更

{changes}
"""
    pathlib.Path("dev-release-notes.md").write_text(notes, encoding="utf-8")
    print(app)


if __name__ == "__main__":
    main()
