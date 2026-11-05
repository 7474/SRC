#!/usr/bin/env python3
"""VB 版の手続きと C# のメソッドの対応表（docs/history/porting/vb-map/）を作る・確かめる（7474/SRC#1498）。

  python3 tools/vb_port_map.py          # 対応表を作る（手で書いた「C#」「状態」「メモ」は残す）
  python3 tools/vb_port_map.py --check  # 対応表に VB のすべての手続きがあり、状態の書き方が正しいかを確かめる（CI）
  python3 tools/vb_port_map.py --stats  # 状態ごとの数を出す

VB 版（SRC/SRC_20121125、UTF-8）の Function・Sub・Property を取り出し、同じ名前の C# のメソッド・プロパティ・クラスを
候補として書く。候補は名前だけで選ぶので、突き合わせるときに人が確かめて直す。
"""
import argparse
import collections
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
VB_DIR = ROOT / "SRC" / "SRC_20121125"
CS_DIRS = [ROOT / "SRC.Sharp" / d for d in ("SRCCore", "SRCRender", "SRCSharpForm")]
OUT_DIR = ROOT / "docs" / "history" / "porting" / "vb-map"

# 状態の書き方。表の「状態」の欄はこのどれかで始める（後ろに「（#123）」などを続けてよい）
STATES = [
    ("未確認", "まだ突き合わせていない"),
    ("一致", "分岐ごとに突き合わせ、違いが無い"),
    ("修正済み", "違いを直した。Issue・PR の番号をメモに書く"),
    ("違いあり", "違いが残っている。Issue の番号をメモに書く"),
    ("SRC#で不要", "SRC# では要らない（VB の画面の部品・Win32 API など）。理由をメモに書く"),
    ("C#に無い", "C# に当たる処理が無く、要るかを決めていない。Issue の番号をメモに書く"),
]
STATE_NAMES = [s for s, _ in STATES]

PROC_RE = re.compile(
    r"^\s*(?:(Public|Private|Friend)\s+)?(?:Static\s+)?(Function|Sub|Property\s+(?:Get|Let|Set))\s+(\w+)",
    re.IGNORECASE,
)
END_RE = re.compile(r"^\s*End\s+(Function|Sub|Property)\b", re.IGNORECASE)

# VB のモジュールと、それを主に移した C# の型。名前の候補が複数あるとき、この型のものを先に出す
MODULE_TYPES = {
    "Command": ["Commands", "Command"],
    "GUI": ["IGUI", "ScreenGUI", "SRCSharpFormGUI"],
    "Intermission": ["InterMission"],
    "Status": ["IGUIStatus", "GUIStatus", "ScreenGUI", "SRCSharpFormGUIStatus"],
    "Units": ["UnitList", "Units"],
    "Pilots": ["PilotList", "Pilots"],
    "Items": ["ItemList", "Items"],
    "Main": ["frmMain", "ScreenGUI", "Main"],
}

CS_DECL_RE = re.compile(
    r"^\s*(?:\[[^\]]*\]\s*)*(?:(?:public|private|protected|internal|static|virtual|override|abstract|sealed|async|new|partial|extern|unsafe|readonly)\s+)+"
    r"(?:(class|interface|struct|enum)\s+(\w+)|[\w<>\[\],.? ]+?\s+(\w+)\s*(?:\(|\{|=>|$))"
)
CS_TYPE_RE = re.compile(r"\b(?:class|interface|struct)\s+(\w+)")


def read_vb():
    """VB のモジュールごとの手続きの一覧 [(kind, name, line, lines)]"""
    modules = collections.OrderedDict()
    for path in sorted(VB_DIR.iterdir()):
        if path.suffix.lower() not in (".bas", ".cls", ".frm"):
            continue
        lines = path.read_text(encoding="utf-8", errors="replace").splitlines()
        procs = []
        current = None
        for i, line in enumerate(lines, 1):
            m = PROC_RE.match(line)
            if m and not line.strip().lower().startswith(("declare", "public declare", "private declare")):
                kind = re.sub(r"\s+", " ", m.group(2)).title()
                current = [kind, m.group(3), i, i]
                procs.append(current)
                continue
            if current and END_RE.match(line):
                current[3] = i
                current = None
        modules[path.name] = [(k, n, s, e - s + 1) for k, n, s, e in procs]
    return modules


def read_cs():
    """名前（小文字）→ [(相対パス, 行, 型, 名前)]"""
    index = collections.defaultdict(list)
    for base in CS_DIRS:
        for path in base.rglob("*.cs"):
            rel = path.relative_to(ROOT / "SRC.Sharp").as_posix()
            if "/obj/" in rel or "/bin/" in rel or rel.endswith(".ref.cs"):
                continue
            current_type = None
            for i, line in enumerate(path.read_text(encoding="utf-8", errors="replace").splitlines(), 1):
                t = CS_TYPE_RE.search(line)
                m = CS_DECL_RE.match(line)
                if not m:
                    continue
                if m.group(1):
                    current_type = m.group(2)
                    index[m.group(2).lower()].append((rel, i, m.group(2), m.group(2)))
                elif m.group(3) and current_type:
                    index[m.group(3).lower()].append((rel, i, current_type, m.group(3)))
    return index


def candidates(module, name, cs):
    stem = module.rsplit(".", 1)[0]
    if module == "CmdData.cls":
        m = re.match(r"Exec(\w+)Cmd$", name, re.IGNORECASE)
        if m:
            hits = [x for x in cs.get((m.group(1) + "Cmd").lower(), []) if x[2] == x[3]]
            return hits[:1]
    hits = cs.get(name.lower(), [])
    preferred = MODULE_TYPES.get(stem, []) + [stem]
    # モジュールに当たる型に同じ名前があれば、それだけを出す
    if any(x[2] in preferred for x in hits):
        hits = [x for x in hits if x[2] in preferred]
    hits = sorted(hits, key=lambda x: (preferred.index(x[2]) if x[2] in preferred else len(preferred), x[0], x[1]))
    # 同じ型の同じ名前（オーバーロード・partial）は 1 つにまとめる
    seen, result = set(), []
    for h in hits:
        key = (h[0], h[2], h[3])
        if key in seen:
            continue
        seen.add(key)
        result.append(h)
    return result[:3]


def format_cs(hits):
    if not hits:
        return "（見つからない）"
    return "<br>".join(f"`{p}` {t}.{n}" if t != n else f"`{p}` {t}" for p, _, t, n in hits)


HEADER = "| VB | 行 | 行数 | C# | 状態 | メモ |\n|---|---|---|---|---|---|\n"


def row_key(kind, name):
    return f"{kind} {name}"


def read_table(path):
    rows = {}
    if not path.exists():
        return rows
    for line in path.read_text(encoding="utf-8").splitlines():
        if not line.startswith("| ") or line.startswith("| VB ") or line.startswith("|---"):
            continue
        cols = [c.strip() for c in line.strip().strip("|").split("|")]
        if len(cols) != 6:
            continue
        rows[cols[0]] = cols
    return rows


def module_file(module):
    return OUT_DIR / (module.replace(".", "_") + ".md")


def generate(vb, cs):
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    for module, procs in vb.items():
        path = module_file(module)
        old = read_table(path)
        lines = [
            f"# {module}",
            "",
            f"VB 版の `SRC/SRC_20121125/{module}` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。",
            "",
        ]
        body = HEADER
        for kind, name, line, count in procs:
            key = row_key(kind, name)
            prev = old.get(key)
            cs_cell = prev[3] if prev and prev[3] else format_cs(candidates(module, name, cs))
            state = prev[4] if prev else "未確認"
            memo = prev[5] if prev else ""
            body += f"| {key} | {line} | {count} | {cs_cell} | {state} | {memo} |\n"
        path.write_text("\n".join(lines) + "\n" + body, encoding="utf-8")
    write_readme(vb)


def stats(vb):
    total = collections.Counter()
    per_module = collections.OrderedDict()
    for module in vb:
        c = collections.Counter()
        for cols in read_table(module_file(module)).values():
            c[cols[4].split("（")[0].strip()] += 1
        per_module[module] = c
        total.update(c)
    return total, per_module


def write_readme(vb):
    total, per_module = stats(vb)
    lines = [
        "# VB 版と C# の手続きの対応表",
        "",
        "VB 版（`SRC/SRC_20121125`）の手続き（Function・Sub・Property）を、C# の対応するメソッドと 1 つずつ突き合わせるための表（7474/SRC#1498）。",
        "",
        "- 表は `python3 tools/vb_port_map.py` で作る。作り直しても、手で書いた「C#」「状態」「メモ」は残る。行番号は VB 版の行",
        "- CI（`test-lint`）の `python3 tools/vb_port_map.py --check` で、VB のすべての手続きが表にあり、状態の書き方が正しいかを確かめる",
        "- 「C#」の候補は名前だけで選んでいる。`（見つからない）` は名前の違う C# に移したか、移していないもの。突き合わせのときに直す",
        "",
        "## 突き合わせの手順",
        "",
        "1. VB の手続きと C# のメソッドを並べ、分岐（`If`・`Select Case`・ループの抜け方・`Exit`・`On Error`）ごとに同じかを確かめる",
        "2. 違いは、元の SRC の動作を期待値にしたテストにして直す（[判断の記録](../../../decisions/2026-10-15-vb-behavior-as-test-oracle.md)）。すぐに直せないものは Issue にして、テストに `[Ignore(\"移植の不具合: …\")]` を付ける",
        "3. その手続きに当たる C# の `TODO`・`XXX`・コメントアウトされた VB のコード（[一覧](todo.md)）を判断する（直す・SRC# では要らない・Issue にする）",
        "4. 表の「状態」と「メモ」を書く。機械で見つけられる型の誤りなら、[SRCPortingAnalyzers](../../../../SRC.Sharp/SRCPortingAnalyzers/README.md) に規則を足す",
        "",
        "## 状態",
        "",
        "| 状態 | 意味 |",
        "|---|---|",
    ]
    lines += [f"| {s} | {d} |" for s, d in STATES]
    lines += [
        "",
        "## 進み具合",
        "",
        "| モジュール | 手続き | " + " | ".join(STATE_NAMES) + " |",
        "|---|---|" + "---|" * len(STATE_NAMES),
    ]
    for module, c in per_module.items():
        n = len(vb[module])
        if n == 0:
            continue
        lines.append(f"| [{module}]({module_file(module).name}) | {n} | " + " | ".join(str(c.get(s, 0)) for s in STATE_NAMES) + " |")
    lines.append(f"| 計 | {sum(len(p) for p in vb.values())} | " + " | ".join(str(total.get(s, 0)) for s in STATE_NAMES) + " |")
    (OUT_DIR / "README.md").write_text("\n".join(lines) + "\n", encoding="utf-8")


def check(vb):
    errors = []
    for module, procs in vb.items():
        path = module_file(module)
        rows = read_table(path)
        if procs and not path.exists():
            errors.append(f"{path.relative_to(ROOT)} がありません")
            continue
        keys = {row_key(k, n) for k, n, _, _ in procs}
        for key in keys - rows.keys():
            errors.append(f"{path.relative_to(ROOT)}: {key} の行がありません")
        for key in rows.keys() - keys:
            errors.append(f"{path.relative_to(ROOT)}: {key} は VB にありません")
        for key, cols in rows.items():
            state = cols[4]
            if not any(state.startswith(s) for s in STATE_NAMES):
                errors.append(f"{path.relative_to(ROOT)}: {key} の状態「{state}」は {'・'.join(STATE_NAMES)} のどれかで始めます")
            elif not state.startswith(("未確認", "一致")) and not cols[5]:
                errors.append(f"{path.relative_to(ROOT)}: {key} は状態が「{state}」なので、メモに理由か Issue の番号を書きます")
    return errors


TODO_RE = re.compile(r"//\s*(TODO|XXX|FIXME)\b(.*)")
# コメントアウトされた VB・C# のコード（「// If ...」「// x = ...;」のような行）
COMMENTED_CODE_RE = re.compile(r"^\s*//\s*(?:(?:If|ElseIf|Else|End If|Select Case|Case|End Select|For|Next|Do|Loop|With|End With|Exit|Call|Set|Dim|GoTo)\b|.*[;{}]\s*$)")


def write_todo():
    """SRCCore の TODO・XXX・コメントアウトされたコードの一覧（todo.md）。手で書いた「状態」「メモ」は、ファイルと本文が同じ行に残す"""
    path = OUT_DIR / "todo.md"
    old = {}
    if path.exists():
        for line in path.read_text(encoding="utf-8").splitlines():
            cols = [c.strip() for c in line.strip().strip("|").split("|")]
            if line.startswith("| `") and len(cols) == 5:
                old[(cols[0].split(":")[0], cols[2])] = (cols[3], cols[4])
    rows = []
    base = ROOT / "SRC.Sharp"
    for file in sorted((base / "SRCCore").rglob("*.cs")):
        rel = file.relative_to(base).as_posix()
        if "/obj/" in rel or "/bin/" in rel or rel.endswith(".ref.cs"):
            continue
        block = None
        for i, line in enumerate(file.read_text(encoding="utf-8", errors="replace").splitlines(), 1):
            m = TODO_RE.search(line)
            if m:
                rows.append((rel, i, m.group(1), (m.group(1) + m.group(2)).strip()))
                block = None
                continue
            if COMMENTED_CODE_RE.match(line):
                # 続いたコメントアウトは 1 つにまとめ、最初の行を書く
                if block is None:
                    block = [rel, i, "コード", line.strip()[2:].strip(), 1]
                    rows.append(block)
                else:
                    block[4] = i - block[1] + 1
                continue
            # 空行とふつうのコメントは、コメントアウトのまとまりを切らない
            if block is not None and (not line.strip() or line.strip().startswith("//")):
                continue
            block = None
    lines = [
        "# SRCCore の TODO・XXX・コメントアウトされたコード",
        "",
        "VB 版との突き合わせ（[対応表](README.md)）のときに、その手続きに当たるものを 1 つずつ判断する。`python3 tools/vb_port_map.py` で作り直す（手で書いた「状態」「メモ」は、ファイルと本文が同じなら残る）。",
        "",
        "- 種類: `TODO`・`XXX`・`FIXME` はコメント、`コード` はコメントアウトされたコード（続いた行は 1 つにまとめ、最初の行と行数を書く）",
        "- 状態: `未確認`・`直した`（PR の番号）・`SRC#で不要`（理由）・`Issue`（番号）",
        "",
        "| 場所 | 種類 | 本文 | 状態 | メモ |",
        "|---|---|---|---|---|",
    ]
    for r in rows:
        rel, i, kind, text = r[0], r[1], r[2], r[3]
        text = text.replace("|", "\\|")
        if len(text) > 120:
            text = text[:117] + "..."
        where = f"`{rel}:{i}`" + (f"（{r[4]} 行）" if kind == "コード" and r[4] > 1 else "")
        state, memo = old.get((f"`{rel}", text), ("未確認", ""))
        lines.append(f"| {where} | {kind} | {text} | {state} | {memo} |")
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")
    return len(rows)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--check", action="store_true")
    parser.add_argument("--stats", action="store_true")
    args = parser.parse_args()
    vb = read_vb()
    if args.check:
        errors = check(vb)
        for e in errors:
            print(e)
        if errors:
            print("python3 tools/vb_port_map.py で表を作り直し、状態を直してください")
            return 1
        print(f"OK: {sum(len(p) for p in vb.values())} の手続きが対応表にあります")
        return 0
    if args.stats:
        total, _ = stats(vb)
        for s in STATE_NAMES:
            print(s, total.get(s, 0))
        return 0
    generate(vb, read_cs())
    write_readme(vb)
    print(f"TODO・コメントアウト: {write_todo()} 件")
    print(f"{OUT_DIR.relative_to(ROOT)} を作りました")
    return 0


if __name__ == "__main__":
    sys.exit(main())
