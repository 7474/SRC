// SRC#Studio のデータのプレビュー（7474/SRC#1643）。データのファイル（robot.txt・unit.txt・pilot.txt）のユニット・パイロットを、
// 画像・能力値・特殊能力・武器・アビリティ・スペシャルパワーで見せる。ファイルの読み書きは使う側（studio.js・vscode-preview.js）が行う。
//
// const preview = new DataPreview(element, {
//   loadImage: async (kind, bitmap) => ImageBitmap | null,
//   onSelect: entry => {},
//   lines: () => string[],                       // 表で直すときの、いまのファイルの行（無ければ表で直せない）
//   editLines: edits => {},                      // 表で直した行 [{ line（1 始まり）, text }] を書き換える
// });
// preview.show(result, line);   // result は studio-core.js の previewData の結果。line（1 始まり）を含むデータを選ぶ
import { t } from './i18n.js';
import { WEAPON_FIELDS, ABILITY_FIELDS, splitLine, replaceField, isContinued } from './table-edit.js';
import { compareUnit, comparePilot, analysisSections } from './compare.js';

export class DataPreview {
  constructor(element, options) {
    this.element = element;
    this.options = options;
    this.result = null;
    this.selected = null;
    element.classList.add('data-preview');
    element.innerHTML = `
      <div class="dp-errors" hidden></div>
      <div class="dp-body">
        <ul class="dp-list" role="listbox"></ul>
        <article class="dp-card" aria-live="polite"></article>
      </div>`;
  }

  /** プレビューを出す。line を含むデータがあればそれを、無ければ前に選んだもの（同じ名前）か最初のものを選ぶ */
  show(result, line) {
    this.result = result;
    const errors = this.element.querySelector('.dp-errors');
    errors.hidden = result.errors.length === 0;
    errors.textContent = result.errors.join('\n');
    const byLine = line ? result.entries.find(e => line >= e.startLine && line <= e.endLine) : null;
    const previous = this.selected && result.entries.find(e => e.name === this.selected.name);
    this.renderList();
    this.select(byLine ?? previous ?? result.entries[0] ?? null);
  }

  renderList() {
    const list = this.element.querySelector('.dp-list');
    list.innerHTML = '';
    for (const entry of this.result.entries) {
      const li = document.createElement('li');
      li.role = 'option';
      li.textContent = entry.name;
      li.title = `${entry.startLine}${t('preview.line')}`;
      li.addEventListener('click', () => {
        this.select(entry);
        this.options.onSelect?.(entry);
      });
      list.append(li);
    }
    if (this.result.entries.length === 0) {
      const li = document.createElement('li');
      li.className = 'dp-empty';
      li.textContent = this.result.kind ? t('preview.empty') : t('preview.notData');
      list.append(li);
    }
  }

  select(entry) {
    this.selected = entry;
    const items = [...this.element.querySelectorAll('.dp-list li')];
    items.forEach(li => li.setAttribute('aria-selected', String(entry != null && li.textContent === entry.name)));
    const card = this.element.querySelector('.dp-card');
    if (!entry) {
      card.innerHTML = '';
      return;
    }
    card.innerHTML = this.result.kind === 'pilot' ? pilotCard(entry) : unitCard(entry);
    this.setupTable(card, entry);
    this.setupCompare(card, entry);
    const canvas = card.querySelector('canvas');
    if (canvas && entry.bitmap && entry.bitmap !== '-.bmp') {
      this.options.loadImage?.(this.result.kind, entry.bitmap).then(img => {
        if (img && this.selected === entry) {
          canvas.width = img.width;
          canvas.height = img.height;
          canvas.getContext('2d').drawImage(img, 0, 0);
          canvas.hidden = false;
        }
      }, () => {});
    }
  }
}

// ---- 表で直す（7474/SRC#1644） ----

DataPreview.prototype.setupTable = function (card, entry) {
  const lines = this.options.lines?.();
  if (!lines || this.result.kind !== 'unit') {
    return;
  }
  const rows = [
    ...entry.weapons.filter(w => w.line).map(w => ({ kind: 'weapon', line: w.line })),
    ...entry.abilities.filter(a => a.line).map(a => ({ kind: 'ability', line: a.line })),
  ];
  if (rows.length === 0) {
    return;
  }
  // 表で直している間は、直すたびにプレビューを作り直しても表を開いたままにする
  const openTable = () => {
    this.tableOpenFor = entry.name;
    return renderTable(rows, this.options.lines(), edits => this.options.editLines(edits));
  };
  const weaponsTable = card.querySelector('.dp-weapons');
  if (this.tableOpenFor === entry.name) {
    if (weaponsTable) {
      weaponsTable.before(openTable());
    } else {
      card.append(openTable());
    }
    return;
  }
  const button = document.createElement('button');
  button.type = 'button';
  button.className = 'dp-table-toggle';
  button.textContent = t('table.edit');
  if (weaponsTable) {
    weaponsTable.before(button);
  } else {
    card.append(button);
  }
  button.addEventListener('click', () => button.replaceWith(openTable()));
};

/** 武器・アビリティの行を、欄ごとの入力欄の表にする */
function renderTable(rows, lines, editLines) {
  const box = document.createElement('div');
  box.className = 'dp-table-edit';
  for (const kind of ['weapon', 'ability']) {
    const list = rows.filter(r => r.kind === kind);
    if (list.length === 0) {
      continue;
    }
    const fields = kind === 'weapon' ? WEAPON_FIELDS : ABILITY_FIELDS;
    const table = document.createElement('table');
    table.className = 'dp-edit';
    table.innerHTML = `<caption>${t(kind === 'weapon' ? 'preview.weapons' : 'preview.abilities')}</caption><thead><tr><th></th>${fields.map(f => `<th>${esc(t(`table.${kind}.${f}`))}</th>`).join('')}</tr></thead>`;
    const body = document.createElement('tbody');
    list.forEach((row, i) => {
      const tr = document.createElement('tr');
      const text = lines[row.line - 1] ?? '';
      const locked = isContinued(lines, row.line);
      const move = document.createElement('td');
      move.className = 'dp-move';
      for (const [label, delta] of [['↑', -1], ['↓', 1]]) {
        const b = document.createElement('button');
        b.type = 'button';
        b.textContent = label;
        b.title = t(delta < 0 ? 'table.up' : 'table.down');
        const other = list[i + delta];
        b.disabled = locked || !other || isContinued(lines, other.line);
        // 並べ替え: 隣の行と入れ替える
        b.addEventListener('click', () => editLines([{ line: row.line, text: lines[other.line - 1] }, { line: other.line, text }]));
        move.append(b);
      }
      tr.append(move);
      const { fields: values } = splitLine(text);
      fields.forEach((f, index) => {
        const td = document.createElement('td');
        const input = document.createElement('input');
        input.value = values[index] ?? '';
        input.disabled = locked || index >= values.length;
        input.dataset.field = f;
        input.size = Math.max(3, Math.min(14, input.value.length + 1));
        input.title = locked ? t('table.continued') : `${row.line}${t('preview.line')}`;
        input.addEventListener('change', () => {
          const current = lines[row.line - 1] ?? '';
          editLines([{ line: row.line, text: replaceField(current, index, input.value.trim()) }]);
        });
        td.append(input);
        tr.append(td);
      });
      body.append(tr);
    });
    table.append(body);
    box.append(table);
  }
  const note = document.createElement('p');
  note.className = 'dp-none';
  note.textContent = t('table.note');
  box.append(note);
  return box;
}

// ---- 比べる（7474/SRC#1644） ----

DataPreview.prototype.setupCompare = function (card, entry) {
  if (!this.options.compare) {
    return;
  }
  const section = document.createElement('section');
  section.className = 'dp-compare';
  section.innerHTML = `<h3>${t('compare.title')}</h3><p class="dp-none">${t('compare.hint')}</p>`;
  const select = document.createElement('select');
  const run = async () => {
    const body = section.querySelector('.dp-compare-body') ?? section.appendChild(document.createElement('div'));
    body.className = 'dp-compare-body';
    body.textContent = t('loading');
    try {
      const rows = this.result.kind === 'pilot' ? await comparePilot(entry, select.value) : await compareUnit(entry, select.value);
      body.innerHTML = `<table class="dp-stats">${rows.map(r => `<tr><th>${esc(r.label)}</th><td>${esc(r.value)}</td><td class="dp-bar-cell">${r.percentile == null ? '-' : `<span class="dp-bar"><span style="width:${(100 - r.percentile).toFixed(0)}%"></span></span> ${t('compare.top', r.percentile.toFixed(0))}`}</td></tr>`).join('')}</table><p class="dp-none">${esc(rows.note ?? '')}</p>`;
    } catch (e) {
      body.textContent = t('compare.failed', String(e.message ?? e));
    }
  };
  analysisSections().then(sections => {
    select.innerHTML = sections.map(s => `<option>${esc(s)}</option>`).join('');
    select.addEventListener('change', run);
    section.querySelector('h3').append(' ', select);
    run();
  }, e => {
    section.querySelector('.dp-none').textContent = t('compare.failed', String(e.message ?? e));
  });
  card.append(section);
};

function esc(s) {
  return String(s ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c]);
}

function table(rows) {
  return `<table class="dp-stats"><tbody>${rows.map(([k, v]) => `<tr><th>${esc(k)}</th><td>${esc(v)}</td></tr>`).join('')}</tbody></table>`;
}

function tags(title, list) {
  return `<h3>${esc(title)}</h3>` + (list.length ? `<ul class="dp-tags">${list.map(x => `<li>${esc(x)}</li>`).join('')}</ul>` : `<p class="dp-none">${t('preview.none')}</p>`);
}

function header(entry) {
  return `<header><canvas class="dp-image" hidden></canvas><div><h2>${esc(entry.name)}</h2>
    <p>${esc(entry.nickname)}・${esc(entry.class)}・${t('preview.lines', entry.startLine, entry.endLine)}</p></div></header>`;
}

function unitCard(u) {
  const weapons = u.weapons.length === 0 ? `<p class="dp-none">${t('preview.none')}</p>` : `<table class="dp-weapons"><thead><tr>
      <th>${t('preview.weapon')}</th><th>${t('preview.power')}</th><th>${t('preview.range')}</th><th>${t('preview.hit')}</th>
      <th>${t('preview.bullet')}</th><th>EN</th><th>${t('preview.morale')}</th><th>${t('preview.adaption')}</th><th>CT</th><th>${t('preview.attributes')}</th>
    </tr></thead><tbody>${u.weapons.map(w => `<tr><td>${esc(w.name)}</td><td>${w.power}</td><td>${esc(w.range)}</td><td>${signed(w.hit)}</td>
      <td>${w.bullet || '-'}</td><td>${w.en || '-'}</td><td>${w.morale || '-'}</td><td>${esc(w.adaption)}</td><td>${signed(w.critical)}</td><td>${esc(w.class)}</td></tr>`).join('')}</tbody></table>`;
  return header(u) + table([
    ['HP', u.hp], ['EN', u.en], [t('preview.armor'), u.armor], [t('preview.mobility'), u.mobility], [t('preview.speed'), u.speed],
    [t('preview.size'), u.size], [t('preview.transportation'), u.transportation], [t('preview.adaption'), u.adaption], [t('preview.exp'), u.exp],
  ]) + tags(t('preview.features'), u.features) + `<h3>${t('preview.weapons')}</h3>` + weapons
    + tags(t('preview.abilities'), u.abilities.map(a => `${a.name}（${a.range}・${a.stock || '-'}）${a.class ? ' ' + a.class : ''}`));
}

function pilotCard(p) {
  return header(p) + table([
    ...Object.entries(p.stats), ['SP', p.sp], [t('preview.personality'), p.personality], [t('preview.adaption'), p.adaption], [t('preview.exp'), p.exp], [t('preview.sex'), p.sex],
  ]) + tags(t('preview.specialPowers'), p.specialPowers) + tags(t('preview.skills'), p.skills);
}

function signed(v) {
  return v > 0 ? `+${v}` : String(v);
}
