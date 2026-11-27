// SRC#Studio のシナリオの見通し（7474/SRC#1692）。srcsharp outline の JSON（SRCAuthoring の ScenarioOutline）を、
// 気づいたこと・部隊と登場人物・出来事・台本・変数の表で出す。スタンドアロン（studio.js）と VS Code の Webview（vscode-outline.js）で共通。
//   hooks.openLine(file, line)       行へ飛ぶ（VS Code はエディタで開く。スタンドアロンは無し）
//   hooks.previewTalk(line, outline) 台本の行を選んだとき、会話の見た目を出す（スタンドアロンはここの MessageWindowPreview、VS Code は talk-preview）
//   hooks.loadImage(kind, bitmap)    顔の画像（スタンドアロンの会話の見た目）
//   hooks.onRefresh()・hooks.onExport() 作り直す・Markdown に書き出す
import { MessageWindowPreview } from './message-window.js';
import { t } from './i18n.js';

const SECTIONS = ['findings', 'cast', 'events', 'script', 'variables'];

export class OutlineView {
  constructor(root, hooks = {}) {
    this.root = root;
    this.hooks = hooks;
    this.data = null;
    this.section = 'findings';
    this.episode = '';
    this.speaker = '';
    this.selected = null;
    this.talk = null;
  }

  setData(outline) {
    this.data = outline;
    const titles = new Set((outline?.episodes ?? []).map(e => e.title));
    if (!titles.has(this.episode)) {
      this.episode = '';
    }
    this.selected = null;
    this.render();
  }

  /** 表示中の要約（テスト・VS Code の状態） */
  status() {
    const d = this.data;
    return d ? {
      section: this.section,
      episodes: d.episodes.length,
      events: d.episodes.reduce((n, e) => n + e.events.length, 0),
      script: d.script.length,
      findings: d.findings.length,
      variables: d.variables.length,
      rows: this.root.querySelectorAll('.ol-body tbody tr, .ol-body li').length,
      selected: this.selected ? { file: this.selected.file, line: this.selected.line, speaker: this.selected.speaker ?? '' } : null,
    } : null;
  }

  render() {
    this.root.innerHTML = '';
    this.talk = null;
    const d = this.data;
    if (!d) {
      return;
    }
    const toolbar = el('div', 'ol-toolbar');
    const summary = el('span', 'ol-summary', t('outline.summary', d.episodes.length, d.episodes.reduce((n, e) => n + e.events.length, 0), d.script.length, d.variables.length, d.findings.length));
    toolbar.append(summary);
    if (this.hooks.onRefresh) {
      toolbar.append(button(t('outline.refresh'), () => this.hooks.onRefresh(), 'ol-refresh'));
    }
    if (this.hooks.onExport) {
      toolbar.append(button(t('outline.export'), () => this.hooks.onExport(), 'ol-export'));
    }
    this.root.append(toolbar);

    const tabs = el('div', 'ol-tabs');
    tabs.setAttribute('role', 'tablist');
    for (const name of SECTIONS) {
      const count = name === 'findings' ? d.findings.length : name === 'script' ? d.script.length : name === 'variables' ? d.variables.length : name === 'events' ? d.episodes.reduce((n, e) => n + e.events.length, 0) : d.episodes.length;
      const b = button(`${t(`outline.${name}`)} (${count})`, () => {
        this.section = name;
        this.render();
      }, `ol-tab-${name}`);
      b.setAttribute('role', 'tab');
      b.setAttribute('aria-selected', String(this.section === name));
      tabs.append(b);
    }
    this.root.append(tabs);

    if (this.section === 'events' || this.section === 'script') {
      this.root.append(this.filters());
    }
    const body = el('div', 'ol-body');
    this.root.append(body);
    switch (this.section) {
      case 'findings':
        this.renderFindings(body);
        break;
      case 'cast':
        this.renderCast(body);
        break;
      case 'events':
        this.renderEvents(body);
        break;
      case 'script':
        this.renderScript(body);
        break;
      case 'variables':
        this.renderVariables(body);
        break;
    }
  }

  filters() {
    const d = this.data;
    const box = el('div', 'ol-filters');
    const episodes = select([['', t('outline.allEpisodes')], ...d.episodes.map(e => [e.title, e.title])], this.episode, v => {
      this.episode = v;
      this.render();
    });
    episodes.id = 'ol-episode';
    box.append(label(t('outline.episode'), episodes));
    if (this.section === 'script') {
      const speakers = [...new Set(d.script.map(l => l.speaker || ''))].filter(Boolean);
      const sp = select([['', t('outline.allSpeakers')], ...speakers.map(s => [s, s])], this.speaker, v => {
        this.speaker = v;
        this.render();
      });
      sp.id = 'ol-speaker';
      box.append(label(t('outline.speaker'), sp));
    }
    return box;
  }

  link(file, line) {
    const text = `${file}:${line}`;
    if (!this.hooks.openLine) {
      return el('span', 'ol-where', text);
    }
    const a = el('a', 'ol-where', text);
    a.href = '#';
    a.addEventListener('click', ev => {
      ev.preventDefault();
      this.hooks.openLine(file, line);
    });
    return a;
  }

  renderFindings(body) {
    const d = this.data;
    if (d.findings.length === 0) {
      body.append(el('p', 'hint', t('outline.noFindings')));
      return;
    }
    const ul = el('ul', 'ol-findings');
    for (const f of d.findings) {
      const li = el('li');
      li.append(this.link(f.file, f.line), ' ', f.message);
      ul.append(li);
    }
    body.append(ul);
  }

  renderCast(body) {
    const d = this.data;
    const rows = d.episodes.map(e => [e.title, list(e.joins), list(e.launches), list(e.leaves), list(e.enemies), list(e.next)]);
    body.append(table([t('outline.col.episode'), t('outline.col.joins'), t('outline.col.launches'), t('outline.col.leaves'), t('outline.col.enemies'), t('outline.col.next')], rows));
    // 人物ごとに、どの話でどれだけ話すか
    const people = new Map();
    for (const e of d.episodes) {
      for (const [who, n] of Object.entries(e.speakers ?? {})) {
        if (!people.has(who)) {
          people.set(who, []);
        }
        people.get(who).push(`${e.title} (${n})`);
      }
    }
    body.append(el('h3', '', t('outline.lines')));
    body.append(table([t('outline.col.speaker'), t('outline.col.lines')], [...people].map(([who, eps]) => [who, eps.join('、')])));
  }

  renderEvents(body) {
    const d = this.data;
    const rows = [];
    for (const e of d.episodes.filter(x => !this.episode || x.title === this.episode)) {
      for (const ev of e.events) {
        rows.push([e.title, `${ev.always ? '*' : ''}${ev.label}`, ev.when, list(ev.conditions), list(ev.actions) + (ev.talkLines ? `（${t('outline.talkLines', ev.talkLines)}）` : ''), this.link(ev.file, ev.line)]);
      }
    }
    body.append(table([t('outline.col.episode'), t('outline.col.label'), t('outline.col.when'), t('outline.col.conditions'), t('outline.col.actions'), t('outline.col.where')], rows));
  }

  renderScript(body) {
    const d = this.data;
    const lines = d.script.filter(l => (!this.episode || l.episode === this.episode) && (!this.speaker || l.speaker === this.speaker));
    const wrap = el('div', 'ol-script');
    const host = el('div', 'ol-talk');
    host.hidden = true;
    if (!this.hooks.previewTalk) {
      this.talk = new MessageWindowPreview(host, { loadImage: this.hooks.loadImage });
    }
    const tbl = table([t('outline.col.episode'), t('outline.col.event'), t('outline.col.speaker'), t('outline.col.text'), t('outline.col.where')], []);
    const tbody = tbl.querySelector('tbody');
    for (const l of lines) {
      const tr = el('tr', l.kind === 'telop' ? 'ol-telop' : '');
      tr.tabIndex = 0;
      for (const v of [l.episode, l.event ?? '', l.kind === 'telop' ? t('outline.telop') : (l.speaker || t('outline.narration')), l.text]) {
        tr.append(el('td', '', v));
      }
      const where = el('td');
      where.append(this.link(l.file, l.line));
      tr.append(where);
      const pick = () => this.selectLine(l, tr, host);
      tr.addEventListener('click', ev => {
        if (!(ev.target instanceof HTMLAnchorElement)) {
          pick();
        }
      });
      tr.addEventListener('keydown', ev => {
        if (ev.key === 'Enter' || ev.key === ' ') {
          ev.preventDefault();
          pick();
        }
      });
      if (this.selected === l) {
        tr.setAttribute('aria-selected', 'true');
      }
      tbody.append(tr);
    }
    wrap.append(host, tbl);
    body.append(el('p', 'hint', t(this.hooks.previewTalk ? 'outline.scriptHintEditor' : 'outline.scriptHint')), wrap);
    if (this.selected && lines.includes(this.selected)) {
      this.showTalk(this.selected, host);
    }
  }

  selectLine(line, tr, host) {
    this.selected = line;
    tr.parentElement.querySelectorAll('tr[aria-selected]').forEach(x => x.removeAttribute('aria-selected'));
    tr.setAttribute('aria-selected', 'true');
    if (this.hooks.previewTalk) {
      this.hooks.previewTalk(line, this.data);
    } else {
      this.showTalk(line, host);
    }
  }

  /** 選んだ行を、会話の見た目（メッセージウィンドウ）で出す。話す人の愛称と顔は、データ（faces）から */
  showTalk(line, host) {
    if (!this.talk) {
      return;
    }
    const face = (this.data.faces ?? {})[line.speaker ?? ''];
    const speaker = line.kind === 'telop' ? 'システム' : (line.speaker ?? '');
    this.talk.setMessages([{ speaker, nickname: face?.nickname || (line.kind === 'telop' ? '' : speaker), bitmap: face?.bitmap ?? '', text: line.text }]);
    host.hidden = false;
  }

  renderVariables(body) {
    const d = this.data;
    const sites = xs => {
      const span = el('span');
      xs.slice(0, 5).forEach((s, i) => {
        if (i > 0) {
          span.append('、');
        }
        span.append(this.link(s.file, s.line));
      });
      if (xs.length > 5) {
        span.append(` …(${xs.length})`);
      }
      return span;
    };
    body.append(table([t('outline.col.variable'), t('outline.col.scope'), t('outline.col.sets'), t('outline.col.reads')],
      d.variables.map(v => [v.name, v.global ? 'Global' : '', sites(v.sets), sites(v.reads)])));
  }
}

function el(tag, cls = '', text) {
  const e = document.createElement(tag);
  if (cls) {
    e.className = cls;
  }
  if (text !== undefined) {
    e.textContent = text;
  }
  return e;
}

function button(text, onClick, id) {
  const b = el('button', '', text);
  b.type = 'button';
  if (id) {
    b.id = id;
  }
  b.addEventListener('click', onClick);
  return b;
}

function label(text, control) {
  const l = el('label', '', `${text} `);
  l.append(control);
  return l;
}

function select(options, value, onChange) {
  const s = el('select');
  for (const [v, text] of options) {
    const o = el('option', '', text);
    o.value = v;
    s.append(o);
  }
  s.value = value;
  s.addEventListener('change', () => onChange(s.value));
  return s;
}

function list(xs) {
  return (xs ?? []).join('、');
}

function table(headers, rows) {
  const tbl = el('table', 'ol-table');
  const tr = el('tr');
  headers.forEach(h => tr.append(el('th', '', h)));
  const thead = el('thead');
  thead.append(tr);
  const tbody = el('tbody');
  for (const row of rows) {
    const r = el('tr');
    for (const v of row) {
      const td = el('td');
      if (v instanceof Node) {
        td.append(v);
      } else {
        td.textContent = v ?? '';
      }
      r.append(td);
    }
    tbody.append(r);
  }
  tbl.append(thead, tbody);
  return tbl;
}
