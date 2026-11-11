#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
実機との比較のプローブ（probe/・probe-bounds/・probe-errors/。#1550）を、元の SRC（VB5 の Src.exe）に渡す形に書き出し、
Windows で流した結果を期待値（probe-expected/）として取り込む。手順は PROBE.md。

  python3 probe_pack.py export [出力先] [--zip]   出力先の既定は ./out/probe-pack（中身は消してから書く）
  python3 probe_pack.py import 持ち帰ったフォルダ  out_*.txt と errors_*.txt を probe-expected/ に UTF-8・LF で写す
  python3 probe_pack.py check                      書き出さずに、Shift_JIS に変換できるかだけを確かめる

export: テキスト（*.eve・*.txt・*.map）は export_sjis.py と同じ変換（Shift_JIS・CRLF）、画像はそのまま写す。
        書き出したフォルダの中の並びは、このフォルダと同じ（probe\\probe.eve・probe-bounds\\incr.eve・probe-errors\\<ケース>\\probe.eve）。
import: シナリオが書いた out_<イベント>.txt（Shift_JIS）と、エラーのダイアログを書き写した errors_<イベント>.txt を、
        probe-expected/<フォルダ>/<イベント>.txt・<イベント>.errors.txt に写す。
        probe-bounds・probe-errors で errors_*.txt が無いケースは、ダイアログが出なかったものとして空の .errors.txt を作る
        （どちらのファイルも無いケースは、流していないものとして飛ばす）。
"""
import os
import shutil
import sys
import zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
sys.dont_write_bytecode = True
sys.path.insert(0, HERE)
from export_sjis import BINARY_EXTENSIONS, TEXT_EXTENSIONS, convert  # noqa: E402

FOLDERS = ["probe", "probe-bounds", "probe-errors"]
EXPECTED = os.path.join(HERE, "probe-expected")


def cases():
    """(Compat からの相対フォルダ, イベントファイル名) の一覧"""
    result = [("probe", "probe.eve")]
    result += [("probe-bounds", n) for n in sorted(os.listdir(os.path.join(HERE, "probe-bounds"))) if n.endswith(".eve")]
    errors = os.path.join(HERE, "probe-errors")
    result += [("probe-errors/" + d, "probe.eve") for d in sorted(os.listdir(errors))
               if os.path.isfile(os.path.join(errors, d, "probe.eve"))]
    return result


def collect():
    errors, files = [], []
    for top in FOLDERS:
        for root, dirs, names in os.walk(os.path.join(HERE, top)):
            dirs.sort()
            for name in sorted(names):
                full = os.path.join(root, name)
                rel = os.path.relpath(full, HERE)
                if not rel.isascii():
                    errors.append(f"{rel}: ファイル名・フォルダ名に ASCII でない文字がある")
                if name.startswith("out_") or name.startswith("errors_"):
                    continue
                ext = os.path.splitext(name)[1].lower()
                if ext in TEXT_EXTENSIONS:
                    data, problems = convert(full)
                    errors += [f"{rel}: {p}" for p in problems]
                    files.append((rel, data))
                elif ext in BINARY_EXTENSIONS:
                    files.append((rel, None))
                else:
                    errors.append(f"{rel}: 扱いの決まっていない種類のファイル")
    return errors, files


def export(argv):
    make_zip = "--zip" in argv
    args = [a for a in argv if not a.startswith("--")]
    dest = os.path.abspath(args[0]) if args else os.path.join(HERE, "out", "probe-pack")
    errors, files = collect()
    if errors:
        print("\n".join(errors), file=sys.stderr)
        return 1
    if os.path.exists(dest):
        shutil.rmtree(dest)
    for rel, data in files:
        out = os.path.join(dest, rel)
        os.makedirs(os.path.dirname(out), exist_ok=True)
        if data is None:
            shutil.copyfile(os.path.join(HERE, rel), out)
        else:
            with open(out, "wb") as f:
                f.write(data)
    bat = make_batch()
    with open(os.path.join(dest, "run_probe.bat"), "wb") as f:
        f.write(bat)
    files.append(("run_probe.bat", bat))
    print(f"{len(files)} ファイルを {dest} に書き出した")
    if make_zip:
        archive = dest.rstrip(os.sep) + ".zip"
        top = os.path.basename(dest.rstrip(os.sep))
        with zipfile.ZipFile(archive, "w", zipfile.ZIP_DEFLATED) as z:
            for rel, _ in files:
                z.write(os.path.join(dest, rel), os.path.join(top, rel).replace(os.sep, "/"))
        print(f"{archive} を作った")
    return 0


def make_batch():
    """すべてのケースを順に Src.exe で開くバッチファイル（Shift_JIS・CRLF）"""
    all_cases = cases()
    lines = [
        "@echo off",
        "rem 実機との比較のプローブ（#1550）を順に流す。手順は SRC.Sharp/Compat/PROBE.md",
        "rem 次の行の SRC_EXE を、Src.exe（Ver.2.2.33）の場所に直してから実行する",
        "set SRC_EXE=C:\\SRC\\Src.exe",
        "if not exist \"%SRC_EXE%\" (",
        "  echo Src.exe が見つかりません: %SRC_EXE%",
        "  pause",
        "  exit /b 1",
        ")",
    ]
    for i, (folder, eve) in enumerate(all_cases, 1):
        path = folder.replace("/", "\\") + "\\" + eve
        name = os.path.splitext(eve)[0]
        lines.append(f"echo [{i}/{len(all_cases)}] {path}")
        if folder != "probe":
            lines.append(f"echo   エラーのダイアログが出たら、文言を {folder.replace('/', chr(92))}\\errors_{name}.txt に書き写してから OK を押す")
        lines.append(f"start \"\" /wait \"%SRC_EXE%\" \"%~dp0{path}\"")
        lines.append("pause")
    lines.append("echo 終わり。このフォルダ（out_*.txt と errors_*.txt）をまとめて持ち帰る")
    lines.append("pause")
    return ("\r\n".join(lines) + "\r\n").encode("cp932")


def read_text(path):
    """Shift_JIS（元の SRC の出力）か UTF-8（書き写したもの）のテキストを読み、改行を LF にそろえる"""
    raw = open(path, "rb").read()
    if raw.startswith(b"\xef\xbb\xbf"):
        raw = raw[3:]
    try:
        text = raw.decode("utf-8")
    except UnicodeDecodeError:
        text = raw.decode("cp932")
    return text.replace("\r\n", "\n").replace("\r", "\n")


def import_results(argv):
    if not argv:
        print("持ち帰ったフォルダ（export で書き出し、Windows で流したもの）を指定する", file=sys.stderr)
        return 1
    source = os.path.abspath(argv[0])
    count = 0
    for folder, eve in cases():
        name = os.path.splitext(eve)[0]
        src_dir = os.path.join(source, *folder.split("/"))
        out_path = os.path.join(src_dir, f"out_{name}.txt")
        err_path = os.path.join(src_dir, f"errors_{name}.txt")
        if not os.path.exists(out_path) and not os.path.exists(err_path):
            print(f"飛ばした（結果が無い）: {folder}/{eve}")
            continue
        dest_dir = os.path.join(EXPECTED, *folder.split("/"))
        os.makedirs(dest_dir, exist_ok=True)
        dest_out = os.path.join(dest_dir, f"{name}.txt")
        if os.path.exists(out_path):
            with open(dest_out, "w", encoding="utf-8", newline="\n") as f:
                f.write(read_text(out_path))
        elif os.path.exists(dest_out):
            os.remove(dest_out)
        dest_err = os.path.join(dest_dir, f"{name}.errors.txt")
        if os.path.exists(err_path):
            with open(dest_err, "w", encoding="utf-8", newline="\n") as f:
                f.write(read_text(err_path))
        elif folder != "probe":
            open(dest_err, "w", encoding="utf-8", newline="\n").close()
        count += 1
        print(f"取り込んだ: {folder}/{eve}")
    print(f"{count} ケースを {EXPECTED} に取り込んだ")
    return 0


def main(argv):
    if not argv or argv[0] not in ("export", "import", "check"):
        print(__doc__, file=sys.stderr)
        return 1
    if argv[0] == "check":
        errors, files = collect()
        if errors:
            print("\n".join(errors), file=sys.stderr)
            return 1
        print(f"{len(files)} ファイルを確かめた。Shift_JIS に変換できる")
        return 0
    if argv[0] == "export":
        return export(argv[1:])
    return import_results(argv[1:])


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
