#!/usr/bin/env python3
"""
ヘルプ（SRC.Sharp.Help）のメニューを 1 か所で管理する。

メニューの正は `SRC.Sharp.Help/src/_data/menu.yml`（サイトの左のナビゲーション）。
`SRC.Sharp.Help/src/menu.md`（メニューの頁）はこのスクリプトが menu.yml から作るので、手で直さない。

    python3 tools/help_menu.py          # menu.md を作り直す
    python3 tools/help_menu.py --check  # menu.md が menu.yml と合っているか・リンク先の頁があるかを確かめる（CI）

menu.yml は決まった形（toc の下に title・subfolderitems・page・url）なので、YAML のライブラリを使わずに読む。
"""
import os
import pathlib
import re
import sys
import urllib.parse

ROOT = pathlib.Path(__file__).resolve().parent.parent
SRC = ROOT / "SRC.Sharp.Help" / "src"
MENU_YML = SRC / "_data" / "menu.yml"
MENU_MD = SRC / "menu.md"

HEADER = """---
layout: default
title: メニュー
---
<!-- このファイルは tools/help_menu.py が _data/menu.yml から作る。手で直さない -->
"""


def parse(text):
    """menu.yml を (深さ, 種類, 値) の並びにする。種類は title（見出し）か page（頁。値は (名前, url)）"""
    entries = []
    pending = None
    for number, line in enumerate(text.splitlines(), 1):
        if not line.strip() or line.strip() == "toc:" or line.strip() == "subfolderitems:":
            continue
        m = re.match(r"^( *)- (title|page): (.+)$", line)
        if m:
            if pending:
                raise ValueError(f"menu.yml:{pending[3]}: page に url がありません")
            depth = (len(m.group(1)) - 2) // 4
            if m.group(2) == "title":
                entries.append((depth, "title", m.group(3).strip()))
            else:
                pending = (depth, "page", m.group(3).strip(), number)
            continue
        m = re.match(r"^ *url: (.+)$", line)
        if m and pending:
            entries.append((pending[0], "page", (pending[2], m.group(1).strip())))
            pending = None
            continue
        raise ValueError(f"menu.yml:{number}: 読めない行です: {line}")
    if pending:
        raise ValueError(f"menu.yml:{pending[3]}: page に url がありません")
    return entries


def page_file(url):
    """/SRCSharpのインストール → SRCSharpのインストール.md"""
    return url.lstrip("/") + ".md"


def render(entries):
    lines = [HEADER.rstrip("\n")]
    for depth, kind, value in entries:
        indent = "    " * depth
        if kind == "title":
            lines.append(f"{indent}- {value}")
        else:
            name, url = value
            lines.append(f"{indent}- [{name}]({page_file(url)})")
    return "\n".join(lines) + "\n"


def problems(entries):
    """リンク先の頁が無い・頁の title とメニューの名前が違う（今いる頁の印が付かない）"""
    found = []
    for _, kind, value in entries:
        if kind != "page":
            continue
        name, url = value
        path = SRC / urllib.parse.unquote(page_file(url))
        if not path.exists():
            found.append(f"{name}: 頁 {path.name} がありません")
            continue
        m = re.search(r"^title: *(.+)$", path.read_text(encoding="utf-8"), re.M)
        if m and m.group(1).strip() != name:
            found.append(f"{name}: 頁 {path.name} の title は「{m.group(1).strip()}」です")
    return found


def main(argv):
    entries = parse(MENU_YML.read_text(encoding="utf-8"))
    expected = render(entries)
    if "--check" in argv:
        errors = problems(entries)
        if MENU_MD.read_text(encoding="utf-8") != expected:
            errors.append("menu.md が menu.yml と合っていません。python3 tools/help_menu.py で作り直してください")
        for error in errors:
            print(f"::error::{error}" if "GITHUB_ACTIONS" in os.environ else error)
        if errors:
            return 1
        print("OK: menu.md は menu.yml と合っています")
        return 0
    MENU_MD.write_text(expected, encoding="utf-8")
    for error in problems(entries):
        print(error)
    print(f"{MENU_MD.relative_to(ROOT)} を作りました")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
