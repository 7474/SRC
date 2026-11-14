// SRC#Studio のデータのプレビュー（7474/SRC#1643）。データのファイル（robot.txt・unit.txt・pilot.txt）のユニット・パイロットを、
// 画像・能力値・特殊能力・武器・アビリティ・スペシャルパワーで見せる。ファイルの読み書きは使う側（studio.js・vscode-preview.js）が行う。
//
// const preview = new DataPreview(element, { loadImage: async (kind, bitmap) => ImageBitmap | null, onSelect: entry => {} });
// preview.show(result, line);   // result は studio-core.js の previewData の結果。line（1 始まり）を含むデータを選ぶ
import { t } from './i18n.js';

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
