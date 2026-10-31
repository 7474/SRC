#!/usr/bin/env python3
"""SRC# のリリースの準備と確かめ（docs/versioning.md の「リリース」）。

  python3 tools/release.py version              # いまの版（アプリ・ライブラリ・VS Code 拡張）
  python3 tools/release.py check [--tag v3.5.5] # 版・CHANGELOG・リリースの案内が揃っているか（CI と公開ワークフローで動かす）
  python3 tools/release.py changes [--from v3.5.4] [--to origin/master]
                                                        # 前のタグからの PR を分野ごとに並べた下書き（Markdown）
  python3 tools/release.py bump [--minor N] [--patch N]
                                                        # 版を上げ、CHANGELOG とリリースの案内のひな形を作る

標準ライブラリと git だけを使う。同じコミットからは同じ結果になる。
"""

import argparse
import json
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROPS = ROOT / "SRC.Sharp" / "Directory.Build.props"
VSCODE = ROOT / "SRC.Sharp" / "SRCSharpVSCode"
CHANGELOG = VSCODE / "CHANGELOG.md"
NOTES_DIR = ROOT / "docs" / "releases"
TEMPLATE = NOTES_DIR / "template.md"
# リリースの案内で、まだ書いていない所の印。残っていると check が通らない
TODO = "<!-- TODO"

# 分野（リリースの案内の「主な変更」の見出しの順）。PR が変えたファイルのうち、最初に当たった分野に入れる
AREAS = [
    ("SRC#Web", ["SRC.Sharp/SRCSharpWeb/"]),
    ("データビューア", ["SRC.Sharp/SRCTestBlazor/"]),
    ("SRC#Avalonia", ["SRC.Sharp/SRCSharpAvalonia/"]),
    ("SRC#Form", ["SRC.Sharp/SRCSharpForm/"]),
    ("本体（SRCCore・SRCRender・SRCSound）", ["SRC.Sharp/SRCCore/", "SRC.Sharp/SRCRender/", "SRC.Sharp/SRCSound/"]),
    ("作成支援（srcsharp・lint・デバッガ）", ["SRC.Sharp/SRCSharpTools/", "SRC.Sharp/SRCAuthoring/", "SRC.Sharp/SRCPlay/",
                                  "SRC.Sharp/SRCDebugAdapter/", "SRC.Sharp/SRCDataLinter/", "SRC.Sharp/SRCLintWeb/"]),
    ("VS Code 拡張", ["SRC.Sharp/SRCSharpVSCode/"]),
    ("ヘルプ", ["SRC.Sharp.Help/"]),
    ("CI", [".github/"]),
    ("文書", ["docs/", "memo/", "AGENTS.md", "CONTRIBUTING.md", "README.md"]),
]
# 分野を決めるときに数えないファイル（テストだけの変更で分野が決まらないように）
TESTS = re.compile(r"(^|/)[^/]*Tests?/|/E2E/")


def git(*args):
    return subprocess.run(["git", *args], cwd=ROOT, check=True, capture_output=True, text=True).stdout


def read_version():
    text = PROPS.read_text(encoding="utf-8")
    minor = re.search(r"<SrcSharpVersionMinor>(\d+)</SrcSharpVersionMinor>", text).group(1)
    patch = re.search(r"<SrcSharpVersionPatch>(\d+)</SrcSharpVersionPatch>", text).group(1)
    return {"minor": int(minor), "patch": int(patch), "app": f"3.{minor}.{patch}", "library": f"0.{minor}.{patch}"}


def vscode_versions():
    package = json.loads((VSCODE / "package.json").read_text(encoding="utf-8"))
    lock = json.loads((VSCODE / "package-lock.json").read_text(encoding="utf-8"))
    return {"package.json": package["version"], "package-lock.json": lock["version"],
            'package-lock.json packages[""]': lock["packages"][""]["version"]}


def notes_path(app):
    return NOTES_DIR / f"v{app}.md"


def cmd_version(_):
    v = read_version()
    print(f"app={v['app']} library={v['library']} vscode={vscode_versions()['package.json']}")


def cmd_check(args):
    v = read_version()
    errors = []
    if args.tag and args.tag != f"v{v['app']}":
        errors.append(f"タグ {args.tag} が SRC# のバージョン {v['app']} と一致しません（SRC.Sharp/Directory.Build.props）")
    for name, value in vscode_versions().items():
        if value != v["library"]:
            errors.append(f"VS Code 拡張の {name} の版 {value} がライブラリの版 {v['library']} と違います")
    if not re.search(rf"^## {re.escape(v['library'])}\b", CHANGELOG.read_text(encoding="utf-8"), re.M):
        errors.append(f"VS Code 拡張の CHANGELOG.md に {v['library']} の節がありません")
    notes = notes_path(v["app"])
    if not notes.exists():
        errors.append(f"リリースの案内 {notes.relative_to(ROOT)} がありません（release.py bump で作る）")
    else:
        text = notes.read_text(encoding="utf-8")
        if TODO in text:
            errors.append(f"リリースの案内 {notes.relative_to(ROOT)} に書きかけ（{TODO}）が残っています")
        for expected in (f"SRC# {v['app']}", v["library"]):
            if expected not in text:
                errors.append(f"リリースの案内 {notes.relative_to(ROOT)} に {expected} がありません")
    for e in errors:
        print(f"::error::{e}")
    if errors:
        sys.exit(1)
    print(f"OK: app={v['app']} library={v['library']}")


def merges(since, until):
    """first-parent のマージコミット（PR）。古い順"""
    out = git("log", "--first-parent", "--reverse", "--format=%H%x1f%s%x1f%b%x1e", f"{since}..{until}")
    for record in filter(str.strip, out.split("\x1e")):
        sha, subject, body = record.strip("\n").split("\x1f")
        m = re.match(r"Merge pull request #(\d+) from (\S+)", subject)
        if not m:
            continue
        title = next((line for line in body.splitlines() if line.strip()), "").strip()
        files = git("diff", "--name-only", f"{sha}^1", sha).split()
        yield {"number": int(m.group(1)), "branch": m.group(2), "title": title, "files": files, "sha": sha}


def area_of(files):
    """PR が変えたファイルがいちばん多い分野（テストは数えない。同じ数なら AREAS の順）"""
    counted = [f for f in files if not TESTS.search(f)] or files
    best = max((sum(any(f.startswith(p) for p in prefixes) for f in counted), -i, name)
               for i, (name, prefixes) in enumerate(AREAS))
    return best[2] if best[0] else "その他"


def previous_tag(until):
    return git("describe", "--tags", "--abbrev=0", "--match", "v3.*", f"{until}^").strip()


def changes_markdown(since, until):
    groups = {}
    deps = []
    for pr in merges(since, until):
        if "/dependabot/" in f"/{pr['branch']}":
            deps.append(pr)
            continue
        groups.setdefault(area_of(pr["files"]), []).append(pr)
    lines = [f"<!-- {since}..{until} の PR（tools/release.py changes）。利用者に伝わる言葉で「主な変更」に書き直す -->", ""]
    order = [name for name, _ in AREAS] + ["その他"]
    for name in order:
        if name not in groups:
            continue
        lines.append(f"- **{name}**")
        for pr in groups[name]:
            lines.append(f"  - {pr['title']}（#{pr['number']}）")
    if deps:
        numbers = "・".join(f"#{p['number']}" for p in deps)
        lines.append(f"- **依存パッケージの更新**（{numbers}）")
    return "\n".join(lines) + "\n"


def cmd_changes(args):
    until = args.to
    since = args.since or previous_tag(until)
    sys.stdout.write(changes_markdown(since, until))


def replace_once(path, pattern, repl, count=1):
    text = path.read_text(encoding="utf-8")
    new, n = re.subn(pattern, repl, text, count=count)
    if n != count:
        raise SystemExit(f"{path.relative_to(ROOT)}: {pattern} が {count} 件見つかりません")
    path.write_text(new, encoding="utf-8")


def cmd_bump(args):
    old = read_version()
    minor = old["minor"] if args.minor is None else args.minor
    patch = (old["patch"] + 1 if args.minor is None else 0) if args.patch is None else args.patch
    if (minor, patch) <= (old["minor"], old["patch"]):
        raise SystemExit(f"版は {old['app']} より新しくする")
    replace_once(PROPS, r"<SrcSharpVersionMinor>\d+</SrcSharpVersionMinor>", f"<SrcSharpVersionMinor>{minor}</SrcSharpVersionMinor>")
    replace_once(PROPS, r"<SrcSharpVersionPatch>\d+</SrcSharpVersionPatch>", f"<SrcSharpVersionPatch>{patch}</SrcSharpVersionPatch>")
    new = read_version()
    lib = new["library"]
    replace_once(VSCODE / "package.json", r'"version": "[^"]+"', f'"version": "{lib}"')
    replace_once(VSCODE / "package-lock.json", r'"version": "[^"]+"', f'"version": "{lib}"', count=2)
    changelog = CHANGELOG.read_text(encoding="utf-8")
    if not re.search(rf"^## {re.escape(lib)}\b", changelog, re.M):
        entry = f"## {lib}\n\n- The version follows the SRC# library version (srcsharp {lib}).\n{TODO}: list the changes of the extension, or remove this line -->\n\n"
        changelog = re.sub(r"^## ", entry + "## ", changelog, count=1, flags=re.M)
        CHANGELOG.write_text(changelog, encoding="utf-8")
    notes = notes_path(new["app"])
    if not notes.exists():
        since = f"v{old['app']}"
        body = TEMPLATE.read_text(encoding="utf-8")
        body = body.replace("{app}", new["app"]).replace("{library}", lib)
        body = body.replace("{changes}", f"{TODO}: 下書きを利用者に伝わる言葉に書き直す -->\n\n" + changes_markdown(since, args.to))
        notes.write_text(body, encoding="utf-8")
    print(f"{old['app']} → {new['app']}（ライブラリ・VS Code 拡張 {lib}）")
    print(f"書き直すもの: {notes.relative_to(ROOT)}、{CHANGELOG.relative_to(ROOT)}（{TODO} の所）")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("version").set_defaults(func=cmd_version)
    p = sub.add_parser("check")
    p.add_argument("--tag")
    p.set_defaults(func=cmd_check)
    p = sub.add_parser("changes")
    p.add_argument("--from", dest="since")
    p.add_argument("--to", default="HEAD")
    p.set_defaults(func=cmd_changes)
    p = sub.add_parser("bump")
    p.add_argument("--minor", type=int)
    p.add_argument("--patch", type=int)
    p.add_argument("--to", default="HEAD", help="下書きに入れる PR の終わり")
    p.set_defaults(func=cmd_bump)
    args = parser.parse_args()
    args.func(args)


if __name__ == "__main__":
    main()
