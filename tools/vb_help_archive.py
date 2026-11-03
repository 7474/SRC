#!/usr/bin/env python3
"""
元の SRC（Ver2.2.33）のヘルプを、SRC# のヘルプのサイトの /vb/ にアーカイブとして置く。

元にするのは SRC/HelpChm（CHM の元の HTML・画像・目次 src.hhc）。元のヘルプは変わらないので、
このスクリプトで一度作ってコミットする（SRC.Sharp.Help/src/vb/）。作り直すときもこのスクリプトを使い、手で直さない。

    python3 tools/vb_help_archive.py

作るもの:
- html/*.htm   元の頁。UTF-8 にする（元は Shift_JIS で、文字コードの宣言も誤っていた）。アーカイブの説明は index.html にだけ出す
- images/      元の画像
- index.html   目次（src.hhc）と検索
- search.json  検索の索引（頁ごとの題と本文）。検索はブラウザの中で行う（SRC# のヘルプの検索とは混ぜない）

Jekyll は front matter の無いファイルをそのまま配信するので、/vb/ は SRC# のヘルプの見た目・検索・
srcsharp help とは独立する。
"""
import html
import json
import pathlib
import re
import shutil
import unicodedata

ROOT = pathlib.Path(__file__).resolve().parent.parent
SOURCE = ROOT / "SRC" / "HelpChm"
OUT = ROOT / "SRC.Sharp.Help" / "src" / "vb"

TITLE = "SRC Ver2.2.33 ヘルプ（アーカイブ）"
CREDIT = "SRC © 1997-2012 Kei Sakamoto / Inui Tetsuyuki"

def read(path):
    return path.read_bytes().decode("cp932", errors="replace")


def page(text):
    """元の頁を UTF-8 の頁にする。CHM 用の ActiveX（キーワードの OBJECT）を除く。アーカイブの説明は目次の頁にだけ出す"""
    text = re.sub(r"<OBJECT\b.*?</OBJECT>", "", text, flags=re.I | re.S)
    text = re.sub(r"<META[^>]*Content-Type[^>]*>", "", text, flags=re.I)
    head = '<meta charset="utf-8">\n<meta name="viewport" content="width=device-width, initial-scale=1">\n'
    return re.sub(r"<HEAD>", lambda m: m.group(0) + "\n" + head, text, count=1, flags=re.I)


def title_of(text):
    m = re.search(r"<TITLE>(.*?)</TITLE>", text, re.I | re.S)
    return html.unescape(m.group(1)).strip() if m else ""


def body_text(text):
    text = re.sub(r"<OBJECT\b.*?</OBJECT>", " ", text, flags=re.I | re.S)
    text = re.sub(r"<(script|style|title)\b.*?</\1>", " ", text, flags=re.I | re.S)
    text = re.sub(r"<[^>]+>", " ", text)
    text = html.unescape(text)
    return re.sub(r"\s+", " ", text).strip()


def toc(text):
    """src.hhc（入れ子の UL・LI と OBJECT の Name・Local）を、入れ子の (名前, リンク, 子) の並びにする"""
    tokens = re.finditer(r"<UL>|</UL>|<OBJECT type=\"text/sitemap\">(.*?)</OBJECT>", text, re.I | re.S)
    root = []
    stack = [root]
    started = False
    for token in tokens:
        tag = token.group(0)
        if tag.upper() == "<UL>":
            if started:
                parent = stack[-1]
                stack.append(parent[-1][2] if parent else parent)
            started = True
        elif tag.upper() == "</UL>":
            if len(stack) > 1:
                stack.pop()
        else:
            params = dict(re.findall(r'<param name="(\w+)" value="([^"]*)"', token.group(1), re.I))
            stack[-1].append((html.unescape(params.get("Name", "")), params.get("Local", ""), []))
    return root


def toc_html(items):
    lines = ["<ul>"]
    for name, local, children in items:
        label = html.escape(name)
        if local:
            label = f'<a href="{html.escape(local)}">{label}</a>'
        if children:
            lines.append(f"<li><details><summary>{label}</summary>{toc_html(children)}</details></li>")
        else:
            lines.append(f"<li>{label}</li>")
    lines.append("</ul>")
    return "".join(lines)


INDEX = """<!doctype html>
<html lang="ja">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{title}</title>
<style>
body{{margin:0 auto;max-width:960px;padding:12px 16px;font:15px/1.7 system-ui,-apple-system,"Segoe UI","Hiragino Sans","Noto Sans JP",sans-serif;color:#222}}
h1{{font-size:20px;margin:8px 0}}
.note{{padding:6px 10px;border:1px solid #c9b46a;background:#fff8dc;font-size:13px}}
#q{{width:100%;box-sizing:border-box;font-size:16px;padding:6px 8px;margin:12px 0 4px}}
#results li{{margin:6px 0}}
#results .snippet{{display:block;color:#555;font-size:13px}}
mark{{background:#ffe680}}
ul{{padding-left:20px}} li{{margin:2px 0}} summary{{cursor:pointer}}
a{{color:#0645ad}}
</style>
</head>
<body>
<h1>{title}</h1>
<p class="note">元の SRC（VB 版、Ver2.2.33）に付いていたヘルプのアーカイブです。データ・イベントの書き方を元の SRC と比べるときなどに使ってください。
SRC# の使い方と、SRC# での動作は <a href="https://srch.7474.jp/">SRC# のヘルプ</a> を見てください。<br>{credit}。元の HTML は <a href="https://github.com/7474/SRC/tree/master/SRC/HelpChm">7474/SRC の SRC/HelpChm</a> にあります。</p>
<input id="q" type="search" placeholder="このアーカイブの中を検索（空白で区切ると、すべての語を含む頁）" autocomplete="off">
<div id="status" class="note" hidden></div>
<ol id="results"></ol>
<h2>目次</h2>
{toc}
<script>
(() => {{
  const q = document.getElementById('q');
  const results = document.getElementById('results');
  const status = document.getElementById('status');
  let pages = null;
  // 全角・半角、大文字・小文字の違いを無視して比べる
  const norm = s => s.normalize('NFKC').toLowerCase();
  const escape = s => s.replace(/[&<>"]/g, c => ({{'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;'}})[c]);
  async function load() {{
    if (!pages) {{
      const data = await (await fetch('search.json')).json();
      pages = data.map(p => ({{ ...p, nt: normMap(p.t)[0], nb: normMap(p.b)[0] }}));
    }}
    return pages;
  }}
  // 1 文字ずつそろえ、そろえた後の位置から元の文字の位置を引けるようにする（強調の位置をずらさない）
  function normMap(s) {{
    let out = '';
    const map = [];
    for (let i = 0; i < s.length; i++) {{
      for (const c of norm(s[i])) {{ out += c; map.push(i); }}
    }}
    map.push(s.length);
    return [out, map];
  }}
  function snippet(page, term) {{
    const [n, map] = normMap(page.b);
    const i = n.indexOf(term);
    if (i < 0) return escape(page.b.slice(0, 80));
    const from = map[i];
    const to = map[i + term.length - 1] + 1;
    const start = Math.max(0, from - 30);
    return (start > 0 ? '…' : '') + escape(page.b.slice(start, from)) + '<mark>' + escape(page.b.slice(from, to)) + '</mark>' + escape(page.b.slice(to, to + 50)) + '…';
  }}
  async function search() {{
    const terms = normMap(q.value)[0].split(/\\s+/).filter(Boolean);
    const hash = q.value ? '#q=' + encodeURIComponent(q.value) : location.pathname;
    history.replaceState(null, '', hash);
    if (!terms.length) {{ results.innerHTML = ''; status.hidden = true; return; }}
    const all = await load();
    const hits = all.filter(p => terms.every(t => p.nt.includes(t) || p.nb.includes(t)))
      .map(p => ({{ p, score: terms.reduce((s, t) => s + (p.nt.includes(t) ? 10 : 0), 0) }}))
      .sort((a, b) => b.score - a.score);
    status.hidden = false;
    status.textContent = hits.length + ' 件' + (hits.length > 200 ? '（先頭の 200 件を出しています）' : '');
    results.innerHTML = hits.slice(0, 200).map(({{ p }}) =>
      '<li><a href="' + p.u + '">' + escape(p.t || p.u) + '</a><span class="snippet">' + snippet(p, terms[0]) + '</span></li>').join('');
  }}
  let timer;
  q.addEventListener('input', () => {{ clearTimeout(timer); timer = setTimeout(search, 200); }});
  const m = location.hash.match(/^#q=(.*)$/);
  if (m) {{ q.value = decodeURIComponent(m[1]); search(); }}
}})();
</script>
</body>
</html>
"""


def main():
    if OUT.exists():
        shutil.rmtree(OUT)
    (OUT / "html").mkdir(parents=True)
    shutil.copytree(SOURCE / "images", OUT / "images")
    index = []
    for path in sorted((SOURCE / "html").glob("*.htm")):
        text = read(path)
        (OUT / "html" / path.name).write_text(page(text), encoding="utf-8")
        index.append({"u": f"html/{path.name}", "t": title_of(text), "b": body_text(text)})
    (OUT / "search.json").write_text(json.dumps(index, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    tree = toc(read(SOURCE / "src.hhc"))
    (OUT / "index.html").write_text(INDEX.format(title=TITLE, credit=CREDIT, toc=toc_html(tree)), encoding="utf-8")
    print(f"{OUT.relative_to(ROOT)}: {len(index)} 頁")


if __name__ == "__main__":
    main()
