// かんたん作成（7474/SRC#1659）。好きな作品のキャラを選び、マップに置き、セリフを書いて、すぐ SRC#Web で遊ぶ。
// 作るのは普通のイベント（.eve）・マップ（.map）と、選んだ作品のデータ（Data/作品/…）の zip。VS Code でそのまま続きを書ける。
// 作品のデータは 7474/SRC-Data（GSC データパック）の index.json と各ファイルを読む（studio.json の data）。
// 場面のサンプル（マップと、置く位置・作品・ユニット・セリフ）は、場面のカタログ（studio.json の scenes）から読む。
// どちらも SRC# のリポジトリの外に置き、差し替えられる（書式は docs/tools/quick-create.md）
import { buildQuickBattle, decodeText, previewData } from './studio-core.js';
import { CLASS_COLORS } from './map-editor.js';
import { makeZip } from './zip.js';
import { storeForWeb, studioConfig, webIsSameOrigin, webPlayUrl } from './web-handoff.js';
import { t } from './i18n.js';

/** SRCCore が作品のフォルダから読むファイル（SRC.data.cs の LoadDataDirectory）と、作品ごとのインクルード */
const DATA_FILES = new Set(['alias.txt', 'sp.txt', 'mind.txt', 'pilot.txt', 'non_pilot.txt', 'robot.txt', 'unit.txt',
  'pilot_message.txt', 'pilot_dialog.txt', 'effect.txt', 'animation.txt', 'ext_animation.txt', 'item.txt', 'include.eve']);
const DRAFT_KEY = 'srcstudio.quick';
const CELL = 26;

const esc = s => String(s ?? '').replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]));

function newBattle() {
  return {
    title: '', width: 20, height: 15,
    /** マップの出どころ（'' は平地、scene:カタログ#id・folder:パス・file:名前）とテキスト、場面の置く位置 */
    mapSource: '', mapText: undefined, spots: null,
    allies: [], enemies: [],
    opening: [], victory: [], defeat: [],
  };
}

export class QuickBuilder {
  /**
   * @param {HTMLElement} root
   * @param {{ systemTexts: () => Promise<{name: string, text: string}[]>, maps: () => string[], readMap: (path: string) => Promise<Uint8Array>, parseMap: Function, terrains: () => {id: number, class: string}[], download: Function }} host
   */
  constructor(root, host) {
    this.root = root;
    this.host = host;
    this.index = null;
    /** 選んだ作品: { key, title, base, path, folder, files: [{ name, bytes }], units: [], pilots: [] } */
    this.titles = [];
    this.battle = newBattle();
    /** 場面のカタログから読んだ場面（{ ...場面, base: カタログの URL }） */
    this.scenes = null;
    /** 読んだマップのマス（parseMap の tiles）。下書きには残さず、mapText から作り直す */
    this.mapTiles = null;
    this.selected = null;
    this.render();
  }

  // ---- 場面のサンプル ----

  async loadScenes() {
    if (this.scenes) {
      return this.scenes;
    }
    const config = await studioConfig();
    const scenes = [];
    for (const url of config.scenes ?? []) {
      try {
        const base = new URL(url, new URL('./', import.meta.url));
        const response = await fetch(base);
        if (!response.ok) {
          continue;
        }
        for (const scene of (await response.json()).scenes ?? []) {
          if (scene?.id && scene.map) {
            scenes.push({ ...scene, base: base.toString() });
          }
        }
      } catch {
        // 読めないカタログは飛ばす（置かれていない・ネットワークが無い）
      }
    }
    this.scenes = scenes;
    return scenes;
  }

  /** マップのテキストを場面に使う。読めなければ理由 */
  async useMap(source, text) {
    const map = await this.host.parseMap(new TextEncoder().encode(text));
    if (map.error) {
      return map.error;
    }
    Object.assign(this.battle, { mapSource: source, mapText: text, width: map.width, height: map.height });
    this.mapTiles = map.tiles;
    this.clampUnits();
    return null;
  }

  usePlain() {
    Object.assign(this.battle, { mapSource: '', mapText: undefined, spots: null });
    this.mapTiles = null;
  }

  /** 場面のサンプルを使う。作品・ユニット・セリフがあれば、尋ねてから入れる */
  async applyScene(scene) {
    const response = await fetch(new URL(scene.map, scene.base));
    if (!response.ok) {
      throw new Error(`${scene.map}: ${response.status}`);
    }
    const error = await this.useMap(`scene:${scene.base}#${scene.id}`, decodeText(new Uint8Array(await response.arrayBuffer())));
    if (error) {
      throw new Error(error);
    }
    const spots = { allies: scene.allies ?? [], enemies: scene.enemies ?? [] };
    this.battle.spots = spots;
    const preset = (scene.units?.allies?.length || scene.units?.enemies?.length || scene.titles?.length || scene.talk);
    const b = this.battle;
    const empty = !b.allies.length && !b.enemies.length && ![b.opening, b.victory, b.defeat].some(x => x.length);
    if (preset && (empty || confirm(t('quick.scenePreset', scene.title)))) {
      if (scene.titles?.length) {
        const index = await this.loadIndex();
        for (const path of scene.titles) {
          const entry = index.find(x => x.path === path);
          if (entry) {
            await this.addTitle(entry);
          }
        }
      }
      const unit = u => ({ unit: u.unit ?? '', pilot: u.pilot ?? '', level: u.level ?? 10, x: u.x ?? 1, y: u.y ?? 1 });
      b.allies = (scene.units?.allies ?? []).map(unit);
      b.enemies = (scene.units?.enemies ?? []).map(unit);
      for (const key of ['opening', 'victory', 'defeat']) {
        b[key] = (scene.talk?.[key] ?? []).map(x => ({ speaker: x.speaker ?? '', text: x.text ?? '' }));
      }
      if (!b.title && scene.title) {
        b.title = scene.title;
      }
    } else {
      // 置いたユニットを、場面の置く位置へ順に移す
      for (const side of ['allies', 'enemies']) {
        b[side].forEach((u, i) => {
          const spot = spots[side][i];
          if (spot) {
            u.x = spot.x;
            u.y = spot.y;
          }
        });
      }
    }
    this.clampUnits();
  }

  // ---- 作品 ----

  async loadIndex() {
    if (this.index) {
      return this.index;
    }
    const config = await studioConfig();
    const response = await fetch(new URL('index.json', config.data));
    if (!response.ok) {
      throw new Error(`${response.status} ${response.statusText}`);
    }
    this.index = (await response.json()).titles ?? [];
    return this.index;
  }

  async addTitle(entry) {
    const key = entry.path;
    if (this.titles.some(x => x.key === key)) {
      return;
    }
    const config = await studioConfig();
    const fetchFolder = async e => {
      const files = [];
      for (const f of e.files.filter(x => DATA_FILES.has(x.toLowerCase()))) {
        const url = new URL(e.path.split('/').map(encodeURIComponent).join('/') + '/' + encodeURIComponent(f), config.data);
        const response = await fetch(url);
        if (!response.ok) {
          throw new Error(`${f}: ${response.status}`);
        }
        files.push({ name: f.toLowerCase(), bytes: new Uint8Array(await response.arrayBuffer()) });
      }
      return files;
    };
    this.status(t('quick.loadingTitle', entry.title));
    try {
      // 同じデータパックの System（共通の別名・アイテム・スペシャルパワー）も一緒に置く
      const system = this.index.find(x => x.base === entry.base && x.title === 'System');
      if (system && entry.title !== 'System' && !this.titles.some(x => x.key === system.path)) {
        this.titles.push(await this.makeTitle(system, await fetchFolder(system), true));
      }
      this.titles.push(await this.makeTitle(entry, await fetchFolder(entry), false));
      this.status('');
    } catch (e) {
      this.status(t('quick.titleFailed', entry.title, e.message ?? e), 'error');
    }
    this.saveDraft();
    this.render();
  }

  /** 作品のデータを読み、ユニット・パイロットの一覧を作る（プレビューと同じ SRCCore の読み込み） */
  async makeTitle(entry, files, system) {
    const base = entry.base.split('/').at(-1);
    let folder = entry.title === 'System' ? `${base}_System` : entry.title;
    if (this.titles.some(x => x.folder === folder)) {
      folder = `${folder}_${base}`;
    }
    const title = { key: entry.path, title: entry.title, base: entry.base, folder, files, units: [], pilots: [], system };
    const before = await this.host.systemTexts();
    for (const f of files) {
      if (!/^(robot|unit|pilot)\.txt$/.test(f.name)) {
        continue;
      }
      const result = await previewData(f.name, decodeText(f.bytes), before);
      for (const e of result.entries ?? []) {
        (result.kind === 'pilot' ? title.pilots : title.units).push({ name: e.name, nickname: e.nickname, class: e.class, pilots: e.pilots });
      }
    }
    return title;
  }

  removeTitle(key) {
    this.titles = this.titles.filter(x => x.key !== key);
    // ほかに使う作品が無い System は外す
    for (const s of this.titles.filter(x => x.system)) {
      if (!this.titles.some(x => !x.system && x.base === s.base)) {
        this.titles = this.titles.filter(x => x !== s);
      }
    }
    this.saveDraft();
    this.render();
  }

  units() {
    return this.titles.flatMap(x => x.units.map(u => ({ ...u, title: x.title })));
  }

  pilots() {
    return this.titles.flatMap(x => x.pilots.map(p => ({ ...p, title: x.title })));
  }

  // ---- ユニット ----

  addUnit(side) {
    const list = this.battle[side];
    const place = this.freeCell(side);
    list.push({ unit: '', pilot: '', level: side === 'allies' ? 10 : 5, x: place.x, y: place.y });
    this.selected = { side, index: list.length - 1 };
    this.saveDraft();
    this.render();
  }

  /** 場面の置く位置のうち空いているところ。無ければ、味方は左から、敵は右から空いているマス */
  freeCell(side) {
    const used = new Set([...this.battle.allies, ...this.battle.enemies].map(u => `${u.x},${u.y}`));
    const spot = (this.battle.spots?.[side] ?? []).find(p => !used.has(`${p.x},${p.y}`));
    if (spot) {
      return { x: spot.x, y: spot.y };
    }
    const { width, height } = this.battle;
    const columns = side === 'allies' ? [...Array(width).keys()].map(i => i + 1) : [...Array(width).keys()].map(i => width - i);
    const midY = Math.ceil(height / 2);
    const rows = [...Array(height).keys()].map(i => i + 1).sort((a, b) => Math.abs(a - midY) - Math.abs(b - midY));
    for (const x of columns.slice(1)) {
      for (const y of rows) {
        if (!used.has(`${x},${y}`)) {
          return { x, y };
        }
      }
    }
    return { x: 1, y: 1 };
  }

  // ---- 書き出し ----

  battleJson() {
    const b = this.battle;
    return JSON.stringify({
      title: b.title || t('quick.defaultTitle'),
      dataTitles: this.titles.map(x => x.folder),
      width: b.width,
      height: b.height,
      map: b.mapSource ? (b.mapText ?? '') : '',
      allies: b.allies,
      enemies: b.enemies,
      opening: b.opening,
      victory: b.victory,
      defeat: b.defeat,
    });
  }

  /** zip の中身。エラーがあれば { errors } */
  async build() {
    const result = await buildQuickBattle(this.battleJson());
    if (result.errors.length) {
      return { errors: result.errors };
    }
    const files = Object.entries(result.files).map(([path, data]) => ({ path, data }));
    for (const title of this.titles) {
      for (const f of title.files) {
        files.push({ path: `Data/${title.folder}/${f.name}`, data: f.bytes });
      }
    }
    const eve = Object.keys(result.files).find(x => x.endsWith('.eve'));
    return { errors: [], name: eve.replace(/\.eve$/, ''), zip: makeZip(files) };
  }

  async play() {
    // ポップアップを止められないよう、押したときに窓を開いてから中身を作る
    const win = window.open('', '_blank');
    const built = await this.build();
    if (built.errors.length) {
      win?.close();
      this.status(built.errors.join('\n'), 'error');
      return;
    }
    const config = await studioConfig();
    const name = `${built.name}.zip`;
    await storeForWeb(name, built.zip, config.requires);
    const url = await webPlayUrl(name);
    if (win) {
      win.location.href = url;
    } else {
      location.href = url;
    }
    this.status(t('quick.played', name), 'info');
  }

  async save() {
    const built = await this.build();
    if (built.errors.length) {
      this.status(built.errors.join('\n'), 'error');
      return;
    }
    this.host.download(`${built.name}.zip`, built.zip);
    this.status(t('quick.saved', `${built.name}.zip`), 'info');
  }

  // ---- 下書き（このブラウザだけ。作品のデータは開き直したときに読み直す） ----

  saveDraft() {
    try {
      localStorage.setItem(DRAFT_KEY, JSON.stringify({ battle: this.battle, titles: this.titles.filter(x => !x.system).map(x => x.key) }));
    } catch {
      // 残せなくても続ける
    }
  }

  async restoreDraft() {
    let draft;
    try {
      draft = JSON.parse(localStorage.getItem(DRAFT_KEY) ?? 'null');
    } catch {
      draft = null;
    }
    if (!draft) {
      return;
    }
    Object.assign(this.battle, draft.battle);
    if (this.battle.mapText && (await this.useMap(this.battle.mapSource, this.battle.mapText))) {
      this.usePlain();
    }
    this.render();
    if (draft.titles?.length) {
      try {
        const index = await this.loadIndex();
        for (const key of draft.titles) {
          const entry = index.find(x => x.path === key);
          if (entry) {
            await this.addTitle(entry);
          }
        }
      } catch (e) {
        this.status(t('quick.indexFailed', e.message ?? e), 'error');
      }
    }
  }

  reset() {
    if (!confirm(t('quick.resetConfirm'))) {
      return;
    }
    this.titles = [];
    this.battle = newBattle();
    this.mapTiles = null;
    this.selected = null;
    this.saveDraft();
    this.render();
  }

  status(text, kind = 'info') {
    const box = this.root.querySelector('.quick-status');
    if (box) {
      box.textContent = text;
      box.className = `quick-status ${text ? kind : ''}`;
      box.hidden = !text;
    }
  }

  // ---- 画面 ----

  render() {
    const b = this.battle;
    const units = this.units();
    const pilots = this.pilots();
    const speakers = [...new Set([...b.allies, ...b.enemies].map(u => u.pilot).filter(Boolean))];
    const maps = this.host.maps();
    const scenes = this.scenes ?? [];
    if (!this.scenes) {
      this.loadScenes().then(list => {
        if (list.length) {
          this.render();
        }
      });
    }
    const sideRows = side => b[side].map((u, i) => `
      <tr data-side="${side}" data-index="${i}" class="${this.selected?.side === side && this.selected.index === i ? 'selected' : ''}">
        <td><input class="q-unit" list="q-units" value="${esc(u.unit)}" placeholder="${esc(t('quick.unit'))}" aria-label="${esc(t('quick.unit'))}"></td>
        <td><input class="q-pilot" list="q-pilots" value="${esc(u.pilot)}" placeholder="${esc(t('quick.pilot'))}" aria-label="${esc(t('quick.pilot'))}"></td>
        <td><input class="q-level" type="number" min="1" max="99" value="${u.level}" aria-label="${esc(t('quick.level'))}"></td>
        <td class="q-pos">${u.x},${u.y}</td>
        <td><button type="button" class="q-remove" aria-label="${esc(t('quick.remove'))}">×</button></td>
      </tr>`).join('');
    const talkRows = scene => b[scene].map((line, i) => `
      <div class="q-line" data-scene="${scene}" data-index="${i}">
        <select class="q-speaker" aria-label="${esc(t('quick.speaker'))}">
          <option value="" ${line.speaker ? '' : 'selected'}>${esc(t('quick.narration'))}</option>
          ${speakers.map(s => `<option ${s === line.speaker ? 'selected' : ''}>${esc(s)}</option>`).join('')}
          ${line.speaker && !speakers.includes(line.speaker) ? `<option selected>${esc(line.speaker)}</option>` : ''}
        </select>
        <textarea class="q-text" rows="2" aria-label="${esc(t('quick.text'))}">${esc(line.text)}</textarea>
        <button type="button" class="q-remove-line" aria-label="${esc(t('quick.remove'))}">×</button>
      </div>`).join('');

    this.root.innerHTML = `
      <p class="hint">${esc(t('quick.intro'))}</p>
      <div class="quick-status" hidden></div>
      <section class="q-step">
        <h2>1. ${esc(t('quick.stepTitles'))}</h2>
        <p class="hint">${esc(t('quick.titlesHint'))}</p>
        <input type="search" class="q-search" placeholder="${esc(t('quick.searchTitles'))}" aria-label="${esc(t('quick.searchTitles'))}">
        <ul class="q-results"></ul>
        <ul class="q-chosen">${this.titles.filter(x => !x.system).map(x => `
          <li data-key="${esc(x.key)}">${esc(x.title)} <span class="hint">${esc(t('quick.counts', x.units.length, x.pilots.length))}</span>
          <button type="button" class="q-remove-title" aria-label="${esc(t('quick.remove'))}">×</button></li>`).join('')}</ul>
        <datalist id="q-units">${units.map(u => `<option value="${esc(u.name)}">${esc(u.title)}${u.class ? ' / ' + esc(u.class) : ''}</option>`).join('')}</datalist>
        <datalist id="q-pilots">${pilots.map(p => `<option value="${esc(p.name)}">${esc(p.nickname || '')} ${esc(p.title)}</option>`).join('')}</datalist>
      </section>
      <section class="q-step">
        <h2>2. ${esc(t('quick.stepUnits'))}</h2>
        <h3>${esc(t('quick.allies'))}</h3>
        <table class="q-table"><tbody>${sideRows('allies')}</tbody></table>
        <button type="button" class="q-add" data-side="allies">${esc(t('quick.addAlly'))}</button>
        <h3>${esc(t('quick.enemies'))}</h3>
        <table class="q-table"><tbody>${sideRows('enemies')}</tbody></table>
        <button type="button" class="q-add" data-side="enemies">${esc(t('quick.addEnemy'))}</button>
      </section>
      <section class="q-step">
        <h2>3. ${esc(t('quick.stepMap'))}</h2>
        <label>${esc(t('quick.scenarioTitle'))} <input class="q-title" value="${esc(b.title)}" placeholder="${esc(t('quick.defaultTitle'))}"></label>
        <label>${esc(t('quick.map'))} <select class="q-map">
          <option value="">${esc(t('quick.plainMap'))}</option>
          ${scenes.length ? `<optgroup label="${esc(t('quick.scenes'))}">${scenes.map(sc => {
            const value = `scene:${sc.base}#${sc.id}`;
            return `<option value="${esc(value)}" ${value === b.mapSource ? 'selected' : ''}>${esc(sc.title ?? sc.id)}${sc.description ? ' — ' + esc(sc.description) : ''}</option>`;
          }).join('')}</optgroup>` : ''}
          ${maps.length ? `<optgroup label="${esc(t('quick.folderMaps'))}">${maps.map(m => `<option value="${esc('folder:' + m)}" ${'folder:' + m === b.mapSource ? 'selected' : ''}>${esc(m)}</option>`).join('')}</optgroup>` : ''}
          ${b.mapSource?.startsWith('file:') ? `<option value="${esc(b.mapSource)}" selected>${esc(b.mapSource.slice(5))}</option>` : ''}
        </select></label>
        <button type="button" class="q-map-file">${esc(t('quick.mapFile'))}</button>
        <input type="file" class="q-map-input" accept=".map" hidden>
        <span class="q-size" ${b.mapSource ? 'hidden' : ''}>
          <label>${esc(t('map.width'))} <input class="q-width" type="number" min="5" max="50" value="${b.width}"></label>
          <label>${esc(t('map.height'))} <input class="q-height" type="number" min="5" max="50" value="${b.height}"></label>
        </span>
        <p class="hint">${esc(t('quick.placeHint'))}</p>
        <div class="q-board"><canvas class="q-canvas"></canvas></div>
      </section>
      <section class="q-step">
        <h2>4. ${esc(t('quick.stepTalk'))}</h2>
        <p class="hint">${esc(t('quick.talkHint'))}</p>
        ${['opening', 'victory', 'defeat'].map(scene => `
          <h3>${esc(t(`quick.scene.${scene}`))}</h3>
          ${talkRows(scene)}
          <button type="button" class="q-add-line" data-scene="${scene}">${esc(t('quick.addLine'))}</button>`).join('')}
      </section>
      <section class="q-step">
        <h2>5. ${esc(t('quick.stepPlay'))}</h2>
        <button type="button" class="q-play primary">${esc(t('quick.play'))}</button>
        <button type="button" class="q-save">${esc(t('quick.save'))}</button>
        <button type="button" class="q-reset">${esc(t('quick.reset'))}</button>
        <p class="hint q-play-hint">${esc(t('quick.playHint'))}</p>
      </section>`;
    this.bind();
    this.drawBoard();
    webIsSameOrigin().then(same => {
      const button = this.root.querySelector('.q-play');
      if (button && !same) {
        button.disabled = true;
        this.root.querySelector('.q-play-hint').textContent = t('quick.playOtherSite');
      }
    });
  }

  bind() {
    const $ = s => this.root.querySelector(s);
    const search = $('.q-search');
    search.addEventListener('input', async () => {
      const words = search.value.trim().split(/\s+/).filter(Boolean);
      const list = $('.q-results');
      if (!words.length) {
        list.innerHTML = '';
        return;
      }
      let index;
      try {
        index = await this.loadIndex();
      } catch (e) {
        this.status(t('quick.indexFailed', e.message ?? e), 'error');
        return;
      }
      const hits = index.filter(x => x.title !== 'System' && x.files.some(f => /^(robot|unit|pilot)\.txt$/i.test(f))
        && words.every(w => `${x.title} ${x.tags.join(' ')}`.includes(w))).slice(0, 30);
      list.innerHTML = hits.map((x, i) => `<li><button type="button" data-i="${i}">${esc(x.title)}</button> <span class="hint">${esc(x.base)}</span></li>`).join('')
        || `<li class="hint">${esc(t('quick.noTitles'))}</li>`;
      list.querySelectorAll('button').forEach(button => button.addEventListener('click', () => this.addTitle(hits[Number(button.dataset.i)])));
    });
    this.root.querySelectorAll('.q-remove-title').forEach(button =>
      button.addEventListener('click', () => this.removeTitle(button.closest('li').dataset.key)));

    this.root.querySelectorAll('.q-add').forEach(button => button.addEventListener('click', () => this.addUnit(button.dataset.side)));
    this.root.querySelectorAll('.q-table tr').forEach(row => {
      const unit = this.battle[row.dataset.side][Number(row.dataset.index)];
      const select = () => {
        if (this.selected?.side !== row.dataset.side || this.selected?.index !== Number(row.dataset.index)) {
          this.selected = { side: row.dataset.side, index: Number(row.dataset.index) };
          this.root.querySelectorAll('.q-table tr').forEach(r => r.classList.toggle('selected', r === row));
          this.drawBoard();
        }
      };
      row.addEventListener('focusin', select);
      row.addEventListener('click', select);
      row.querySelector('.q-unit').addEventListener('change', ev => {
        unit.unit = ev.target.value.trim();
        this.saveDraft();
        this.drawBoard();
      });
      row.querySelector('.q-pilot').addEventListener('change', ev => {
        unit.pilot = ev.target.value.trim();
        this.saveDraft();
        // セリフの話し手の候補を変える
        this.render();
      });
      row.querySelector('.q-level').addEventListener('change', ev => {
        unit.level = Math.max(1, Math.min(99, Number(ev.target.value) || 1));
        this.saveDraft();
      });
      row.querySelector('.q-remove').addEventListener('click', ev => {
        ev.stopPropagation();
        this.battle[row.dataset.side].splice(Number(row.dataset.index), 1);
        this.selected = null;
        this.saveDraft();
        this.render();
      });
    });

    $('.q-title').addEventListener('change', ev => {
      this.battle.title = ev.target.value;
      this.saveDraft();
    });
    const resize = () => {
      this.battle.width = Math.max(5, Math.min(50, Number($('.q-width').value) || 20));
      this.battle.height = Math.max(5, Math.min(50, Number($('.q-height').value) || 15));
      this.clampUnits();
      this.saveDraft();
      this.drawBoard();
    };
    $('.q-width').addEventListener('change', resize);
    $('.q-height').addEventListener('change', resize);
    $('.q-map').addEventListener('change', async ev => {
      const value = ev.target.value;
      try {
        if (!value) {
          this.usePlain();
        } else if (value.startsWith('scene:')) {
          await this.applyScene(this.scenes.find(sc => `scene:${sc.base}#${sc.id}` === value));
        } else if (value.startsWith('folder:')) {
          const error = await this.useMap(value, decodeText(await this.host.readMap(value.slice(7))));
          if (error) {
            throw new Error(error);
          }
          this.battle.spots = null;
        }
        this.status('');
      } catch (e) {
        this.usePlain();
        this.status(t('quick.mapFailed', e.message ?? e), 'error');
      }
      this.saveDraft();
      this.render();
    });
    $('.q-map-file').addEventListener('click', () => $('.q-map-input').click());
    $('.q-map-input').addEventListener('change', async ev => {
      const file = ev.target.files[0];
      if (!file) {
        return;
      }
      const error = await this.useMap(`file:${file.name}`, decodeText(new Uint8Array(await file.arrayBuffer())));
      if (error) {
        this.usePlain();
        this.status(t('quick.mapFailed', error), 'error');
      } else {
        this.battle.spots = null;
        this.status('');
      }
      this.saveDraft();
      this.render();
    });
    const canvas = $('.q-canvas');
    canvas.addEventListener('click', ev => {
      const rect = canvas.getBoundingClientRect();
      const x = Math.floor((ev.clientX - rect.left) / (rect.width / this.battle.width)) + 1;
      const y = Math.floor((ev.clientY - rect.top) / (rect.height / this.battle.height)) + 1;
      this.clickCell(x, y);
    });

    this.root.querySelectorAll('.q-add-line').forEach(button => button.addEventListener('click', () => {
      const scene = button.dataset.scene;
      const last = this.battle[scene].at(-1);
      this.battle[scene].push({ speaker: last?.speaker ?? '', text: '' });
      this.saveDraft();
      this.render();
      this.root.querySelector(`.q-line[data-scene="${scene}"]:last-of-type .q-text`)?.focus();
    }));
    this.root.querySelectorAll('.q-line').forEach(div => {
      const line = this.battle[div.dataset.scene][Number(div.dataset.index)];
      div.querySelector('.q-speaker').addEventListener('change', ev => {
        line.speaker = ev.target.value;
        this.saveDraft();
      });
      div.querySelector('.q-text').addEventListener('input', ev => {
        line.text = ev.target.value;
        this.saveDraft();
      });
      div.querySelector('.q-remove-line').addEventListener('click', () => {
        this.battle[div.dataset.scene].splice(Number(div.dataset.index), 1);
        this.saveDraft();
        this.render();
      });
    });

    $('.q-play').addEventListener('click', () => this.play().catch(e => this.status(String(e.message ?? e), 'error')));
    $('.q-save').addEventListener('click', () => this.save().catch(e => this.status(String(e.message ?? e), 'error')));
    $('.q-reset').addEventListener('click', () => this.reset());
  }

  clampUnits() {
    for (const u of [...this.battle.allies, ...this.battle.enemies]) {
      u.x = Math.min(u.x, this.battle.width);
      u.y = Math.min(u.y, this.battle.height);
    }
  }

  /** 置いたユニットのマスなら選び、空いたマスなら選んでいるユニットを動かす */
  clickCell(x, y) {
    for (const side of ['allies', 'enemies']) {
      const index = this.battle[side].findIndex(u => u.x === x && u.y === y);
      if (index >= 0) {
        this.selected = { side, index };
        this.root.querySelectorAll('.q-table tr').forEach(r =>
          r.classList.toggle('selected', r.dataset.side === side && Number(r.dataset.index) === index));
        this.drawBoard();
        return;
      }
    }
    const unit = this.selected && this.battle[this.selected.side][this.selected.index];
    if (unit) {
      unit.x = x;
      unit.y = y;
      const row = this.root.querySelector(`.q-table tr[data-side="${this.selected.side}"][data-index="${this.selected.index}"] .q-pos`);
      if (row) {
        row.textContent = `${x},${y}`;
      }
      this.saveDraft();
      this.drawBoard();
    }
  }

  drawBoard() {
    const canvas = this.root.querySelector('.q-canvas');
    if (!canvas) {
      return;
    }
    const { width, height } = this.battle;
    const ratio = window.devicePixelRatio || 1;
    canvas.width = width * CELL * ratio;
    canvas.height = height * CELL * ratio;
    canvas.style.width = `${width * CELL}px`;
    const ctx = canvas.getContext('2d');
    ctx.scale(ratio, ratio);
    ctx.fillStyle = CLASS_COLORS['陸'];
    ctx.fillRect(0, 0, width * CELL, height * CELL);
    if (this.mapTiles) {
      // 地形タイプの色で描く（画像は SRC#Web の汎用グラフィック集で出る）
      const terrains = new Map(this.host.terrains().map(x => [x.id, x]));
      for (let y = 0; y < height; y++) {
        for (let x = 0; x < width; x++) {
          const info = terrains.get(this.mapTiles[(y * width + x) * 2]);
          ctx.fillStyle = info ? (CLASS_COLORS[info.class] ?? '#999') : '#999';
          ctx.fillRect(x * CELL, y * CELL, CELL, CELL);
        }
      }
    }
    // 場面の置く位置
    for (const [side, color] of [['allies', 'rgba(47,111,214,0.35)'], ['enemies', 'rgba(208,57,62,0.35)']]) {
      ctx.fillStyle = color;
      for (const p of this.battle.spots?.[side] ?? []) {
        ctx.fillRect((p.x - 1) * CELL + 6, (p.y - 1) * CELL + 6, CELL - 12, CELL - 12);
      }
    }
    ctx.strokeStyle = 'rgba(0,0,0,0.2)';
    ctx.lineWidth = 1;
    for (let x = 0; x <= width; x++) {
      ctx.beginPath();
      ctx.moveTo(x * CELL + 0.5, 0);
      ctx.lineTo(x * CELL + 0.5, height * CELL);
      ctx.stroke();
    }
    for (let y = 0; y <= height; y++) {
      ctx.beginPath();
      ctx.moveTo(0, y * CELL + 0.5);
      ctx.lineTo(width * CELL, y * CELL + 0.5);
      ctx.stroke();
    }
    ctx.font = `bold ${CELL * 0.42}px system-ui, sans-serif`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    for (const side of ['allies', 'enemies']) {
      this.battle[side].forEach((u, i) => {
        const left = (u.x - 1) * CELL;
        const top = (u.y - 1) * CELL;
        ctx.fillStyle = side === 'allies' ? '#2f6fd6' : '#d0393e';
        ctx.fillRect(left + 2, top + 2, CELL - 4, CELL - 4);
        // 水の地形（青）の上でも見分けられるよう白で縁取る
        ctx.strokeStyle = '#ffffff';
        ctx.lineWidth = 1.5;
        ctx.strokeRect(left + 2.5, top + 2.5, CELL - 5, CELL - 5);
        if (this.selected?.side === side && this.selected.index === i) {
          ctx.strokeStyle = '#ffd400';
          ctx.lineWidth = 3;
          ctx.strokeRect(left + 1.5, top + 1.5, CELL - 3, CELL - 3);
        }
        ctx.fillStyle = '#ffffff';
        ctx.fillText((u.unit || '?').slice(0, 2), left + CELL / 2, top + CELL / 2 + 1);
      });
    }
  }
}
