#!/usr/bin/env python3
"""
元の SRC（Ver2.2.33）のヘルプを、SRC# のヘルプのサイトの /vb/ にアーカイブとして置く。

元にするのは SRC/HelpChm（CHM の元の HTML・画像・目次 src.hhc・キーワード SRC.hhk）。元のヘルプは変わらないので、
このスクリプトで一度作ってコミットする（SRC.Sharp.Help/src/vb/）。作り直すときもこのスクリプトを使い、手で直さない。

    python3 tools/vb_help_archive.py

見た目は Windows 95 のヘルプ（WinHelp）に似せ、使い勝手は今のサイトに合わせる。
- 各頁: 上にボタンの列（目次・表紙・戻る・<<・>>）、左に「ヘルプ トピック」の欄（目次・キーワード・検索のタブ）、
  本文の下に前後の頁へのリンク（目次の順）。狭い画面では左の欄を「目次」のボタンで引き出す
- 本文は元の HTML のまま（UTF-8 にし、CHM 用の ActiveX を除く）。アーカイブの説明は表紙（index.html）にだけ出す

作るもの:
- html/*.htm     元の頁（上の枠に入れたもの）
- images/        元の画像
- index.html     表紙（アーカイブの説明）
- winhelp.css・winhelp.js  見た目と、左の欄（目次のツリー・キーワード・検索）。元は tools/vb_help_archive/
- toc.json・keywords.json・search.json  目次・キーワード・検索の索引（検索の索引は検索したときだけ読む）

Jekyll は front matter の無いファイルをそのまま配信するので、/vb/ は SRC# のヘルプの見た目・検索・
srcsharp help とは独立する。
"""
import html
import json
import pathlib
import re
import shutil
import struct

ROOT = pathlib.Path(__file__).resolve().parent.parent
SOURCE = ROOT / "SRC" / "HelpChm"
ASSETS = pathlib.Path(__file__).resolve().parent / "vb_help_archive"
OUT = ROOT / "SRC.Sharp.Help" / "src" / "vb"

TITLE = "SRC ヘルプ"
CREDIT = "SRC © 1997-2012 Kei Sakamoto / Inui Tetsuyuki"


def read(path):
    return path.read_bytes().decode("cp932", errors="replace")


def title_of(text):
    m = re.search(r"<TITLE>(.*?)</TITLE>", text, re.I | re.S)
    return html.unescape(m.group(1)).strip() if m else ""


def body_of(text):
    """元の頁の本文。CHM 用の ActiveX（キーワードの OBJECT）を除く"""
    m = re.search(r"<BODY[^>]*>(.*?)</BODY>", text, re.I | re.S)
    body = m.group(1) if m else text
    return re.sub(r"<OBJECT\b.*?</OBJECT>", "", body, flags=re.I | re.S).strip()


def body_text(body):
    text = re.sub(r"<(script|style)\b.*?</\1>", " ", body, flags=re.I | re.S)
    text = re.sub(r"<[^>]+>", " ", text)
    return re.sub(r"\s+", " ", html.unescape(text)).strip()


def sitemap(text):
    """src.hhc・SRC.hhk（入れ子の UL・LI と OBJECT の Name・Local）を、入れ子の {n: 名前, u: リンク, c: 子} にする"""
    root = []
    stack = [root]
    started = False
    for token in re.finditer(r"<UL>|</UL>|<OBJECT type=\"text/sitemap\">(.*?)</OBJECT>", text, re.I | re.S):
        tag = token.group(0).upper()
        if tag == "<UL>":
            if started and stack[-1]:
                stack.append(stack[-1][-1].setdefault("c", []))
            elif started:
                stack.append(stack[-1])
            started = True
        elif tag == "</UL>":
            if len(stack) > 1:
                stack.pop()
        else:
            params = dict(re.findall(r'<param name="(\w+)" value="([^"]*)"', token.group(1), re.I))
            item = {"n": html.unescape(params.get("Name", ""))}
            if params.get("Local"):
                item["u"] = params["Local"]
            stack[-1].append(item)
    return root


# ── ポップアップ（WinHelp では、押すと頁を移らずに小さな窓で説明を出していたもの） ──
# 元の WinHelp のプロジェクト（SRC/Help/Src.hdp）の {\ul 語}{\v 飛び先} が本文の中の用語のポップアップ、
# SHG（SRC/Help/*.shg）の押せる場所が図の上のポップアップ。CHM にしたときに普通のリンクになり、
# 図の押せる場所は失われていた
HELP = ROOT / "SRC" / "Help"
# SHG と、それを表示している図、その説明の頁の題に付ける呼び出し元の名前
FIGURES = [
    ("StatusWindow.shg", "bm8.gif", "ステータスウインドウ"),
    ("MapEdit.shg", "bm23.gif", "MapEdit"),
]
# 図の押せる場所の説明だが、押せる場所の情報が残っていない頁（呼び出し元の名前）
ORPHANS = {**{f"HLP{n:06d}": "反撃" for n in range(192, 196)}, "HLP000284": "MapEdit", "HLP000285": "MapEdit"}


def popup_terms():
    """本文の中の用語のポップアップの飛び先の頁の題"""
    text = read(HELP / "Src.hdp")
    pages = dict(re.findall(r"//Page//\n(HLP\d+)\n([^\n]*)\n", text))
    return {pages[i] for i in re.findall(r"\{\\ul [^}]*\}\{\\v (HLP\d+)\}", text) if i in pages}


def hotspots(name):
    """SHG の押せる場所 (x, y, 幅, 高さ, 飛び先)。SHG の形は helpdeco の資料による（押せる場所の数・マクロの大きさ・15 バイトずつの位置）"""
    d = (HELP / name).read_bytes()
    for q in range(len(d) - 3000, len(d)):
        if d[q] != 1:
            continue
        n, size = struct.unpack("<HI", d[q + 1:q + 7])
        names = d.find(b"Hotspot", q)
        if 0 < n < 100 and size < 5000 and abs(q + 7 + 15 * n + size - names) < 4:
            strings = d[q + 7 + 15 * n + size:].split(b"\x00")
            spots = []
            for k in range(n):
                x, y, w, h = struct.unpack("<4h", d[q + 7 + 15 * k + 3:q + 7 + 15 * k + 11])
                spots.append((max(x, 0), max(y, 0), w, h, strings[2 * k + 1].decode("cp932")))
            return spots
    raise ValueError(f"{name} の押せる場所を読めません")


def first_bold(body):
    m = re.search(r"<B>(.*?)</B>", body, re.I | re.S)
    return body_text(m.group(1)) if m else ""


def sequence(items, out=None):
    """目次の順に並べた頁（前後の頁へのリンクに使う）"""
    out = [] if out is None else out
    for item in items:
        if item.get("u") and item["u"] not in out:
            out.append(item["u"])
        sequence(item.get("c", []), out)
    return out


def chrome(title, content, root, prev, next, titles, home=False, current=None):
    """WinHelp に似せた枠。root は vb/ への相対パス"""
    def button(target, label, cls, key):
        if not target:
            return f'<span class="wh-button {cls}" aria-disabled="true">{label}</span>'
        return f'<a class="wh-button {cls}" href="{root}{target}" title="{html.escape(titles.get(target, ""))}" accesskey="{key}">{label}</a>'

    browse = ""
    if not home:
        def side(target, label, rel):
            if not target:
                return "<span></span>"
            return f'<a href="{root}{target}" rel="{rel}">{label}<span>{html.escape(titles.get(target, ""))}</span></a>'
        browse = f'<nav class="wh-browse" aria-label="前後の頁">{side(prev, "« 前へ", "prev")}{side(next, "次へ »", "next")}</nav>'
    return f"""<!doctype html>
<html lang="ja">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{html.escape(title)} - {TITLE}</title>
<link rel="stylesheet" href="{root}winhelp.css">
<script src="{root}winhelp.js" defer></script>
</head>
<body data-root="{root}"{f' data-current="{current}"' if current else ""}>
<div class="wh-window">
<header class="wh-titlebar"><span class="wh-titleicon" aria-hidden="true"></span><span>{TITLE}（元の SRC のヘルプのアーカイブ）</span></header>
<div class="wh-buttons" role="toolbar" aria-label="ボタン">
<button type="button" class="wh-button wh-topics" aria-controls="wh-panel" aria-expanded="false" accesskey="c">目次(<u>C</u>)</button>
<a class="wh-button" href="{root}index.html" accesskey="h">表紙(<u>H</u>)</a>
<button type="button" class="wh-button wh-back" accesskey="b">戻る(<u>B</u>)</button>
{button(prev, "&lt;&lt;", "wh-prev", ",")}
{button(next, "&gt;&gt;", "wh-next", ".")}
</div>
<div class="wh-main">
<aside id="wh-panel" class="wh-panel" aria-label="ヘルプ トピック">
<div class="wh-tabs" role="tablist">
<button type="button" role="tab" data-tab="toc" aria-selected="true">目次</button>
<button type="button" role="tab" data-tab="keywords" aria-selected="false">キーワード</button>
<button type="button" role="tab" data-tab="search" aria-selected="false">検索</button>
</div>
<div class="wh-tabpanel" data-panel="toc"><div class="wh-tree" role="tree" aria-label="目次"></div></div>
<div class="wh-tabpanel" data-panel="keywords" hidden>
<label class="wh-label">1 調べたい語の最初の何文字かを入力してください。<input type="search" class="wh-input" data-input="keywords" autocomplete="off"></label>
<div class="wh-label">2 見たいキーワードをクリックしてください。</div>
<ul class="wh-list" data-list="keywords"></ul>
</div>
<div class="wh-tabpanel" data-panel="search" hidden>
<label class="wh-label">探したい語を入力してください（空白で区切ると、すべての語を含む頁）。<input type="search" class="wh-input" data-input="search" autocomplete="off"></label>
<div class="wh-status" data-status="search"></div>
<ul class="wh-list wh-results" data-list="search"></ul>
</div>
</aside>
<main class="wh-topic">
<div class="wh-content">
{content}
</div>
{browse}
</main>
</div>
</div>
</body>
</html>
"""


HOME = f"""<h1>元の SRC のヘルプ（アーカイブ）</h1>
<p>元の SRC（VB 版、Ver2.2.33）に付いていたヘルプを、ブラウザで読めるようにしたものです。データ・イベントの書き方を元の SRC と比べるときなどに使ってください。</p>
<p>SRC# とは動作が違うところがあります。SRC# の使い方と、SRC# での動作は <a href="https://srch.7474.jp/">SRC# のヘルプ</a> を見てください。</p>
<ul>
<li>「目次」のボタンで、目次・キーワード・検索の欄を出し入れします（広い画面でははじめから出ています）。</li>
<li>本文の下の「前へ」「次へ」と、上のボタンの「&lt;&lt;」「&gt;&gt;」で、目次の順に頁を移ります。</li>
<li>検索は、このアーカイブの中だけを探します。</li>
</ul>
<p><a href="html/00000001.htm">はじめに</a>から読む</p>
<p class="wh-credit">{CREDIT}。元の HTML は <a href="https://github.com/7474/SRC/tree/master/SRC/HelpChm">7474/SRC の SRC/HelpChm</a> にあります。文字コードを UTF-8 に直し、この枠に入れたほかは手を加えていません。</p>
"""


def main():
    if OUT.exists():
        shutil.rmtree(OUT)
    (OUT / "html").mkdir(parents=True)
    shutil.copytree(SOURCE / "images", OUT / "images")
    for name in ("winhelp.css", "winhelp.js"):
        shutil.copy(ASSETS / name, OUT / name)

    tree = sitemap(read(SOURCE / "src.hhc"))
    order = sequence(tree)
    keywords = [{"n": k["n"], "u": k["u"]} for k in sitemap(read(SOURCE / "SRC.hhk")) if k.get("u")]
    keywords.sort(key=lambda k: k["n"])

    pages = {}
    for path in sorted((SOURCE / "html").glob("*.htm")):
        text = read(path)
        pages[f"html/{path.name}"] = (title_of(text), body_of(text))
    by_page_title = {title: key for key, (title, _) in pages.items()}

    # 図の押せる場所の説明の頁（題が「HLP000164」のような頁）に、呼び出し元と項目の名前で題を付ける
    callers = dict(ORPHANS)
    figures = {}
    for shg, image, caller in FIGURES:
        spots = hotspots(shg)
        figures[image] = (caller, spots)
        for *_, target in spots:
            callers.setdefault(target, caller)
    for hid, caller in callers.items():
        key = by_page_title.get(hid)
        if key:
            title, body = pages[key]
            pages[key] = (f"{caller}: {first_bold(body)}", body)
    popups = {by_page_title[t] for t in popup_terms() if t in by_page_title}
    popups |= {by_page_title[h] for h in callers if h in by_page_title}

    # 本文の中の、ポップアップだったリンクに印を付ける。図には押せる場所（image map）を付ける
    def mark(body):
        def link(m):
            target = "html/" + m.group(2)
            return m.group(1) + (' class="wh-popup"' if target in popups else "")
        body = re.sub(r'(<A\s+HREF="([^"#]+\.htm)(?:#[^"]*)?")', link, body, flags=re.I)
        for image, (caller, spots) in figures.items():
            if f"images/{image}" not in body:
                continue
            areas = "".join(
                f'<area shape="rect" coords="{x},{y},{x + w},{y + h}" href="{by_page_title[t].split("/")[1]}" class="wh-popup" '
                f'alt="{html.escape(pages[by_page_title[t]][0])}">'
                for x, y, w, h, t in spots if t in by_page_title)
            name = image.split(".")[0]
            body = re.sub(rf'(<IMG\s+SRC="\.\./images/{re.escape(image)}")', rf'\1 usemap="#{name}"', body, count=1, flags=re.I)
            body += f'<map name="{name}">{areas}</map>'
        return body
    pages = {key: (title, mark(body)) for key, (title, body) in pages.items()}
    titles = {key: title for key, (title, _) in pages.items()}

    # 目次に無い頁の多くは、目次の頁と題も本文も同じ写し（元のヘルプの WinHelp 版から残ったもの）。
    # 写しは目次の頁と同じ位置として扱い（前後の頁・目次の印）、検索の結果には目次の頁だけを出す
    by_title = {}
    for key in order:
        by_title.setdefault(titles[key], []).append(key)
    canonical = {}
    for key, (title, body) in pages.items():
        if key in order:
            canonical[key] = key
        elif len(by_title.get(title, [])) == 1 and body_text(pages[by_title[title][0]][1]) == body_text(body):
            canonical[key] = by_title[title][0]

    index = []
    for key, (title, body) in pages.items():
        base = canonical.get(key)
        position = order.index(base) if base else -1
        prev = order[position - 1] if position > 0 else None
        next = order[position + 1] if 0 <= position < len(order) - 1 else None
        (OUT / key).write_text(chrome(title, body, "../", prev, next, titles, current=base), encoding="utf-8")
        if base in (None, key):
            index.append({"u": key, "t": title, "b": body_text(body)})

    (OUT / "index.html").write_text(chrome("表紙", HOME, "", None, order[0], titles, home=True), encoding="utf-8")

    def dump(name, data):
        (OUT / name).write_text(json.dumps(data, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    dump("toc.json", tree)
    dump("keywords.json", keywords)
    dump("search.json", index)
    print(f"ポップアップ {len(popups)} 頁・図の押せる場所 {sum(len(v[1]) for v in figures.values())} か所")
    print(f"{OUT.relative_to(ROOT)}: {len(pages)} 頁（目次 {len(order)} 頁・その写し {len(canonical) - len(order)} 頁）、検索 {len(index)} 頁、キーワード {len(keywords)} 語")


if __name__ == "__main__":
    main()
