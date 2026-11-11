#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
実機との比較のプローブ（probe/・probe-bounds/・probe-errors/。#1550）と、画面と音の見比べのシナリオ（screens/。#1553）と、
セーブデータの見本を作るシナリオ（torres/ と savedata/。#1577）を、元の SRC（VB5 の Src.exe）に渡す形に書き出し、
Windows で流した結果を期待値（probe-expected/・screens-expected/・savedata-expected/）として取り込む。
手順は PROBE.md（見比べの場面は SCREENS.md）。

  python3 probe_pack.py export [出力先] [--zip]   出力先の既定は ./out/probe-pack（中身は消してから書く）
  python3 probe_pack.py import 持ち帰ったフォルダ  out_*.txt と errors_*.txt を probe-expected/ に UTF-8・LF で写す。
                                                  見比べのスクリーンショットと音のメモを screens-expected/・screens-form/ に写す
  python3 probe_pack.py check                      書き出さずに、Shift_JIS に変換できるかだけを確かめる

export: テキスト（*.eve・*.txt・*.map・*.ini）は export_sjis.py と同じ変換（Shift_JIS・CRLF）、画像・音はそのまま写す。
        書き出したフォルダの中の並びは、このフォルダと同じ（probe\\probe.eve・probe-bounds\\incr.eve・probe-errors\\<ケース>\\probe.eve・
        screens\\screens.eve）。screens の PlayScript（*.play。SRC# のテストだけで使う）は書き出さない。
        run_probe.bat（プローブを順に流し、続けて見比べを始める）・run_screens.bat（見比べだけ）と、
        capture_screens.ps1（場面ごとに Src.exe の画面を撮る。このフォルダの capture_screens.ps1 に screens-scenes.txt の場面を埋める）、
        run_probe_auto.ps1（プローブを自動で流し、エラーのフォームの文言を errors_*.txt に書く）も書く。
import: シナリオが書いた out_<イベント>.txt（Shift_JIS）と、エラーのダイアログを書き写した errors_<イベント>.txt を、
        probe-expected/<フォルダ>/<イベント>.txt・<イベント>.errors.txt に写す。
        probe-bounds・probe-errors で errors_*.txt が無いケースは、ダイアログが出なかったものとして空の .errors.txt を作る
        （どちらのファイルも無いケースは、流していないものとして飛ばす）。
        見比べ: screens-shots/<ID>.png（Src.exe のスクリーンショット）と screens-sounds.txt（聞いたことのメモ）を
        screens-expected/ に、screens-form-shots/<ID>.png と screens-form-shots/sounds.txt（SRC#Form）を screens-form/ に写す。
        セーブデータ: torres\\suspend.src（中断データ）・torres\\clear.src（クリアデータ）を、そのまま（Shift_JIS のまま）
        savedata-expected/ に写す。名前を変えずに保存したもの（savedataを中断.src・savedataまでクリア.src）も同じ名前で写す。
export の補足: savedata/*.eve は torres のデータを使うので、書き出した torres\\ に置く（torres\\savedata.eve）。
"""
import os
import shutil
import sys
import zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
sys.dont_write_bytecode = True
sys.path.insert(0, HERE)
from export_sjis import BINARY_EXTENSIONS, TEXT_EXTENSIONS, convert  # noqa: E402

FOLDERS = ["probe", "probe-bounds", "probe-errors", "screens", "torres"]
# torres のデータを使うので、書き出した torres/ に置くイベント（セーブデータの見本を作る。#1577）
SAVEDATA = "savedata"
EXPECTED = os.path.join(HERE, "probe-expected")
SAVEDATA_EXPECTED = os.path.join(HERE, "savedata-expected")
# 持ち帰るセーブデータの見本（torres\ の中の名前 → savedata-expected/ の名前）。名前を変えずに保存したものも受け付ける
SAVEDATA_SAMPLES = [
    (["suspend.src", "savedataを中断.src"], "suspend.src"),
    (["clear.src", "savedataまでクリア.src"], "clear.src"),
]
SCREENS_EXPECTED = os.path.join(HERE, "screens-expected")
SCREENS_FORM = os.path.join(HERE, "screens-form")
# SRC# のテストだけで使うので、元の SRC には渡さないファイル
SKIP_EXTENSIONS = {".play"}


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
                if ext in SKIP_EXTENSIONS:
                    continue
                if ext in TEXT_EXTENSIONS:
                    data, problems = convert(full)
                    errors += [f"{rel}: {p}" for p in problems]
                    files.append((rel, data))
                elif ext in BINARY_EXTENSIONS:
                    files.append((rel, None))
                else:
                    errors.append(f"{rel}: 扱いの決まっていない種類のファイル")
    for name in sorted(os.listdir(os.path.join(HERE, SAVEDATA))):
        rel = os.path.join(SAVEDATA, name)
        if not name.isascii():
            errors.append(f"{rel}: ファイル名に ASCII でない文字がある")
        if os.path.splitext(name)[1].lower() != ".eve":
            errors.append(f"{rel}: 扱いの決まっていない種類のファイル")
            continue
        data, problems = convert(os.path.join(HERE, rel))
        errors += [f"{rel}: {p}" for p in problems]
        files.append((os.path.join("torres", name), data))
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
    for name, data in [("run_probe.bat", make_batch()), ("run_screens.bat", make_screens_batch()),
                       ("run_savedata.bat", make_savedata_batch()), ("capture_screens.ps1", make_capture_script()),
                       ("run_probe_auto.ps1", make_ps1("run_probe_auto.ps1"))]:
        with open(os.path.join(dest, name), "wb") as f:
            f.write(data)
        files.append((name, data))
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
    lines.append("echo プローブは終わり。続けて、画面と音の見比べ（#1553）を始める（PROBE.md の手順 5）")
    lines.append("pause")
    lines += screens_batch_lines()
    lines.append("echo 見比べは終わり。続けて、セーブデータの見本（#1577）を作る（PROBE.md の手順 6）")
    lines.append("pause")
    lines += savedata_batch_lines()
    lines.append("echo 終わり。このフォルダ（out_*.txt・errors_*.txt・screens-shots・screens-sounds.txt・torres\\suspend.src・torres\\clear.src）をまとめて持ち帰る")
    lines.append("pause")
    return ("\r\n".join(lines) + "\r\n").encode("cp932")


def savedata_batch_lines():
    """Src.exe に torres\\savedata.eve を 2 回開かせ、中断データとクリアデータの見本を作る（SRC_EXE は呼ぶ側で決める）"""
    return [
        "rem セーブデータの見本（#1577）。手順は SRC.Sharp/Compat/PROBE.md の手順 6",
        "echo [1/2] 1 ターン目の味方フェイズで、マップのコマンド「中断」。torres\\suspend.src の名前で保存する",
        "start \"\" /wait \"%SRC_EXE%\" \"%~dp0torres\\savedata.eve\"",
        "pause",
        "echo [2/2] 1 ターン目の味方フェイズで、マップのコマンド「ターン終了」。インターミッションの「データセーブ」で torres\\clear.src の名前で保存し、「終了」で終える",
        "start \"\" /wait \"%SRC_EXE%\" \"%~dp0torres\\savedata.eve\"",
        "pause",
    ]


def make_savedata_batch():
    """セーブデータの見本だけを作るバッチファイル（Shift_JIS・CRLF）"""
    lines = [
        "@echo off",
        "rem セーブデータの見本（#1577）だけを作る。手順は SRC.Sharp/Compat/PROBE.md",
        "rem 次の行の SRC_EXE を、Src.exe（Ver.2.2.33）の場所に直してから実行する",
        "set SRC_EXE=C:\\SRC\\Src.exe",
    ] + savedata_batch_lines()
    return ("\r\n".join(lines) + "\r\n").encode("cp932")


def screens_batch_lines():
    """capture_screens.ps1 で、Src.exe に screens\\screens.eve を開かせ、場面ごとに画面を撮る（SRC_EXE は呼ぶ側で決める）"""
    return [
        "rem 画面と音の見比べ（#1553）。手順は SRC.Sharp/Compat/PROBE.md と SCREENS.md",
        "powershell -NoProfile -ExecutionPolicy Bypass -File \"%~dp0capture_screens.ps1\" -SrcExe \"%SRC_EXE%\"",
    ]


def make_screens_batch():
    """画面と音の見比べだけを流すバッチファイル（Shift_JIS・CRLF）"""
    lines = [
        "@echo off",
        "rem 画面と音の見比べ（#1553）だけを流す。手順は SRC.Sharp/Compat/PROBE.md",
        "rem 次の行の SRC_EXE を、Src.exe（Ver.2.2.33）の場所に直してから実行する",
        "set SRC_EXE=C:\\SRC\\Src.exe",
    ] + screens_batch_lines() + ["pause"]
    return ("\r\n".join(lines) + "\r\n").encode("cp932")


def read_scenes():
    """screens-scenes.txt の (ID, 種類, 場面, 実機での操作) の一覧"""
    scenes = []
    with open(os.path.join(HERE, "screens-scenes.txt"), encoding="utf-8") as f:
        for line in f:
            line = line.rstrip("\n")
            if not line or line.startswith("#"):
                continue
            scenes.append(tuple((line.split("\t") + ["", "", "", ""])[:4]))
    return scenes


def make_ps1(name):
    """このフォルダの PowerShell のスクリプトを、Windows PowerShell 5.1 が日本語を読めるように UTF-8 の BOM 付き・CRLF で書く"""
    with open(os.path.join(HERE, name), encoding="utf-8-sig") as f:
        script = f.read().replace("\r\n", "\n")
    return b"\xef\xbb\xbf" + script.replace("\n", "\r\n").encode("utf-8")


def make_capture_script():
    """
    capture_screens.ps1 の「@@SCENES@@」の行に screens-scenes.txt の場面を埋める。
    Windows PowerShell 5.1 が日本語を読めるように、UTF-8 の BOM を付け、改行を CRLF にする
    """
    def quote(text):
        return "'" + text.replace("'", "''") + "'"
    entries = ",\n".join(f"  @({', '.join(quote(c) for c in scene)})" for scene in read_scenes())
    with open(os.path.join(HERE, "capture_screens.ps1"), encoding="utf-8") as f:
        template = f.read().replace("\r\n", "\n")
    marker = "\n@@SCENES@@\n"
    if template.count(marker) != 1:
        raise ValueError("capture_screens.ps1 に @@SCENES@@ だけの行が 1 つでない")
    script = template.replace(marker, "\n" + entries + "\n")
    return b"\xef\xbb\xbf" + script.replace("\n", "\r\n").encode("utf-8")


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


def strip_error_folders(text):
    """エラーのダイアログの 1 行目（「ファイル名：n行目」）のファイル名から、フォルダを除く。
    Windows で流した場所（ユーザー名を含むことがある）をリポジトリに残さないため。比べるとき（CompatProbeTests）も同じく除く"""
    lines = text.split("\n")
    first = True
    for i, line in enumerate(lines):
        if line.strip() == "----":
            first = True
            continue
        if first and line.strip():
            colon = line.find("：")
            if colon > 0:
                file = line[:colon]
                cut = max(file.rfind("\\"), file.rfind("/"))
                lines[i] = file[cut + 1:] + line[colon:]
            first = False
    return "\n".join(lines)


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
                f.write(strip_error_folders(read_text(err_path)))
        elif folder != "probe":
            open(dest_err, "w", encoding="utf-8", newline="\n").close()
        count += 1
        print(f"取り込んだ: {folder}/{eve}")
    print(f"{count} ケースを {EXPECTED} に取り込んだ")
    import_screens(source)
    import_savedata(source)
    return 0


def import_savedata(source):
    """セーブデータの見本（torres\\suspend.src・clear.src）を、Shift_JIS のまま savedata-expected/ に写す"""
    for names, dest_name in SAVEDATA_SAMPLES:
        found = [n for n in names if os.path.exists(os.path.join(source, "torres", n))]
        if not found:
            print(f"飛ばした（見本が無い）: torres/{names[0]}")
            continue
        os.makedirs(SAVEDATA_EXPECTED, exist_ok=True)
        dest = os.path.join(SAVEDATA_EXPECTED, dest_name)
        shutil.copyfile(os.path.join(source, "torres", found[0]), dest)
        print(f"取り込んだ: torres/{found[0]} → {dest}")


def import_screens(source):
    """見比べのスクリーンショットと音のメモを screens-expected/・screens-form/ に写す"""
    ids = [s[0] for s in read_scenes()]
    for shots, dest in [("screens-shots", SCREENS_EXPECTED), ("screens-form-shots", SCREENS_FORM)]:
        folder = os.path.join(source, shots)
        if not os.path.isdir(folder):
            continue
        copied = 0
        for name in sorted(os.listdir(folder)):
            stem, ext = os.path.splitext(name)
            if ext.lower() not in (".png", ".jpg", ".jpeg", ".bmp"):
                continue
            if stem not in ids:
                print(f"飛ばした（場面の ID でない）: {shots}/{name}")
                continue
            os.makedirs(dest, exist_ok=True)
            shutil.copyfile(os.path.join(folder, name), os.path.join(dest, stem + ext.lower()))
            copied += 1
        print(f"{copied} 枚を {dest} に取り込んだ")
    for notes, dest in [(os.path.join(source, "screens-sounds.txt"), SCREENS_EXPECTED),
                        (os.path.join(source, "screens-form-shots", "sounds.txt"), SCREENS_FORM)]:
        if os.path.exists(notes):
            os.makedirs(dest, exist_ok=True)
            with open(os.path.join(dest, "sounds.txt"), "w", encoding="utf-8", newline="\n") as f:
                f.write(read_text(notes))
            print(f"音のメモを {os.path.join(dest, 'sounds.txt')} に取り込んだ")


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
