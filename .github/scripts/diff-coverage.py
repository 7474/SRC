"""PR で変えた行のうち、テストが通った割合（#1363）。

  python3 .github/scripts/diff-coverage.py <基準のコミット> <coverage.cobertura.xml>... >> "$GITHUB_STEP_SUMMARY"

SRC.Sharp の *.cs（テストのプロジェクトを除く）で、基準から増えた・変わった行のうち、カバレッジの対象になる行（Cobertura の line）を数える。
下限は設けず、少ないときに warning の注釈を出すだけにする（自動化を優先し、リグレッションが起きたら見直す。docs/testing.md）。
"""
import re
import subprocess
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict

WARN_BELOW = 50  # %。変えた行が WARN_MIN_LINES 以上で、これを下回ったら warning
WARN_MIN_LINES = 10


def repo_path(filename):
    filename = filename.replace("\\", "/")
    i = filename.find("SRC.Sharp/")
    return filename[i:] if i >= 0 else None


def coverage(files):
    hits = defaultdict(dict)
    for f in files:
        root = ET.parse(f).getroot()
        # filename は <sources> の <source> からの道のことがある
        sources = [s.text or "" for s in root.iter("source")] or [""]
        for cls in root.iter("class"):
            path = next((p for p in (repo_path(src.rstrip("/\\") + "/" + cls.get("filename")) for src in sources) if p), None) \
                or repo_path(cls.get("filename"))
            if not path:
                continue
            for line in cls.iter("line"):
                n = int(line.get("number"))
                hits[path][n] = max(hits[path].get(n, 0), int(line.get("hits")))
    return hits


def changed_lines(base):
    out = subprocess.run(["git", "diff", "-U0", base, "HEAD", "--", "SRC.Sharp/*.cs"],
                         check=True, capture_output=True, text=True).stdout
    changed = defaultdict(set)
    path = None
    for line in out.splitlines():
        if line.startswith("+++ "):
            path = line[6:] if line.startswith("+++ b/") else None
            if path and re.search(r"(^|/)[^/]*Tests?/", path):
                path = None
        elif line.startswith("@@") and path:
            m = re.search(r"\+(\d+)(?:,(\d+))?", line)
            start, count = int(m.group(1)), int(m.group(2) or 1)
            changed[path].update(range(start, start + count))
    return changed


def main():
    base, files = sys.argv[1], sys.argv[2:]
    hits = coverage(files)
    rows = []
    total = covered = 0
    for path, lines in sorted(changed_lines(base).items()):
        measured = [n for n in sorted(lines) if n in hits.get(path, {})]
        if not measured:
            continue
        ok = [n for n in measured if hits[path][n] > 0]
        missed = [n for n in measured if hits[path][n] == 0]
        total += len(measured)
        covered += len(ok)
        rows.append((path, len(ok), len(measured), missed))
    print("## 変えた行のカバレッジ\n")
    if not total:
        print("カバレッジの対象になる変更はありません（テスト・文書だけ、または計っていないプロジェクト）。")
        return
    rate = 100 * covered / total
    print(f"変えた行のうちテストが通った行: **{covered} / {total}（{rate:.1f}%）**\n")
    print("| ファイル | 通った行 | 通っていない行 |\n|---|---|---|")
    for path, ok, n, missed in rows:
        shown = ", ".join(map(str, missed[:15])) + (" …" if len(missed) > 15 else "")
        print(f"| {path} | {ok} / {n} | {shown} |")
    if total >= WARN_MIN_LINES and rate < WARN_BELOW:
        print(f"::warning::変えた行のカバレッジが {rate:.1f}%（{covered} / {total}）。テストを足せないか確かめる", file=sys.stderr)


if __name__ == "__main__":
    main()
