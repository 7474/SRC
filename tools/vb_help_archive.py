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
    print(f"{OUT.relative_to(ROOT)}: {len(pages)} 頁（目次 {len(order)} 頁・その写し {len(canonical) - len(order)} 頁）、検索 {len(index)} 頁、キーワード {len(keywords)} 語")


if __name__ == "__main__":
    main()
