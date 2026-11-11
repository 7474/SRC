#!/usr/bin/env python3
"""互換性の検証用のシナリオ（SRC.Sharp/Compat/）を、lint に掛け、最初から最後まで自動でプレイする（7474/SRC#1551）。

  python3 tools/compat_play.py                  # lint と、Compat/*.play のすべてを流してゴールデンと比べる
  python3 tools/compat_play.py torres.play      # 指定した *.play だけ流す（lint は省く）
  SRC_GOLDEN_UPDATE=1 python3 tools/compat_play.py   # ゴールデン（Compat/golden/<名前>.txt）を作り直す

すること:
  1. Compat/ の下の、Data を持つフォルダ（シナリオのフォルダ）を srcsharp lint と SRCDataLinter に掛ける。エラーがあれば失敗
  2. Compat/*.play のそれぞれを srcsharp play --skip-wait --seed で流す。*.play には
       #   srcsharp play Compat/<フォルダ>/<イベントファイル> --script Compat/<名前>.play --app-path Content/basic-data ...
     の形の行（コメント）を書く。この行から、始めるイベントファイルと --app-path を読む（パスは SRC.Sharp からの相対）
     - 終了コードが 0 でない（例外・expect の失敗。expect error none はイベントのエラーのダイアログも失敗にする）、
       またはシナリオが終わらない（最後の行が terminated でない）なら失敗
     - シナリオのフォルダは一時フォルダに写して動かす（セーブのファイルを書くため）
  3. 各話の終わりの状態（srcsharp play --state）を、同じ種で --repeat 回流して同じになることを確かめ、
     Compat/golden/<*.play の名前>.txt と比べる。違えば差分を出して失敗

srcsharp と SRCDataLinter は Release でビルドしたもの（無ければビルドする）を使う。--srcsharp・--datalinter で別のコマンドを渡せる。
"""
import argparse
import difflib
import os
import pathlib
import re
import shlex
import shutil
import subprocess
import sys
import tempfile

ROOT = pathlib.Path(__file__).resolve().parent.parent
SHARP = ROOT / "SRC.Sharp"
COMPAT = SHARP / "Compat"
GOLDEN = COMPAT / "golden"
SRCSHARP_DLL = SHARP / "SRCSharpTools" / "bin" / "Release" / "net10.0" / "srcsharp.dll"
DATALINTER_DLL = SHARP / "SRCDataLinter" / "bin" / "Release" / "net10.0" / "SRCDataLinter.dll"

HEADER = re.compile(r"srcsharp\s+play\s+(\S+\.eve)\b(.*)$")


def error(message):
    if os.environ.get("GITHUB_ACTIONS") == "true":
        print("::error::" + message.replace("\n", "%0A"))
    else:
        print("error: " + message, file=sys.stderr)


def tool_command(given, dll, project):
    if given:
        return shlex.split(given)
    if not dll.exists():
        subprocess.run(["dotnet", "build", str(project), "-c", "Release"], check=True)
    return ["dotnet", str(dll)]


def resolve(path):
    """*.play の行に書いたパス（SRC.Sharp からの相対。リポジトリのルートからの相対でもよい）"""
    for base in (SHARP, ROOT):
        p = (base / path).resolve()
        if p.exists():
            return p
    return None


def read_header(play):
    for line in play.read_text(encoding="utf-8").splitlines():
        if not line.lstrip().startswith("#"):
            continue
        m = HEADER.search(line)
        if not m:
            continue
        eve = resolve(m.group(1))
        rest = m.group(2).split()
        app = None
        if "--app-path" in rest and rest.index("--app-path") + 1 < len(rest):
            app = resolve(rest[rest.index("--app-path") + 1])
        return eve, app
    return None, None


def lint(srcsharp, datalinter):
    dirs = sorted(d for d in COMPAT.iterdir() if d.is_dir() and (d / "Data").is_dir())
    ok = True
    for d in dirs:
        rel = d.relative_to(ROOT)
        print(f"== lint {rel}", flush=True)
        r = subprocess.run(srcsharp + ["lint", str(d)])
        if r.returncode != 0:
            error(f"srcsharp lint {rel} が失敗した（終了コード {r.returncode}）")
            ok = False
        print(f"== SRCDataLinter {rel}", flush=True)
        r = subprocess.run(datalinter + [str(d)], stdout=subprocess.PIPE, text=True)
        # Checked [...] の行は多いので、最後の行（かかった時間）だけ出す
        lines = [x for x in r.stdout.splitlines() if not x.startswith("Checked [")]
        print("\n".join(lines))
        if r.returncode != 0:
            error(f"SRCDataLinter {rel} が失敗した（終了コード {r.returncode}）")
            ok = False
    return ok


def play_once(srcsharp, play, eve, app, seed, work, timeout):
    """*.play を 1 回流し、(成否, 状態の文字列, 出力) を返す"""
    top = eve.relative_to(COMPAT).parts[0]
    scenario = work / top
    if scenario.exists():
        shutil.rmtree(scenario)
    shutil.copytree(COMPAT / top, scenario)
    state = work / "state.txt"
    if state.exists():
        state.unlink()
    cmd = srcsharp + ["play", str(scenario / eve.relative_to(COMPAT / top)), "--script", str(play),
                      "--skip-wait", "--seed", str(seed), "--state", str(state)]
    if app:
        cmd += ["--app-path", str(app)]
    try:
        r = subprocess.run(cmd, cwd=work, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, timeout=timeout)
    except subprocess.TimeoutExpired as ex:
        out = ex.stdout or ""
        if isinstance(out, bytes):
            out = out.decode("utf-8", "replace")
        return False, None, out + f"\n{timeout} 秒で終わらなかった"
    out = r.stdout
    text = state.read_text(encoding="utf-8") if state.exists() else None
    if text is not None:
        # 一時フォルダのパスは毎回変わるので置き換える
        text = text.replace(str(scenario), "<scenario>").replace(str(work), "<work>")
    last = out.strip().splitlines()[-1] if out.strip() else ""
    if r.returncode != 0:
        return False, text, out + f"\n終了コード {r.returncode}"
    if last != "terminated":
        return False, text, out + "\nシナリオが最後まで進まなかった（最後の行が terminated でない）"
    return True, text, out


def diff(a, b, name_a, name_b):
    return "".join(difflib.unified_diff(a.splitlines(True), b.splitlines(True), name_a, name_b, n=2))


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("plays", nargs="*", help="流す *.play（Compat/ の下の名前）。省くとすべて")
    ap.add_argument("--srcsharp", help="srcsharp を動かすコマンド（既定は Release でビルドした srcsharp.dll）")
    ap.add_argument("--datalinter", help="SRCDataLinter を動かすコマンド（既定は Release でビルドした SRCDataLinter.dll）")
    ap.add_argument("--seed", type=int, default=1, help="乱数の種（既定 1。変えるとゴールデンも変わる）")
    ap.add_argument("--repeat", type=int, default=2, help="同じ種で流す回数。すべて同じ状態になることを確かめる（既定 2）")
    ap.add_argument("--timeout", type=int, default=600, help="1 回のプレイの上限の秒数（既定 600）")
    ap.add_argument("--no-lint", action="store_true", help="lint を省く")
    ap.add_argument("--out", help="各回の出力と状態を書き出すフォルダ（CI の成果物）")
    args = ap.parse_args()

    srcsharp = tool_command(args.srcsharp, SRCSHARP_DLL, SHARP / "SRCSharpTools" / "SRCSharpTools.csproj")
    ok = True
    if not args.no_lint and not args.plays:
        datalinter = tool_command(args.datalinter, DATALINTER_DLL, SHARP / "SRCDataLinter" / "SRCDataLinter.csproj")
        ok = lint(srcsharp, datalinter) and ok

    plays = [COMPAT / p for p in args.plays] if args.plays else sorted(COMPAT.glob("*.play"))
    if not plays:
        error("Compat/*.play が無い")
        return 1
    update = os.environ.get("SRC_GOLDEN_UPDATE") == "1"
    out_dir = pathlib.Path(args.out).resolve() if args.out else None
    if out_dir:
        out_dir.mkdir(parents=True, exist_ok=True)

    for play in plays:
        name = play.stem
        print(f"== play {play.relative_to(ROOT)}", flush=True)
        eve, app = read_header(play)
        if eve is None:
            error(f"{play.name} に「#   srcsharp play Compat/<フォルダ>/<イベントファイル> --script ... --app-path ...」の行が無いか、"
                  "イベントファイルが見つからない")
            ok = False
            continue
        if COMPAT not in eve.parents:
            error(f"{play.name} のイベントファイル {eve} が Compat/ の下に無い")
            ok = False
            continue
        states = []
        failed = False
        with tempfile.TemporaryDirectory(prefix="srcs-compat-") as tmp:
            work = pathlib.Path(tmp)
            for i in range(args.repeat):
                success, text, out = play_once(srcsharp, play.resolve(), eve, app, args.seed, work, args.timeout)
                if out_dir:
                    (out_dir / f"{name}.{i + 1}.log").write_text(out, encoding="utf-8")
                    if text is not None:
                        (out_dir / f"{name}.{i + 1}.state.txt").write_text(text, encoding="utf-8")
                if not success:
                    tail = "\n".join(out.splitlines()[-60:])
                    print(tail)
                    error(f"{play.name}（{i + 1} 回目）が失敗した")
                    failed = True
                    break
                states.append(text or "")
                print(f"  {i + 1} 回目: 最後まで進んだ", flush=True)
        if failed:
            ok = False
            continue
        for i, s in enumerate(states[1:], start=2):
            if s != states[0]:
                print(diff(states[0], s, f"{name} 1 回目", f"{name} {i} 回目"))
                error(f"{play.name}: 同じ乱数の種で流したのに、終わりの状態が 1 回目と {i} 回目で違う")
                ok = False
                break
        else:
            golden = GOLDEN / f"{name}.txt"
            if update:
                GOLDEN.mkdir(exist_ok=True)
                golden.write_text(states[0], encoding="utf-8", newline="\n")
                print(f"  {golden.relative_to(ROOT)} を作り直した（SRC_GOLDEN_UPDATE=1）")
            elif not golden.exists():
                error(f"{golden.relative_to(ROOT)} が無い。SRC_GOLDEN_UPDATE=1 python3 tools/compat_play.py {play.name} で作る")
                ok = False
            else:
                expected = golden.read_text(encoding="utf-8").replace("\r\n", "\n")
                if expected != states[0]:
                    print(diff(expected, states[0], f"golden/{name}.txt", f"{name}（今回）"))
                    error(f"{play.name}: 終わりの状態がゴールデン（golden/{name}.txt）と違う。"
                          "意図した変更なら SRC_GOLDEN_UPDATE=1 python3 tools/compat_play.py で作り直して差分を PR に載せる")
                    ok = False
                else:
                    print("  ゴールデンと同じ")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
