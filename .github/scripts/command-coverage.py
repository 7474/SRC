#!/usr/bin/env python3
"""
シナリオを動かすテスト（SRCRenderTests: E2E のコーパス・ランタン号など）が、SRCCore のコマンドをどれだけ通したかを集計する。

- イベントコマンド（SRCCore/CmdDatas/Commands/**/<Name>Cmd.cs）: 1 行でも実行された種類の数と、行カバレッジ
- ユニット・マップのコマンド（SRCCore/Commands/Command.*.cs）: ファイルごとの行カバレッジ

使い方: python3 command-coverage.py <cobertura.xml ...>  （Markdown を標準出力に書く。GITHUB_STEP_SUMMARY に足す）
部分クラスの同じ行は行番号で重複を除き、実行回数の最大を取る。
"""
import collections
import re
import sys
import xml.etree.ElementTree as ET

# 処理を持たないコマンド（未実装・非対応の受け皿）は数えない
IGNORED = {"NopCmd", "NotImplementedCmd", "NotSupportedCmd"}


def load(paths):
    hits = collections.defaultdict(dict)
    for path in paths:
        root = ET.parse(path).getroot()
        source = (root.findtext(".//source") or "").rstrip("/\\")
        for cls in root.iter("class"):
            full = (source + "/" + cls.get("filename")).replace("\\", "/")
            m = re.search(r"/SRCCore/(.*)", full)
            if not m:
                continue
            lines = hits[m.group(1)]
            for line in cls.iter("line"):
                n = int(line.get("number"))
                lines[n] = max(lines.get(n, 0), int(line.get("hits")))
    return hits


def rate(lines):
    covered = sum(1 for h in lines.values() if h > 0)
    return covered, len(lines)


def main(paths):
    hits = load(paths)
    if not hits:
        print("SRCCore のカバレッジが見つかりません")
        return 1
    commands = {}
    for f, lines in hits.items():
        m = re.match(r"CmdDatas/Commands/.*/(\w+Cmd)\.cs$", f)
        if m and m.group(1) not in IGNORED:
            commands[m.group(1)] = lines
    executed = {name for name, lines in commands.items() if any(h > 0 for h in lines.values())}
    cmd_cov = [rate(lines) for lines in commands.values()]
    unit_files = {f: lines for f, lines in hits.items() if re.match(r"Commands/Command\.\w+\.cs$", f)}

    def pct(c, n):
        return f"{c / n * 100:.1f}%" if n else "-"

    core_c = sum(c for c, _ in (rate(v) for k, v in hits.items() if not k.startswith("TestLib/")))
    core_n = sum(n for _, n in (rate(v) for k, v in hits.items() if not k.startswith("TestLib/")))
    cmd_c = sum(c for c, _ in cmd_cov)
    cmd_n = sum(n for _, n in cmd_cov)
    unit_c = sum(rate(v)[0] for v in unit_files.values())
    unit_n = sum(rate(v)[1] for v in unit_files.values())

    print("## シナリオで通したコマンド（SRCRenderTests）")
    print()
    print("| 対象 | 結果 |")
    print("|---|---|")
    print(f"| イベントコマンドの種類（1 行でも実行） | {len(executed)} / {len(commands)}（{pct(len(executed), len(commands))}） |")
    print(f"| イベントコマンドの行 | {cmd_c} / {cmd_n}（{pct(cmd_c, cmd_n)}） |")
    print(f"| ユニット・マップのコマンド（Command.*.cs）の行 | {unit_c} / {unit_n}（{pct(unit_c, unit_n)}） |")
    print(f"| SRCCore 全体の行 | {core_c} / {core_n}（{pct(core_c, core_n)}） |")
    print()
    never = sorted(set(commands) - executed)
    print(f"**一度も実行されていないイベントコマンド（{len(never)}）**: " + ("、".join(n[:-3] for n in never) if never else "なし"))
    print()
    low = sorted((rate(commands[n])[0] / rate(commands[n])[1], n) for n in executed if rate(commands[n])[1] >= 30)[:15]
    print("**行カバレッジの低いイベントコマンド（30 行以上、低い順に 15）**: "
          + "、".join(f"{n[:-3]} {r * 100:.0f}%" for r, n in low))
    print()
    print("<details><summary>ユニット・マップのコマンド（ファイルごと）</summary>")
    print()
    print("| ファイル | 行 |")
    print("|---|---|")
    for f in sorted(unit_files, key=lambda f: rate(unit_files[f])[0] / max(rate(unit_files[f])[1], 1)):
        c, n = rate(unit_files[f])
        print(f"| {f} | {c} / {n}（{pct(c, n)}） |")
    print()
    print("</details>")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
