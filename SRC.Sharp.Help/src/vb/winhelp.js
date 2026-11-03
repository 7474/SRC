// 元の SRC のヘルプのアーカイブ（/vb/）の左の欄（目次のツリー・キーワード・検索）と、ボタンの動き。
// tools/vb_help_archive.py が SRC.Sharp.Help/src/vb/ に写す。ここを直したら作り直す。
(() => {
  const root = document.body.dataset.root || '';
  // 今の頁（toc.json などのリンクと同じ「html/xxx.htm」の形）
  // 目次の頁の写しでは、目次の頁（data-current）を今の頁として扱う
  const here = document.body.dataset.current || (() => {
    const m = location.pathname.match(/\/(html\/[^/]+\.htm)$/i);
    return m ? m[1] : '';
  })();
  const panel = document.getElementById('wh-panel');
  const toggle = document.querySelector('.wh-topics');
  const wide = window.matchMedia('(min-width: 800px)');
  const store = {
    get(key) { try { return localStorage.getItem(key); } catch { return null; } },
    set(key, value) { try { localStorage.setItem(key, value); } catch { /* 保存できなくても動く */ } },
  };

  // 左の欄を出す・しまう。広い画面では出しておき（しまったら覚える）、狭い画面では引き出しにする
  const backdrop = document.createElement('div');
  backdrop.className = 'wh-backdrop';
  panel.after(backdrop);
  function setOpen(open, remember) {
    document.body.classList.toggle('wh-panel-open', open);
    toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
    if (remember && wide.matches) {
      store.set('srcvb.panel', open ? 'open' : 'closed');
    }
  }
  setOpen(wide.matches && store.get('srcvb.panel') !== 'closed', false);
  wide.addEventListener('change', () => setOpen(wide.matches && store.get('srcvb.panel') !== 'closed', false));
  toggle.addEventListener('click', () => {
    const open = !document.body.classList.contains('wh-panel-open');
    setOpen(open, true);
    if (open) {
      panel.querySelector('[aria-current="page"], [role="tab"][aria-selected="true"]')?.focus({ preventScroll: true });
    }
  });
  backdrop.addEventListener('click', () => setOpen(false, false));
  document.addEventListener('keydown', e => {
    if (e.key === 'Escape' && !wide.matches && document.body.classList.contains('wh-panel-open')) {
      setOpen(false, false);
      toggle.focus();
    }
  });
  // 狭い画面では、頁を選んだら引き出しをしまう
  panel.addEventListener('click', e => {
    if (!wide.matches && e.target.closest('a[href]')) {
      setOpen(false, false);
    }
  });

  document.querySelector('.wh-back').addEventListener('click', () => history.back());

  // タブ（目次・キーワード・検索）。選んだタブは覚えておく
  const tabs = [...panel.querySelectorAll('[role="tab"]')];
  const loaded = {};
  function selectTab(name, focus) {
    for (const tab of tabs) {
      const selected = tab.dataset.tab === name;
      tab.setAttribute('aria-selected', selected ? 'true' : 'false');
      tab.tabIndex = selected ? 0 : -1;
      panel.querySelector(`[data-panel="${tab.dataset.tab}"]`).hidden = !selected;
    }
    store.set('srcvb.tab', name);
    if (!loaded[name]) {
      loaded[name] = true;
      ({ toc: loadToc, keywords: loadKeywords, search: () => {} })[name]();
    }
    if (focus) {
      panel.querySelector(`[data-input="${name}"]`)?.focus();
    }
  }
  tabs.forEach((tab, i) => {
    tab.addEventListener('click', () => selectTab(tab.dataset.tab, true));
    tab.addEventListener('keydown', e => {
      const step = { ArrowRight: 1, ArrowLeft: -1 }[e.key];
      if (step) {
        const next = tabs[(i + step + tabs.length) % tabs.length];
        selectTab(next.dataset.tab, false);
        next.focus();
      }
    });
  });

  const escape = s => s.replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' })[c]);
  const json = name => fetch(root + name).then(r => r.json());

  // 目次のツリー。今の頁までの本を開き、今の頁に印を付ける
  async function loadToc() {
    const tree = panel.querySelector('.wh-tree');
    const items = await json('toc.json');
    const contains = item => item.u === here || (item.c || []).some(contains);
    const render = list => '<ul role="group">' + list.map(item => {
      const label = '<span>' + escape(item.n) + '</span>';
      const current = item.u && item.u === here ? ' aria-current="page"' : '';
      if (item.c && item.c.length) {
        const title = item.u ? `<a href="${root}${item.u}"${current}>${label}</a>` : label;
        return `<li role="treeitem"><details${contains(item) ? ' open' : ''}><summary>${title}</summary>${render(item.c)}</details></li>`;
      }
      return `<li role="treeitem"><a class="wh-page" href="${root}${item.u || ''}"${current}>${label}</a></li>`;
    }).join('') + '</ul>';
    tree.innerHTML = render(items);
    tree.querySelector('[aria-current="page"]')?.scrollIntoView({ block: 'center' });
  }

  // キーワード（元のヘルプの索引）。入力した文字で始まるものを先に、含むものを後に並べる
  const norm = s => s.normalize('NFKC').toLowerCase();
  async function loadKeywords() {
    const input = panel.querySelector('[data-input="keywords"]');
    const list = panel.querySelector('[data-list="keywords"]');
    const keywords = (await json('keywords.json')).map(k => ({ ...k, k: norm(k.n) }));
    const show = () => {
      const q = norm(input.value.trim());
      const hits = q
        ? [...keywords.filter(k => k.k.startsWith(q)), ...keywords.filter(k => !k.k.startsWith(q) && k.k.includes(q))]
        : keywords;
      list.innerHTML = hits.map(k => `<li><a href="${root}${k.u}"${k.u === here ? ' aria-current="page"' : ''}>${escape(k.n)}</a></li>`).join('');
    };
    input.addEventListener('input', show);
    input.addEventListener('keydown', e => {
      if (e.key === 'Enter') {
        list.querySelector('a')?.click();
      }
    });
    show();
  }

  // 検索（このアーカイブの中だけ）。索引は最初に探したときに読む
  {
    const input = panel.querySelector('[data-input="search"]');
    const list = panel.querySelector('[data-list="search"]');
    const status = panel.querySelector('[data-status="search"]');
    let pages = null;
    // 1 文字ずつそろえ、そろえた後の位置から元の文字の位置を引けるようにする（強調の位置をずらさない）
    const normMap = s => {
      let out = '';
      const map = [];
      for (let i = 0; i < s.length; i++) {
        for (const c of norm(s[i])) { out += c; map.push(i); }
      }
      map.push(s.length);
      return [out, map];
    };
    const snippet = (text, term) => {
      const [n, map] = normMap(text);
      const i = n.indexOf(term);
      if (i < 0) return escape(text.slice(0, 80));
      const from = map[i];
      const to = map[i + term.length - 1] + 1;
      const start = Math.max(0, from - 20);
      return (start > 0 ? '…' : '') + escape(text.slice(start, from)) + '<mark>' + escape(text.slice(from, to)) + '</mark>' + escape(text.slice(to, to + 40)) + '…';
    };
    async function search() {
      const terms = normMap(input.value)[0].split(/\s+/).filter(Boolean);
      store.set('srcvb.search', input.value);
      if (!terms.length) { list.innerHTML = ''; status.textContent = ''; return; }
      if (!pages) {
        status.textContent = '索引を読み込んでいます…';
        pages = (await json('search.json')).map(p => ({ ...p, nt: normMap(p.t)[0], nb: normMap(p.b)[0] }));
      }
      const hits = pages.filter(p => terms.every(t => p.nt.includes(t) || p.nb.includes(t)))
        .map(p => ({ p, score: terms.reduce((s, t) => s + (p.nt.includes(t) ? 10 : 0), 0) }))
        .sort((a, b) => b.score - a.score);
      status.textContent = hits.length + ' 件' + (hits.length > 200 ? '（先頭の 200 件）' : '');
      list.innerHTML = hits.slice(0, 200).map(({ p }) =>
        `<li><a href="${root}${p.u}"${p.u === here ? ' aria-current="page"' : ''}>${escape(p.t || p.u)}<span class="wh-snippet">${snippet(p.b, terms[0])}</span></a></li>`).join('');
    }
    let timer;
    input.addEventListener('input', () => { clearTimeout(timer); timer = setTimeout(search, 200); });
    // 頁を移っても、前の検索の語と結果を出す
    const last = store.get('srcvb.search');
    if (last) {
      input.value = last;
      if (store.get('srcvb.tab') === 'search') search();
    }
  }

  selectTab(store.get('srcvb.tab') || 'toc', false);
})();
