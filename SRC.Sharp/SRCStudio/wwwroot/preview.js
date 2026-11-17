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
import { MessageWindowPreview } from './message-window.js';

export class DataPreview {
  constructor(element, options) {
    this.element = element;
    this.options = options;
    this.result = null;
    this.selected = null;
    this.selectedItemLine = null;
    this.cursorLine = null;
    this.messageView = 'pilot';
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
    this.cursorLine = line ?? null;
    const errors = this.element.querySelector('.dp-errors');
    errors.hidden = result.errors.length === 0;
    errors.textContent = result.errors.join('\n');
    const byLine = line ? result.entries.find(e => line >= e.startLine && line <= e.endLine) : null;
    const previous = this.selected && result.entries.find(e => e.name === this.selected.name);
    if (line && byLine) {
      const itemAtLine = byLine.items?.find(it => it.line === line || it.messages?.some(m => m.line === line));
      if (itemAtLine) {
        this.selectedItemLine = itemAtLine.line;
      } else if (!byLine.items?.some(it => it.line === this.selectedItemLine)) {
        this.selectedItemLine = byLine.items?.[0]?.line ?? null;
      }
    }
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
      li.dataset.name = entry.name;
      li.title = `${entry.startLine}${t('preview.line')}`;
      li.addEventListener('click', () => {
        this.selectedItemLine = entry.items?.[0]?.line ?? null;
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
    items.forEach(li => li.setAttribute('aria-selected', String(entry != null && li.dataset.name === entry.name)));
    const card = this.element.querySelector('.dp-card');
    const isMsg = this.result.kind === 'message' || this.result.kind === 'dialog';
    if (!entry && !(isMsg && this.result.missingPilots?.length)) {
      card.innerHTML = '';
      return;
    }
    if (isMsg) {
      this.renderMessageCard(card, entry);
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

// ---- メッセージ・ダイアログデータのプレビューと充実度の一覧（7474/SRC#1651） ----

DataPreview.prototype.renderMessageCard = function (card, entry) {
  card.innerHTML = '';
  const tabs = document.createElement('div');
  tabs.className = 'dp-msg-tabs';
  for (const [mode, labelKey] of [['pilot', 'msg.tabPilot'], ['all', 'msg.tabAll']]) {
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.textContent = t(labelKey);
    btn.setAttribute('aria-pressed', String(this.messageView === mode));
    btn.addEventListener('click', () => {
      this.messageView = mode;
      this.renderMessageCard(card, entry);
    });
    tabs.append(btn);
  }
  card.append(tabs);

  if (this.messageView === 'all' || !entry) {
    this.renderAllPilotsMatrix(card);
    return;
  }

  const head = document.createElement('header');
  const face = document.createElement('canvas');
  face.className = 'dp-image';
  face.hidden = true;
  const info = document.createElement('div');
  const h2 = document.createElement('h2');
  h2.textContent = entry.name;
  const meta = document.createElement('p');
  const parts = [entry.nickname && entry.nickname !== entry.name ? entry.nickname : null, entry.class || null,
    entry.units?.length ? `${t('msg.unit')}: ${entry.units.join('・')}` : null,
    t('preview.lines', entry.startLine, entry.endLine)].filter(Boolean);
  meta.textContent = parts.join('・');
  info.append(h2, meta);
  head.append(face, info);
  card.append(head);

  if (entry.bitmap && entry.bitmap !== '-.bmp') {
    this.options.loadImage?.('pilot', entry.bitmap).then(img => {
      if (img && this.selected === entry) {
        face.width = img.width;
        face.height = img.height;
        face.getContext('2d').drawImage(img, 0, 0);
        face.hidden = false;
      }
    }, () => {});
  }

  // メッセージウィンドウのプレビュー
  const previewSection = document.createElement('section');
  previewSection.className = 'dp-msg-window-section';
  const previewTitle = document.createElement('h3');
  previewTitle.textContent = t('msg.previewTitle');
  const windowHost = document.createElement('div');
  previewSection.append(previewTitle, windowHost);
  card.append(previewSection);

  const mw = new MessageWindowPreview(windowHost, { loadImage: this.options.loadImage });
  const selectPreviewItem = (item) => {
    this.selectedItemLine = item?.line ?? null;
    for (const row of card.querySelectorAll('.dp-msg-item')) {
      row.setAttribute('aria-selected', String(item != null && Number(row.dataset.line) === item.line));
    }
    if (!item) {
      mw.setMessages([]);
      return;
    }
    mw.setMessages(item.messages, sampleVarsForMessage(entry, item));
  };

  // このパイロットのシチュエーション充実度
  const covSection = document.createElement('section');
  covSection.className = 'dp-coverage';
  const renderSlots = (titleText, slots) => {
    if (slots.length === 0) return;
    const h3 = document.createElement('h3');
    h3.textContent = titleText;
    const list = document.createElement('div');
    list.className = 'dp-cov-list';
    for (const slot of slots) {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = `dp-cov-chip ${slot.count > 0 ? 'covered' : 'missing'}`;
      const label = document.createElement('span');
      label.textContent = slot.label;
      const badge = document.createElement('span');
      badge.className = 'dp-cov-count';
      badge.textContent = slot.count > 0 ? String(slot.count) : t('msg.missing');
      btn.append(label, badge);
      if (slot.count > 0) {
        btn.title = `${slot.lines[0]}${t('preview.line')}`;
        btn.addEventListener('click', () => {
          const item = entry.items?.find(it => it.line === slot.lines[0]);
          if (item) selectPreviewItem(item);
          this.options.onSelect?.(entry, slot.lines[0]);
        });
      } else if (this.options.insertLines) {
        btn.title = t('msg.addSlot');
        btn.addEventListener('click', () => {
          const snippet = buildSlotSnippet(this.result.kind, entry.name, slot.key);
          this.options.insertLines(entry.lastLine ?? entry.endLine, snippet);
        });
      } else {
        btn.disabled = true;
      }
      list.append(btn);
    }
    covSection.append(h3, list);
  };
  const stdSlots = (entry.coverage ?? []).filter(c => c.kind === 'standard');
  const wpSlots = (entry.coverage ?? []).filter(c => c.kind === 'weapon');
  renderSlots(t('msg.situation'), stdSlots);
  renderSlots(t('msg.weapons'), wpSlots);
  card.append(covSection);

  // セリフの一覧
  const itemsSection = document.createElement('section');
  itemsSection.className = 'dp-msg-items';
  const itemsH3 = document.createElement('h3');
  itemsH3.textContent = `${t('msg.text')}（${t('msg.lineCount', entry.items?.length ?? 0)}）`;
  itemsSection.append(itemsH3);
  if (!entry.items?.length) {
    const p = document.createElement('p');
    p.className = 'dp-none';
    p.textContent = t('msg.noItems');
    itemsSection.append(p);
  } else {
    const tableEl = document.createElement('table');
    tableEl.className = 'dp-weapons dp-msg-table';
    tableEl.innerHTML = `<thead><tr><th>${esc(t('msg.situation'))}</th><th>${esc(t('msg.text'))}</th><th>${esc(t('preview.line') || '#')}</th></tr></thead>`;
    const tbody = document.createElement('tbody');
    for (const item of entry.items) {
      const tr = document.createElement('tr');
      tr.className = 'dp-msg-item';
      tr.dataset.line = String(item.line);
      const tdSit = document.createElement('td');
      tdSit.textContent = item.situation;
      const tdTxt = document.createElement('td');
      tdTxt.className = 'dp-msg-text-cell';
      tdTxt.textContent = item.messages.map(m =>
        this.result.kind === 'dialog' ? `${m.nickname || m.speaker}: ${m.text}` : m.text).join(' / ');
      const tdLine = document.createElement('td');
      tdLine.textContent = String(item.line);
      tr.append(tdSit, tdTxt, tdLine);
      tr.addEventListener('click', () => {
        selectPreviewItem(item);
        this.options.onSelect?.(entry, item.line);
      });
      tbody.append(tr);
    }
    tableEl.append(tbody);
    itemsSection.append(tableEl);
  }
  card.append(itemsSection);

  const initialItem = entry.items?.find(it => it.line === this.selectedItemLine) ?? entry.items?.[0] ?? null;
  selectPreviewItem(initialItem);
};

DataPreview.prototype.renderAllPilotsMatrix = function (card) {
  const stds = this.result.standardSituations ?? [];
  const entries = this.result.entries ?? [];
  const h3 = document.createElement('h3');
  h3.textContent = t('msg.tabAll');
  card.append(h3);

  if (entries.length > 0) {
    const wrap = document.createElement('div');
    wrap.className = 'dp-matrix-wrap';
    const tableEl = document.createElement('table');
    tableEl.className = 'dp-msg-matrix';
    const thead = document.createElement('thead');
    const htr = document.createElement('tr');
    for (const label of [t('msg.pilot'), t('msg.unit'), ...stds, t('msg.weapons')]) {
      const th = document.createElement('th');
      th.textContent = label;
      htr.append(th);
    }
    thead.append(htr);
    tableEl.append(thead);

    const tbody = document.createElement('tbody');
    for (const e of entries) {
      const tr = document.createElement('tr');
      if (this.selected?.name === e.name) tr.className = 'selected';
      const tdPilot = document.createElement('td');
      const pBtn = document.createElement('button');
      pBtn.type = 'button';
      pBtn.className = 'dp-matrix-pilot';
      pBtn.textContent = e.name;
      pBtn.addEventListener('click', () => {
        this.messageView = 'pilot';
        this.selectedItemLine = e.items?.[0]?.line ?? null;
        this.select(e);
        this.options.onSelect?.(e, e.startLine);
      });
      tdPilot.append(pBtn);
      const tdUnit = document.createElement('td');
      tdUnit.textContent = e.units?.join(', ') || '-';
      tr.append(tdPilot, tdUnit);

      for (const s of stds) {
        const slot = e.coverage?.find(c => c.kind === 'standard' && c.key === s);
        const td = document.createElement('td');
        td.className = slot && slot.count > 0 ? 'cov-ok' : 'cov-miss';
        if (slot && slot.count > 0) {
          const btn = document.createElement('button');
          btn.type = 'button';
          btn.className = 'dp-matrix-cell';
          btn.textContent = slot.count > 1 ? String(slot.count) : '○';
          btn.title = `${s}: ${slot.lines[0]}${t('preview.line')}`;
          btn.addEventListener('click', () => {
            this.messageView = 'pilot';
            this.selectedItemLine = slot.lines[0];
            this.select(e);
            this.options.onSelect?.(e, slot.lines[0]);
          });
          td.append(btn);
        } else {
          td.textContent = '—';
        }
        tr.append(td);
      }
      const tdW = document.createElement('td');
      const wSlots = (e.coverage ?? []).filter(c => c.kind === 'weapon');
      if (wSlots.length === 0) {
        tdW.textContent = '-';
      } else {
        const ok = wSlots.filter(c => c.count > 0).length;
        tdW.className = ok === wSlots.length ? 'cov-ok' : (ok > 0 ? 'cov-partial' : 'cov-miss');
        tdW.textContent = `${ok}/${wSlots.length}`;
        tdW.title = wSlots.map(w => `${w.label}: ${w.count > 0 ? w.count : t('msg.missing')}`).join('\n');
      }
      tr.append(tdW);
      tbody.append(tr);
    }
    tableEl.append(tbody);
    wrap.append(tableEl);
    card.append(wrap);
  }

  const missing = this.result.missingPilots ?? [];
  if (missing.length > 0) {
    const mh3 = document.createElement('h3');
    mh3.textContent = `${t('msg.missingPilots')}（${missing.length}）`;
    card.append(mh3);
    const ul = document.createElement('ul');
    ul.className = 'dp-missing-pilots';
    const eofLine = entries.length > 0 ? Math.max(...entries.map(e => e.lastLine ?? e.endLine)) : 0;
    for (const mp of missing) {
      const li = document.createElement('li');
      const label = document.createElement('span');
      const uText = mp.units?.length ? `（${mp.units.join('・')}）` : '';
      label.textContent = `${mp.name}${uText}`;
      li.append(label);
      if (this.options.insertLines) {
        const addBtn = document.createElement('button');
        addBtn.type = 'button';
        addBtn.textContent = t('msg.addPilot');
        addBtn.addEventListener('click', () => {
          const snippet = buildPilotSnippet(this.result.kind, mp.name, mp.weapons);
          this.options.insertLines(eofLine, snippet);
        });
        li.append(addBtn);
      }
      ul.append(li);
    }
    card.append(ul);
  }
};

function sampleVarsForMessage(entry, item) {
  const unitName = entry.units?.[0] ?? 'ホタル';
  const sit = item?.situation ?? '';
  const paren = sit.indexOf('(');
  const sitBase = (paren < 0 ? sit : sit.slice(0, paren)).trim();
  const weaponName = entry.weapons?.find(w => w.name === sitBase)?.name ?? entry.weapons?.[0]?.name ?? 'ビーム';
  return {
    '$(パイロット)': entry.nickname || entry.name || 'パイロット',
    '$(ユニット)': unitName,
    '$(機体)': unitName,
    '$(武器)': weaponName,
    '$(損傷率)': '50',
    '$(相手パイロット)': '敵パイロット',
    '$(相手ユニット)': '敵機',
    '$(相手機体)': '敵機',
    '$(相手武器)': 'ビーム',
    '$(相手損傷率)': '50',
  };
}

function buildSlotSnippet(kind, pilotName, situation) {
  if (kind === 'dialog') {
    return `${situation}\n${pilotName}, ${t('msg.sampleText')}`;
  }
  return `${situation}, ${t('msg.sampleText')}`;
}

function buildPilotSnippet(kind, pilotName, weapons) {
  const lines = ['', pilotName];
  if (kind === 'dialog') {
    lines.push('攻撃', `${pilotName}, ${t('msg.sampleText')}`);
    for (const wp of weapons ?? []) {
      lines.push(wp, `${pilotName}, ${t('msg.sampleText')}`);
    }
  } else {
    lines.push(`攻撃, ${t('msg.sampleText')}`);
    lines.push(`回避, ${t('msg.sampleText')}`);
    lines.push(`破壊, ${t('msg.sampleText')}`);
    for (const wp of weapons ?? []) {
      lines.push(`${wp}, ${t('msg.sampleText')}`);
    }
  }
  return lines.join('\n');
}

