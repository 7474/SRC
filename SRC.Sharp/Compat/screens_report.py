#!/usr/bin/env python3
# SPDX-License-Identifier: MIT
# Copyright (c) 2026 koudenpa
"""
画面と音の見比べ（#1553）の並べた一覧を作る。手順は SCREENS.md。

  python3 screens_report.py [--actual SRC# の画像のフォルダ] [--out 出力先]

- 場面の一覧は screens-scenes.txt
- SRC#（SRCRender）の画像と音の記録は、SRCRenderTests の CompatScreensTests が書き出したもの
  （既定は SRC.Sharp/SRCRenderTests/bin/*/net11.0/TestResults/e2e/compat-screens の新しいもの）
- 実機（元の SRC・VB5 の Src.exe）のスクリーンショットは screens-expected/<ID>.png、
  聞いた音のメモは screens-expected/sounds.txt（1 行に「ID<TAB>聞いたこと」）
- SRC#Form（Windows）のスクリーンショットは screens-form/<ID>.png
- 出力は <出力先>/index.html（画像は <出力先>/images/ に写す）。既定の出力先は ./out/screens-report

画像はピクセルで比べない（フォントなどで違う）。並べて人が見る。どれかの画像が無くても一覧は作る（無い所は「なし」）。
"""
import argparse
import glob
import html
import os
import shutil
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
SHARP = os.path.dirname(HERE)
IMAGE_EXTENSIONS = [".png", ".jpg", ".jpeg", ".bmp"]

COLUMNS = [
    ("expected", "実機（Src.exe）", os.path.join(HERE, "screens-expected")),
    ("actual", "SRC#（SRCRender）", None),
    ("form", "SRC#Form（Windows）", os.path.join(HERE, "screens-form")),
]


def read_scenes():
    scenes = []
    with open(os.path.join(HERE, "screens-scenes.txt"), encoding="utf-8") as f:
        for line in f:
            line = line.rstrip("\n")
            if not line or line.startswith("#"):
                continue
            cols = line.split("\t")
            cols += [""] * (4 - len(cols))
            scenes.append({"id": cols[0], "kind": cols[1], "title": cols[2], "steps": cols[3]})
    return scenes


def read_tsv(path):
    """「ID<TAB>値」の行を読む。無ければ空"""
    result = {}
    if path and os.path.exists(path):
        with open(path, encoding="utf-8-sig") as f:
            for line in f:
                line = line.rstrip("\r\n")
                if not line or line.startswith("#") or "\t" not in line:
                    continue
                key, value = line.split("\t", 1)
                result[key] = value
    return result


def default_actual():
    candidates = glob.glob(os.path.join(SHARP, "SRCRenderTests", "bin", "*", "*", "TestResults", "e2e", "compat-screens"))
    candidates = [c for c in candidates if os.path.isdir(c)]
    if not candidates:
        return None
    return max(candidates, key=os.path.getmtime)


def find_image(folder, scene_id):
    if not folder:
        return None
    for ext in IMAGE_EXTENSIONS:
        path = os.path.join(folder, scene_id + ext)
        if os.path.exists(path):
            return path
    return None


def main(argv):
    ap = argparse.ArgumentParser(description="画面と音の見比べ（#1553）の並べた一覧を作る")
    ap.add_argument("--actual", help="SRC#（SRCRender）の画像と sounds.txt のフォルダ（CompatScreensTests が書き出したもの）")
    ap.add_argument("--out", default=os.path.join(HERE, "out", "screens-report"), help="出力先（中身は消してから書く）")
    args = ap.parse_args(argv)

    actual = os.path.abspath(args.actual) if args.actual else default_actual()
    if actual is None or not os.path.isdir(actual):
        print("SRC# の画像が見つからない。先に dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj "
              "--filter FullyQualifiedName~CompatScreensTests を流すか、--actual で渡す", file=sys.stderr)
        return 1
    folders = {key: (actual if key == "actual" else folder) for key, _, folder in COLUMNS}

    out = os.path.abspath(args.out)
    if os.path.exists(out):
        shutil.rmtree(out)
    os.makedirs(os.path.join(out, "images"))

    scenes = read_scenes()
    actual_sounds = read_tsv(os.path.join(actual, "sounds.txt"))
    expected_sounds = read_tsv(os.path.join(HERE, "screens-expected", "sounds.txt"))

    counts = {key: 0 for key, _, _ in COLUMNS}
    rows = []
    for scene in scenes:
        if scene["kind"] != "画面":
            continue
        cells = []
        for key, label, _ in COLUMNS:
            path = find_image(folders[key], scene["id"])
            if path is None:
                cells.append('<td class="none">なし</td>')
                continue
            counts[key] += 1
            name = f"{key}-{scene['id']}{os.path.splitext(path)[1].lower()}"
            shutil.copyfile(path, os.path.join(out, "images", name))
            cells.append(f'<td><a href="images/{html.escape(name)}"><img src="images/{html.escape(name)}" alt="{html.escape(label)}"></a></td>')
        rows.append(f'<tr id="{html.escape(scene["id"])}"><th><code>{html.escape(scene["id"])}</code><br>{html.escape(scene["title"])}'
                    f'<div class="steps">{html.escape(scene["steps"])}</div></th>' + "".join(cells) + "</tr>")

    sound_rows = []
    for scene in scenes:
        sid = scene["id"]
        played = actual_sounds.get(sid, "")
        heard = expected_sounds.get(sid, "")
        if scene["kind"] != "音" and played in ("", "（なし）") and not heard:
            continue
        sound_rows.append(f'<tr><th><code>{html.escape(sid)}</code><br>{html.escape(scene["title"])}</th>'
                          f'<td>{html.escape(played) or "（記録なし）"}</td>'
                          f'<td class="{"" if heard else "none"}">{html.escape(heard) or "まだ"}</td></tr>')
    if "(終わり)" in actual_sounds:
        sound_rows.append(f'<tr><th>（終わり）</th><td>{html.escape(actual_sounds["(終わり)"])}</td><td></td></tr>')

    total = sum(1 for s in scenes if s["kind"] == "画面")
    header = "".join(f"<th>{html.escape(label)}<br><small>{counts[key]} / {total}</small></th>" for key, label, _ in COLUMNS)
    page = f"""<!doctype html>
<html lang="ja">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>画面と音の見比べ（#1553）</title>
<style>
body {{ font-family: sans-serif; margin: 16px; background: #fff; color: #222; }}
table {{ border-collapse: collapse; }}
th, td {{ border: 1px solid #ccc; padding: 4px; vertical-align: top; }}
th {{ text-align: left; background: #f4f4f4; max-width: 16em; }}
td img {{ max-width: 480px; height: auto; display: block; }}
td.none {{ color: #999; text-align: center; vertical-align: middle; min-width: 8em; }}
.steps {{ font-weight: normal; font-size: 85%; color: #555; margin-top: 4px; }}
</style>
</head>
<body>
<h1>画面と音の見比べ（#1553）</h1>
<p>元の SRC（VB5 の Src.exe）と SRC# の、同じ場面の画面を並べる。ピクセルの一致は求めない（フォントなどで違う）。
配置・色・文字・大きさの違いを見る。場面の一覧と手順は SRC.Sharp/Compat/SCREENS.md。</p>
<p>SRC# の画像: <code>{html.escape(actual)}</code></p>
<h2>画面</h2>
<table>
<tr><th>場面</th>{header}</tr>
{chr(10).join(rows)}
</table>
<h2>音</h2>
<p>SRC# の記録は、前の場面からその場面までに鳴らした BGM と効果音（CompatScreensTests）。実機は screens-expected/sounds.txt に人が書いたもの。</p>
<table>
<tr><th>場面</th><th>SRC# の記録</th><th>実機で聞いたこと</th></tr>
{chr(10).join(sound_rows)}
</table>
</body>
</html>
"""
    with open(os.path.join(out, "index.html"), "w", encoding="utf-8", newline="\n") as f:
        f.write(page)
    summary = "、".join(f"{label} {counts[key]}/{total}" for key, label, _ in COLUMNS)
    print(f"{os.path.join(out, 'index.html')} を作った（{summary}）")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
