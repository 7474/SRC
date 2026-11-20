#!/usr/bin/env python3
"""VB 版の手続きと C# のメソッドの対応表（docs/history/porting/vb-map/）を作る・確かめる（7474/SRC#1498）。

  python3 tools/vb_port_map.py          # 対応表を作る（手で書いた「C#」「状態」「メモ」は残す）
  python3 tools/vb_port_map.py --check  # 対応表に VB のすべての手続きがあり、状態の書き方が正しいか、
                                        # SRCCore の TODO・XXX・コメントアウトされたコードに「未確認」が無いかを確かめる（CI）
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
            if "/obj/" in rel or "/bin/" in rel:
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
        "- CI（`port-map-check`）の `python3 tools/vb_port_map.py --check` で、VB のすべての手続きが表にあり、状態の書き方が正しいか、[TODO・コメントアウトの一覧](todo.md)に「未確認」が無いかを確かめる",
        "- 対象は `SRC/SRC_20121125` の `*.bas`・`*.cls`・`*.frm` のすべての手続き。選んで外した手続きは無い（手続きの外にあるものは下の「表に載っていないもの」）",
        "- 「C#」の候補は名前だけで選んでいる。`（見つからない）` は名前の違う C# に移したか、移していないもの。突き合わせのときに直す",
        "- 「一致」の 50 件を抜き取って読み直した結果は [resample-1556.md](resample-1556.md)（7474/SRC#1556。食い違いは 2 件で、打ち切った）",
        "",
        "## 突き合わせの手順",
        "",
        "1. VB の手続きと C# のメソッドを並べ、分岐（`If`・`Select Case`・ループの抜け方・`Exit`・`On Error`）ごとに同じかを確かめる",
        "2. 違いは、元の SRC の動作を期待値にしたテストにして直す（[判断の記録](../../../decisions/2026-10-15-vb-behavior-as-test-oracle.md)）。すぐに直せないものは Issue にして、テストに `[Ignore(\"移植の不具合: …\")]` を付ける",
        "3. その手続きに当たる C# の `TODO`・`XXX`・コメントアウトされた VB のコード（[一覧](todo.md)）を判断する（直す・SRC# では要らない・Issue にする）",
        "4. 表の「状態」と「メモ」を書く。機械で見つけられる型の誤りなら、[SRCPortingAnalyzers](../../../../SRC.Sharp/SRCPortingAnalyzers/README.md) に規則を足す",
        "5. あとの単位で扱う手がかりは、その手続きの行の「メモ」に書く（状態は「未確認」のまま）",
        "",
        "## 表に載っていないもの（突き合わせのときに一緒に見る）",
        "",
        "表は手続き（Function・Sub・Property）を単位にしているので、次のものは載っていない。手続きを突き合わせるときに、そのモジュールの分を一緒に見る。",
        "",
        "`Const`・`Type`・`Enum`・モジュールの変数を突き合わせた結果は [declarations.md](declarations.md) にある（7474/SRC#1498 の最後の項目）。",
        "",
        "| 種類 | 見ること |",
        "|---|---|",
        "| モジュールの変数の宣言（手続きの外。約 600） | 型（Integer・Long・Single・Double・String・Variant）。代入の丸め・あふれ・既定値が型で決まる。初期値は `Class_Initialize` の行で見る |",
        "| `Const`（約 120）・`Type`・`Enum`（約 30） | 値と項目が C# の定数・型と同じか。既定値・上限などの数値が違えば動作が変わる（#1498 のチェックリストの「モジュールの宣言」の項目でまとめて見る） |",
        "| `Declare`（Win32 API の宣言。約 120） | SRC# では使わない。それを呼ぶ手続きの行で、代わりの処理を見る |",
        "| `.frm` の画面の定義（`Begin VB.Form` 〜 `Attribute VB_Name` の前）・`.frx` | ボタン・リストなどの配置・大きさ・既定の文言・タブの順。フォームの手続きを突き合わせるときに、SRCRender・SRCSharpForm の画面と比べる |",
        "| 各ファイルの先頭の `Option` 文 | `Option Compare` を書いたファイルは無い。文字列の比較は既定の Binary（大文字・小文字・全角半角・ひらがなカタカナを区別する。ただし Collection のキーは区別しない） |",
        "",
        "## 元の SRC の実行ファイルのコンパイルの設定（`SRC/SRC_20121125/SRC.VBP`）",
        "",
        "「VB では実行時エラーになる（ならない）」と判断するときは、配布された Src.exe のコンパイルの設定と、下の実機で確かめた結果を考える。`SRC.VBP` には次の指定がある。",
        "",
        "```",
        "BoundsCheck=-1      ' 配列の範囲チェックをしない",
        "OverflowCheck=-1    ' 整数のオーバーフローチェックをしない",
        "FDIVCheck=-1        ' Pentium の FDIV の不具合のチェックをしない",
        "NoAliasing=-1       ' 別名（エイリアス）が無いとみなす",
        "FlPointCheck=-1     ' 浮動小数点のエラーのチェックをしない",
        "UnroundedFP=0       ' 浮動小数点の途中の結果を丸める（丸めない指定はしていない）",
        "```",
        "",
        "- -1 は、VB5 のプロジェクトのプロパティ（コンパイル → 詳細な最適化）の項目に印が付いた状態（チェックをしない）と読んでいる。`UnroundedFP=0` なので、途中の結果は Double に丸められる（C# の double と同じ）",
        "- 以前はこの読み方から「範囲外の参照・整数のあふれ・浮動小数点のあふれは実行時エラーにならない」と考えていた。元の SRC の実機（#1550。2026-11-12、Ver.2.2.33 の Src.exe、日本語版の Windows 11。[PROBE.md](../../../../SRC.Sharp/Compat/PROBE.md) の「実機で確かめた結果」）では、確かめた整数のあふれ・浮動小数点のあふれと非数は**どれも実行時エラーになった**（イベントのコマンドの中なので、`CmdData.cls` の `Exec` の ErrorHandler が「イベントデータが不正です」を出してイベントを止める。Src.exe は落ちない）",
        "- これらの設定が外すのは、コンパイラーがその場に埋め込む確かめ（Integer・Long どうしの足し算などのあふれ、配列の添字の範囲、浮動小数点の演算の後の状態）だけで、型の変換などランタイム（MSVBVM50.DLL）の関数の中の確かめは残る、と考えると実機の結果と合う。ただし、どの命令でエラーになったかは確かめていない（下の表の「エラーの起きる所」は、元の SRC のコードから読んだ候補）",
        "",
        "### 実機で確かめた結果",
        "",
        "| 項目（プローブの項目名） | 実機 | 元の SRC のコードと、エラーの起きる所 |",
        "|---|---|---|",
        "| 引数の無い `Incr`・`Ride`・`UnSet`（`VB5.範囲外.*`。`probe-bounds/`） | 「イベントデータが不正です」。Src.exe は落ちない | `CmdData.cls` の `Parse`（81〜82 行）は `ArgNum > 1` のときだけ `ReDim strArgs(2 To ArgNum)` する。引数が無いと `strArgs` は一度も ReDim されない（配列が無い）。`ExecIncrCmd`（6839 行）・`ExecRideCmd`（10727 行）・`ExecUnsetCmd`（12726 行）は引数の数を確かめずに `GetArg(2)`（`strArgs(2)`）を読む。範囲外だからではなく、配列が無いのを読んだためのエラーと読める。範囲チェックを外しても配列が無いときにエラーになる仕組み（ランタイムが確かめるのか）は確かめていない |",
        "| `Args(0)`・`Args(-1)`（`VB5.範囲外.Args(0)`・`Args(-1)`） | エラーにならず `[a2]`・`[a1]`（前の呼び出しが残した値） | `Expression.bas` の `Args`（1053〜1057 行）は番号の上だけを確かめて `ArgStack(ArgIndex - num + 1)` を読む。0・-1 は `ArgStack(0 To 200)`（`Event.bas` 71 行）の**範囲の中**の、今の引数より上（前の呼び出しが残した所）を読むので、範囲チェックを外したことの裏付けにはならない（SRC# も同じ所を読んで同じ値を返す） |",
        "| `RankUp あふれＩＤ 65541`（`VB5.あふれ.RankUp`） | エラー | `ExecRankUpCmd`（9539 行）の `rk` は Integer で、`rk = GetArgAsLong(3)`（9555 行）が Long から Integer への変換。続く `.Rank = .Rank + rk`（9567 行）は Integer の足し算。変換で起きたと考えられるが、確かめていない |",
        "| `IncreaseMorale あふれ機 65541`（`VB5.あふれ.IncreaseMorale`） | エラー | `ExecIncreaseMoraleCmd`（6857 行）の `num` は String で、`.IncreaseMorale num, True`（6874 行）が Integer の引数（`Unit.cls` 26912 行の `ByVal num As Integer`）に渡すときに文字列から Integer へ変換する。文字列の変換はランタイムの関数なので、そこで起きたと考えられる |",
        "| `1e10 \\ 1`・`1e10 Mod 7`・`2147483648 \\ 1`（`VB5.あふれ.*`） | エラー（`2147483647 \\ 1` は 2147483647） | `Expression.bas` の `\\`（393 行）・`Mod`（419・422 行）は Double の `lnum`・`rnum` をそのまま VB の演算子に渡す。VB は割る前に両辺を Long に丸めるので、Long に収まらない値の変換で起きたと考えられる。境目が Long の上限なのはこの読み方と合う |",
        "| `10 ^ 400`・`1e308 * 10`（`VB5.浮動小数点.*`） | エラー | `Expression.bas` の `^`（439 行）・`*`（349 行）の結果を `FormatNum`（`GeneralLib.bas` 1006 行。`Int(n)` と `Format$`）で文字列にする。演算そのもの（`^` はランタイムの関数）か、無限大を受け取った `Int`・`Format$` か、どこで起きたかは確かめていない |",
        "| `(-8) ^ 0.5`・`Sqr(-1)`（`VB5.浮動小数点.*`） | エラー | `^`（439 行）・`Sqr`（3130 行）。非数になる演算そのものか、`FormatNum` か、どこで起きたかは確かめていない |",
        "| `5 Mod 0`・`5 Mod 0.4`（`VB5.割り算.*`） | エラー | `Mod` は 0 を確かめない（0 での割り算。エラー 11）。`/`・`\\` は `Expression.bas` が除数の 0 を確かめて 0 にする（`5 / 0`・`5 \\ 0` は 0） |",
        "",
        "SRC# は、これらを元の SRC と同じくエラーにしている（`VBMath.Checked`・`VBMath.CInt`・`CmdData.GetArgRaw`。#1564・#1580）。",
        "",
        "### 「VB では実行時エラーにならない」と判断するときの指針",
        "",
        "- コンパイルの設定だけを理由に「エラーにならない」と判断しない。型の変換（Long・Double・String から Integer・Long へ。代入・`ByVal` の引数への受け渡し・`\\`・`Mod` を含む）のあふれ、浮動小数点の無限大・非数は、実機でエラーになった。同じ形のものは、エラーになると読む",
        "- 配列が無い（ReDim していない）ときの読みも、実機でエラーになった。確保した配列の範囲外（`strArgs(2 To 3)` の `strArgs(4)` など）の読み書きと、Integer・Long どうしの足し算・掛け算のあふれ（変換を含まないもの）は、実機で確かめていない。これらを理由にするときは「実機では確かめていない」と書き、SRC# は値の定まる側（エラーにする・空文字列にするなど）に寄せる。確かめる必要があれば、[プローブ](../../../../SRC.Sharp/Compat/PROBE.md)に項目を足す",
        "- 整数の割り算（`\\`・`Mod`）の 0 での割り算（エラー 11）・`Nothing` の参照（エラー 91）・型の不一致（エラー 13）・`CDbl` などの変換の失敗は、この設定に関わらずエラーになる。Collection（`colItem` など）はランタイムの部品で配列ではないので、キーが無い・番号が範囲外のときはこの設定に関わらずエラー（5・9）になる",
        "- イベントのコマンドの外（インターミッションのコマンドなど、`On Error` の無い所）の実行時エラーは、Src.exe が終わると考えられる（確かめていない）",
        "- 範囲外の引数の参照（7474/SRC#1511）は、引数の無いコマンドを元の SRC に合わせ、引数はあって番号だけが範囲外のものは SRC# の違いとして残した（[CmdData.cls](CmdData_cls.md) の GetArg の行）",
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


def build_todo():
    """SRCCore の TODO・XXX・コメントアウトされたコードの一覧（todo.md）の行と、状態が「未確認」の行の場所を返す。
    手で書いた「状態」「メモ」は、ファイルと本文が同じ行に残す"""
    path = OUT_DIR / "todo.md"
    old = {}
    # 同じファイルに同じ本文の行が複数あるときは、場所（ファイルと行）が同じ行の状態とメモを先に使う
    old_at = {}
    if path.exists():
        for line in path.read_text(encoding="utf-8").splitlines():
            cols = [c.strip() for c in line.strip().strip("|").split("|")]
            if line.startswith("| `") and len(cols) == 5:
                old[(cols[0].split(":")[0], cols[2])] = (cols[3], cols[4])
                old_at[(cols[0], cols[2])] = (cols[3], cols[4])
    rows = []
    base = ROOT / "SRC.Sharp"
    for file in sorted((base / "SRCCore").rglob("*.cs")):
        rel = file.relative_to(base).as_posix()
        if "/obj/" in rel or "/bin/" in rel:
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
        "- 「未確認」の行があると CI（`port-map-check`）が落ちる。TODO・XXX・コメントアウトされたコードを足したら、直すか、この表に状態とメモを書く（7474/SRC#1555）",
        "",
        "| 場所 | 種類 | 本文 | 状態 | メモ |",
        "|---|---|---|---|---|",
    ]
    unchecked = []
    for r in rows:
        rel, i, kind, text = r[0], r[1], r[2], r[3]
        text = text.replace("|", "\\|")
        if len(text) > 120:
            text = text[:117] + "..."
        where = f"`{rel}:{i}`" + (f"（{r[4]} 行）" if kind == "コード" and r[4] > 1 else "")
        state, memo = old_at.get((where, text)) or old.get((f"`{rel}", text), ("未確認", ""))
        lines.append(f"| {where} | {kind} | {text} | {state} | {memo} |")
        if state.startswith("未確認"):
            unchecked.append(f"{where} {kind} {text}")
    return lines, unchecked


def write_todo():
    lines, _ = build_todo()
    (OUT_DIR / "todo.md").write_text("\n".join(lines) + "\n", encoding="utf-8")
    return sum(1 for line in lines if line.startswith("| `"))


def check_todo():
    """SRCCore の新しい TODO・XXX・コメントアウトされたコード（todo.md で「未確認」のもの）を見つける（7474/SRC#1555）"""
    _, unchecked = build_todo()
    path = (OUT_DIR / "todo.md").relative_to(ROOT)
    return [f"{path}: {u} の状態が「未確認」です。直すか、todo.md に状態とメモを書きます" for u in unchecked]


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--check", action="store_true")
    parser.add_argument("--stats", action="store_true")
    args = parser.parse_args()
    vb = read_vb()
    if args.check:
        errors = check(vb) + check_todo()
        for e in errors:
            print(e)
        if errors:
            print("python3 tools/vb_port_map.py で表を作り直し、状態を直してください")
            return 1
        print(f"OK: {sum(len(p) for p in vb.values())} の手続きが対応表にあり、TODO・コメントアウトに「未確認」はありません")
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
