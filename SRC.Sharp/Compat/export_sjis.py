#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
互換性の検証用のシナリオ（torres/）を、元の SRC（VB5 の Src.exe）が読める形に書き出す（#1549）。

リポジトリの中では、テキスト（*.eve・*.txt・*.map）を UTF-8・LF で持つ（SRC# はデータとイベントを UTF-8 で読む）。
元の SRC は Shift_JIS（Windows のコードページ 932）・CRLF で読むので、このスクリプトで変換して別のフォルダに書き出す。
画像・音（*.bmp・*.wav・*.mid）はそのまま写す。

- Shift_JIS にできない文字（例: U+2212 の「−」、U+00A0、絵文字）があれば、ファイル・行・文字を出して失敗する
- ファイル名・フォルダ名が ASCII でなければ失敗する（zip に入れたときや日本語版でない Windows で名前が化けないように）
- UTF-8 として読めないファイル、BOM の付いたファイルがあれば失敗する

使い方:
  python3 export_sjis.py [出力先]          出力先の既定は ./out/torres（中身は消してから書く）
  python3 export_sjis.py --check           書き出さずに、変換できるかだけを確かめる
  python3 export_sjis.py 出力先 --zip      出力先のフォルダを zip にもする（出力先.zip）
"""
import os
import shutil
import sys
import zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
SOURCE = os.path.join(HERE, "torres")
TEXT_EXTENSIONS = {".eve", ".txt", ".map", ".ini"}
BINARY_EXTENSIONS = {".bmp", ".wav", ".mid"}


def convert(path):
    """UTF-8・LF のテキストを Shift_JIS・CRLF のバイト列にする。問題があれば (None, [メッセージ]) を返す"""
    raw = open(path, "rb").read()
    problems = []
    if raw.startswith(b"\xef\xbb\xbf"):
        problems.append("BOM が付いている")
    try:
        text = raw.decode("utf-8")
    except UnicodeDecodeError as e:
        return None, [f"UTF-8 として読めない（{e}）"]
    # Windows で core.autocrlf を使って取り出した CRLF のファイルも受け付ける
    text = text.replace("\r\n", "\n")
    out = []
    for no, line in enumerate(text.split("\n"), 1):
        for ch in line:
            try:
                ch.encode("cp932")
            except UnicodeEncodeError:
                problems.append(f"{no} 行目: Shift_JIS にできない文字 U+{ord(ch):04X}「{ch}」")
        out.append(line)
    if problems:
        return None, problems
    return "\r\n".join(out).encode("cp932"), []


def main(argv):
    check_only = "--check" in argv
    make_zip = "--zip" in argv
    args = [a for a in argv if not a.startswith("--")]
    dest = os.path.abspath(args[0]) if args else os.path.join(HERE, "out", "torres")

    errors = []
    files = []
    for root, dirs, names in os.walk(SOURCE):
        dirs.sort()
        for name in sorted(names):
            full = os.path.join(root, name)
            rel = os.path.relpath(full, SOURCE)
            if not rel.isascii():
                errors.append(f"{rel}: ファイル名・フォルダ名に ASCII でない文字がある")
            ext = os.path.splitext(name)[1].lower()
            if ext in TEXT_EXTENSIONS:
                data, problems = convert(full)
                errors += [f"{rel}: {p}" for p in problems]
                files.append((rel, data))
            elif ext in BINARY_EXTENSIONS:
                files.append((rel, None))
            else:
                errors.append(f"{rel}: 扱いの決まっていない種類のファイル")

    if errors:
        print("\n".join(errors), file=sys.stderr)
        return 1
    if check_only:
        print(f"{len(files)} ファイルを確かめた。Shift_JIS に変換できる")
        return 0

    if os.path.exists(dest):
        shutil.rmtree(dest)
    for rel, data in files:
        out = os.path.join(dest, rel)
        os.makedirs(os.path.dirname(out), exist_ok=True)
        if data is None:
            shutil.copyfile(os.path.join(SOURCE, rel), out)
        else:
            with open(out, "wb") as f:
                f.write(data)
    print(f"{len(files)} ファイルを {dest} に書き出した")

    if make_zip:
        archive = dest.rstrip(os.sep) + ".zip"
        top = os.path.basename(dest.rstrip(os.sep))
        with zipfile.ZipFile(archive, "w", zipfile.ZIP_DEFLATED) as z:
            for rel, _ in files:
                z.write(os.path.join(dest, rel), os.path.join(top, rel).replace(os.sep, "/"))
        print(f"{archive} を作った")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
