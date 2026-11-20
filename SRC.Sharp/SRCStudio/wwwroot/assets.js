// SRC#Studio の素材の一覧画面（7474/SRC#1654）。素材の入手・再配布は扱わず、揃っているかの管理だけを行う
import { t } from './i18n.js';

const ROOT_LABELS = {
  scenario: () => t('assets.root.scenario'),
  extdata: () => t('assets.root.extdata'),
  extdata2: () => t('assets.root.extdata2'),
  app: () => t('assets.root.app'),
};

const KIND_LABELS = {
  image: () => t('assets.kind.image'),
  sound: () => t('assets.kind.sound'),
  map: () => t('assets.kind.map'),
};

export class AssetsView {
  constructor(root, hooks) {
    this.root = root;
    this.hooks = hooks;
    this.data = null;
    this.readmeText = '';
    this.filter = 'all';
    this.showReadme = false;
  }

  setData(report, readmeText = '') {
    this.data = report;
    this.readmeText = readmeText;
    this.render();
  }

  render() {
    this.root.innerHTML = '';
    if (!this.data) {
      return;
    }

    const toolbar = el('div', 'assets-toolbar');
    const actions = el('div', 'assets-actions');
    const refreshBtn = el('button', '', t('assets.refresh'));
    refreshBtn.type = 'button';
    refreshBtn.id = 'assets-refresh';
    refreshBtn.addEventListener('click', () => this.hooks.onRefresh?.());

    const readmeBtn = el('button', '', t('assets.copyReadme'));
    readmeBtn.type = 'button';
    readmeBtn.id = 'assets-readme-btn';
    readmeBtn.addEventListener('click', async () => {
      this.showReadme = true;
      let copied = false;
      try {
        if (navigator.clipboard?.writeText) {
          await navigator.clipboard.writeText(this.readmeText);
          copied = true;
        }
      } catch {
        copied = false;
      }
      this.render();
      this.hooks.onMessage?.(t(copied ? 'assets.readmeCopied' : 'assets.readmeReady'), 'info');
    });
    actions.append(refreshBtn, readmeBtn);
    toolbar.append(actions);
    this.root.append(toolbar);

    const rootsLine = (this.data.roots ?? [])
      .map(r => (ROOT_LABELS[r.kind]?.() ?? r.kind) + (r.name ? ` (${r.name})` : ''))
      .join(' → ');
    this.root.append(el('p', 'hint assets-roots', t('assets.searchOrder', rootsLine)));

    const assets = this.data.assets ?? [];
    const dynamic = this.data.dynamic ?? [];
    const unused = this.data.unused ?? [];
    const externalCount = assets.filter(a => !a.missing && a.foundIn !== 'scenario').length;

    const summary = el('div', 'assets-summary');
    summary.append(
      this.chip('all', t('assets.filter.all'), assets.length),
      this.chip('missing', t('assets.filter.missing'), this.data.missing ?? 0, (this.data.missing ?? 0) > 0),
      this.chip('external', t('assets.filter.external'), externalCount),
      this.chip('dynamic', t('assets.filter.dynamic'), dynamic.length),
      this.chip('unused', t('assets.filter.unused'), unused.length),
    );
    this.root.append(summary);

    if (this.showReadme) {
      const box = el('div', 'assets-readme-box');
      const head = el('div', 'assets-readme-head');
      head.append(el('strong', '', t('assets.readmeHeading')));
      const closeBtn = el('button', '', t('quick.close'));
      closeBtn.type = 'button';
      closeBtn.addEventListener('click', () => {
        this.showReadme = false;
        this.render();
      });
      head.append(closeBtn);
      const area = el('textarea');
      area.id = 'assets-readme-text';
      area.readOnly = true;
      area.rows = Math.min(14, Math.max(4, this.readmeText.split('\n').length + 1));
      area.value = this.readmeText;
      box.append(head, area);
      this.root.append(box);
    }

    if (this.filter === 'all' || this.filter === 'missing' || this.filter === 'external') {
      const list = this.filter === 'missing' ? assets.filter(a => a.missing)
        : this.filter === 'external' ? assets.filter(a => !a.missing && a.foundIn !== 'scenario')
        : assets;
      const sec = el('section', 'assets-block');
      sec.append(el('h3', '', t('assets.section.referenced')));
      if (list.length === 0) {
        sec.append(el('p', 'hint', t('assets.none')));
      } else {
        const table = el('table', 'assets-table');
        const thead = el('thead');
        const hr = el('tr');
        for (const h of [t('assets.col.name'), t('assets.col.kind'), t('assets.col.foundIn'), t('assets.col.usages')]) {
          hr.append(el('th', '', h));
        }
        thead.append(hr);
        const tbody = el('tbody');
        for (const a of list) {
          const tr = el('tr');
          tr.dataset.asset = a.name;
          tr.append(el('td', 'assets-name', a.name));
          tr.append(el('td', '', KIND_LABELS[a.kind]?.() ?? a.kind));
          const where = el('td');
          if (a.missing) {
            where.append(el('span', 'assets-missing', t('assets.missing')));
          } else {
            where.textContent = `${ROOT_LABELS[a.foundIn]?.() ?? a.foundIn}: ${a.foundPath}`;
          }
          tr.append(where);
          const usages = el('td', 'assets-usages');
          usages.textContent = (a.usages ?? []).map(u => `${u.file}:${u.line}`).join(', ');
          tr.append(usages);
          tbody.append(tr);
        }
        table.append(thead, tbody);
        sec.append(table);
      }
      this.root.append(sec);
    }

    if (this.filter === 'all' || this.filter === 'dynamic') {
      const sec = el('section', 'assets-block');
      sec.append(el('h3', '', t('assets.section.dynamic')));
      if (dynamic.length === 0) {
        sec.append(el('p', 'hint', t('assets.none')));
      } else {
        const ul = el('ul', 'assets-list');
        for (const d of dynamic) {
          const li = el('li');
          li.append(el('span', 'assets-loc', `${d.file}:${d.line} `), el('code', '', d.text));
          ul.append(li);
        }
        sec.append(ul);
      }
      this.root.append(sec);
    }

    if (this.filter === 'all' || this.filter === 'unused') {
      const sec = el('section', 'assets-block');
      sec.append(el('h3', '', t('assets.section.unused')));
      sec.append(el('p', 'hint', t('assets.unusedNote')));
      if (unused.length === 0) {
        sec.append(el('p', 'hint', t('assets.none')));
      } else {
        const ul = el('ul', 'assets-list');
        for (const u of unused) {
          ul.append(el('li', 'assets-name', u));
        }
        sec.append(ul);
      }
      this.root.append(sec);
    }
  }

  chip(id, label, count, bad = false) {
    const btn = el('button', `assets-chip${bad ? ' bad' : ''}`, `${label} ${count}`);
    btn.type = 'button';
    btn.dataset.filter = id;
    btn.setAttribute('aria-pressed', String(this.filter === id));
    btn.addEventListener('click', () => {
      this.filter = id;
      this.render();
    });
    return btn;
  }
}

function el(tag, className = '', text = undefined) {
  const e = document.createElement(tag);
  if (className) {
    e.className = className;
  }
  if (text !== undefined) {
    e.textContent = text;
  }
  return e;
}
