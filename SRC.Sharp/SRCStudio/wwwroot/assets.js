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
    this.kindFilter = '';
    this.query = '';
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

    const filters = el('div', 'assets-filters');
    const kindSelect = el('select', 'assets-kind-filter');
    kindSelect.setAttribute('aria-label', t('assets.col.kind'));
    for (const [val, text] of [['', t('assets.kind.all')], ['image', t('assets.kind.image')], ['sound', t('assets.kind.sound')], ['map', t('assets.kind.map')]]) {
      const opt = el('option', '', text);
      opt.value = val;
      if (val === this.kindFilter) {
        opt.selected = true;
      }
      kindSelect.append(opt);
    }
    kindSelect.addEventListener('change', () => {
      this.kindFilter = kindSelect.value;
      this.renderContent();
    });

    const search = el('input', 'assets-search');
    search.type = 'search';
    search.value = this.query;
    search.placeholder = t('assets.search');
    search.setAttribute('aria-label', t('assets.search'));
    search.addEventListener('input', () => {
      this.query = search.value;
      this.renderContent();
    });
    filters.append(kindSelect, search);

    toolbar.append(actions, filters);
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

    const content = el('div', 'assets-content');
    this.root.append(content);
    this.renderContent(content);
  }

  renderContent(content = this.root.querySelector('.assets-content')) {
    if (!content || !this.data) {
      return;
    }
    content.innerHTML = '';
    const assets = this.data.assets ?? [];
    const dynamic = this.data.dynamic ?? [];
    const unused = this.data.unused ?? [];
    const q = this.query.trim().toLowerCase();
    const matchKind = a => !this.kindFilter || a.kind === this.kindFilter;
    const matchText = text => !q || String(text ?? '').toLowerCase().includes(q);

    if (this.filter === 'all' || this.filter === 'missing' || this.filter === 'external') {
      const base = this.filter === 'missing' ? assets.filter(a => a.missing)
        : this.filter === 'external' ? assets.filter(a => !a.missing && a.foundIn !== 'scenario')
        : assets;
      const list = base.filter(a => matchKind(a) && matchText(`${a.name} ${a.foundPath ?? ''} ${(a.usages ?? []).map(u => u.file).join(' ')}`));
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
          const nameTd = el('td', 'assets-name');
          if (a.kind === 'image' && !a.missing && this.hooks.loadImage && a.foundPath) {
            const thumb = el('canvas', 'assets-thumb');
            thumb.width = 24;
            thumb.height = 24;
            thumb.hidden = true;
            thumb.title = a.foundPath;
            nameTd.append(thumb);
            this.hooks.loadImage(a.foundIn, a.foundPath).then(img => {
              if (!img || !thumb.isConnected) {
                return;
              }
              const ctx = thumb.getContext('2d');
              const scale = Math.min(24 / img.width, 24 / img.height);
              const w = Math.max(1, Math.round(img.width * scale));
              const h = Math.max(1, Math.round(img.height * scale));
              ctx.imageSmoothingEnabled = false;
              ctx.clearRect(0, 0, 24, 24);
              ctx.drawImage(img, Math.floor((24 - w) / 2), Math.floor((24 - h) / 2), w, h);
              thumb.hidden = false;
            }, () => {});
          }
          nameTd.append(document.createTextNode(a.name));
          tr.append(nameTd);
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
      content.append(sec);
    }

    if (this.filter === 'all' || this.filter === 'dynamic') {
      const list = dynamic.filter(d => matchText(`${d.file} ${d.text}`));
      const sec = el('section', 'assets-block');
      sec.append(el('h3', '', t('assets.section.dynamic')));
      if (list.length === 0) {
        sec.append(el('p', 'hint', t('assets.none')));
      } else {
        const ul = el('ul', 'assets-list');
        for (const d of list) {
          const li = el('li');
          li.append(el('span', 'assets-loc', `${d.file}:${d.line} `), el('code', '', d.text));
          ul.append(li);
        }
        sec.append(ul);
      }
      content.append(sec);
    }

    if (this.filter === 'all' || this.filter === 'unused') {
      const list = unused.filter(u => matchText(u));
      const sec = el('section', 'assets-block');
      sec.append(el('h3', '', t('assets.section.unused')));
      sec.append(el('p', 'hint', t('assets.unusedNote')));
      if (list.length === 0) {
        sec.append(el('p', 'hint', t('assets.none')));
      } else {
        const ul = el('ul', 'assets-list');
        for (const u of list) {
          ul.append(el('li', 'assets-name', u));
        }
        sec.append(ul);
      }
      content.append(sec);
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
