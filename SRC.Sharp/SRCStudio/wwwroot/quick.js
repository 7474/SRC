// かんたん作成（7474/SRC#1659）。好きな作品のキャラを選び、マップに置き、セリフを書いて、すぐ SRC#Web で遊ぶ。
// 作るのは普通のイベント（.eve）・マップ（.map）と、選んだ作品のデータ（Data/作品/…）の zip。VS Code でそのまま続きを書ける。
// 作品のデータは 7474/SRC-Data（GSC データパック）の index.json と各ファイルを読む（studio.json の data）。
// 場面のサンプル（マップと、置く位置・作品・ユニット・セリフ）は、場面のカタログ（studio.json の scenes）から読む。
// どちらも SRC# のリポジトリの外に置き、差し替えられる（書式は docs/tools/quick-create.md）
import { buildQuickBattle, buildQuickCampaign, decodeText, previewData } from './studio-core.js';
import { CLASS_COLORS } from './map-editor.js';
import { makeZip } from './zip.js';
import { storeForWeb, studioConfig, webIsSameOrigin, webPlayUrl } from './web-handoff.js';
import { t } from './i18n.js';

/** SRCCore が作品のフォルダから読むファイル（SRC.data.cs の LoadDataDirectory）と、作品ごとのインクルード */
const DATA_FILES = new Set(['alias.txt', 'sp.txt', 'mind.txt', 'pilot.txt', 'non_pilot.txt', 'robot.txt', 'unit.txt',
  'pilot_message.txt', 'pilot_dialog.txt', 'effect.txt', 'animation.txt', 'ext_animation.txt', 'item.txt', 'include.eve']);
const DRAFT_KEY = 'srcstudio.quick';
const CELL = 26;
// SRC は幅か高さが 15 マス未満のマップをドラッグでスクロールしない（Main.frm の picMain_MouseMove。SRC# も同じ）。
// 平地はこれより小さくせず、選んだマップが小さければ知らせる
const MIN_DRAG_SIZE = 15;

const esc = s => String(s ?? '').replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]));

/** 画像の無いマスの色（地形タイプの色。宇宙・月面の構造物は明るくして見分ける） */
function terrainColor(info) {
  if (!info) {
    return '#999';
  }
  const base = CLASS_COLORS[info.class] ?? '#999';
  if (info.class === '宇宙' && info.hitMod > 0) {
    return info.hitMod >= 30 ? '#8a8fa8' : '#4a4e6e';
  }
  return base;
}

function newBattle() {
  return {
    title: '', width: 20, height: 15,
    /** マップの出どころ（'' は平地、scene:カタログ#id・folder:パス・file:名前）とテキスト、場面の置く位置 */
    mapSource: '', mapText: undefined, spots: null,
    allies: [], enemies: [],
    opening: [], victory: [], defeat: [],
    /** 出来事（7474/SRC#1689）: 勝利・敗北の条件と、戦闘中の出来事（会話・援軍・撤退） */
    goal: { kind: 'annihilate', target: '', turns: 5, x: 0, y: 0 },
    loss: { kind: 'annihilate', target: '', turns: 10 },
    events: [],
  };
}

/** 出来事の初めの形（援軍・会話・撤退） */
function newEvent(kind, battle) {
  const boss = battle.enemies.find(u => (u.bossRank ?? -1) >= 0) ?? battle.enemies[0];
  const ally = battle.allies[0];
  switch (kind) {
    case 'reinforce':
      return { trigger: 'turn', turn: 2, phase: '敵', unit: '', target: '', ratio: 50, talk: [], allies: [], enemies: [], retreat: false };
    case 'retreat':
      return { trigger: 'damaged', turn: 2, phase: '敵', unit: boss?.pilot ?? '', target: '', ratio: 50, talk: [{ speaker: boss?.pilot ?? '', text: '' }], allies: [], enemies: [], retreat: true };
    default:
      return { trigger: 'attack', turn: 2, phase: '敵', unit: ally?.pilot ?? '', target: boss?.pilot ?? '', ratio: 50, talk: [{ speaker: ally?.pilot ?? '', text: '' }], allies: [], enemies: [], retreat: false };
  }
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
    /** 話（7474/SRC#1691）。this.battle は今開いている話。題名は 1 つ目の話の title */
    this.episodes = [this.battle];
    /** 場面のカタログから読んだ場面（{ ...場面, base: カタログの URL }） */
    this.scenes = null;
    /** 読んだマップのマス（parseMap の tiles）。下書きには残さず、mapText から作り直す */
    this.mapTiles = null;
    this.selected = null;
    /** サンプルの場面の一覧を開いているか（既定は閉じる。開いたときに縮図を読む） */
    this.galleryOpen = false;
    /** 場面の縮図（id → マス）と、マップの画像（"画像の名前:番号" → ImageBitmap か null） */
    this.thumbs = new Map();
    this.images = new Map();
    // 知らせは画面の下に出す（作り直しても消さない）
    this.statusBox = document.createElement('div');
    this.statusBox.className = 'quick-status';
    this.statusBox.setAttribute('role', 'status');
    this.statusBox.hidden = true;
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
    const b = this.battle;
    // 平地は 15 マスより小さくしない（選んでいたマップが小さくても）
    Object.assign(b, { mapSource: '', mapText: undefined, spots: null, width: Math.max(MIN_DRAG_SIZE, b.width), height: Math.max(MIN_DRAG_SIZE, b.height) });
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
    const preset = (scene.units?.allies?.length || scene.units?.enemies?.length || scene.titles?.length || scene.talk || scene.events?.length || scene.goal);
    const b = this.battle;
    const empty = !b.allies.length && !b.enemies.length && ![b.opening, b.victory, b.defeat, b.events].some(x => x.length);
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
      const unit = u => ({ unit: u.unit ?? '', pilot: u.pilot ?? '', level: u.level ?? 10, rank: u.rank ?? 0, bossRank: u.bossRank ?? -1, x: u.x ?? 1, y: u.y ?? 1 });
      b.allies = (scene.units?.allies ?? []).map(unit);
      b.enemies = (scene.units?.enemies ?? []).map(unit);
      for (const key of ['opening', 'victory', 'defeat']) {
        b[key] = (scene.talk?.[key] ?? []).map(x => ({ speaker: x.speaker ?? '', text: x.text ?? '' }));
      }
      // 出来事（7474/SRC#1689）: 勝利・敗北の条件と、戦闘中の出来事
      const fresh = newBattle();
      b.goal = { ...fresh.goal, ...(scene.goal ?? {}) };
      b.loss = { ...fresh.loss, ...(scene.loss ?? {}) };
      b.events = (scene.events ?? []).map(e => ({
        ...newEvent('talk', { allies: [], enemies: [] }),
        ...e,
        unit: e.unit ?? '', target: e.target ?? '',
        talk: (e.talk ?? []).map(x => ({ speaker: x.speaker ?? '', text: x.text ?? '' })),
        allies: (e.allies ?? []).map(unit),
        enemies: (e.enemies ?? []).map(unit),
        retreat: Boolean(e.retreat),
      }));
      if (!this.title && scene.title) {
        this.title = scene.title;
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

  /** ユニットの表の鍵（allies・enemies・ev0.allies など）から、その一覧 */
  unitList(key) {
    const m = /^ev(\d+)\.(allies|enemies)$/.exec(key);
    return m ? this.battle.events[Number(m[1])][m[2]] : this.battle[key];
  }

  /** セリフの鍵（opening・victory・defeat・ev0 など）から、その一覧 */
  lineList(key) {
    const m = /^ev(\d+)$/.exec(key);
    return m ? this.battle.events[Number(m[1])].talk : this.battle[key];
  }

  /** 出来事の援軍も含めた、置くユニット（{ key, index, side, unit, event }） */
  allUnits() {
    const b = this.battle;
    const result = [];
    for (const side of ['allies', 'enemies']) {
      b[side].forEach((unit, index) => result.push({ key: side, index, side, unit, event: -1 }));
    }
    b.events.forEach((e, i) => {
      for (const side of ['allies', 'enemies']) {
        e[side].forEach((unit, index) => result.push({ key: `ev${i}.${side}`, index, side, unit, event: i }));
      }
    });
    return result;
  }

  // ---- 話（7474/SRC#1691） ----

  /** 前の話までに出た、名前のある味方のパイロット（次の話で置くと続けて出る） */
  carriedPilots(index = this.episodes.indexOf(this.battle)) {
    const named = p => p && !p.includes('(ザコ)') && !p.includes('(汎用)');
    return [...new Set(this.episodes.slice(0, index).flatMap(b => [...b.allies, ...b.events.flatMap(e => e.allies)]).map(u => u.pilot).filter(named))];
  }

  async showEpisode(index) {
    this.battle = this.episodes[index];
    this.selected = null;
    this.picking = null;
    if (this.battle.mapSource && this.battle.mapText) {
      if (await this.useMap(this.battle.mapSource, this.battle.mapText)) {
        this.usePlain();
      }
    } else {
      this.mapTiles = null;
    }
    this.saveDraft();
    this.render();
  }

  /** 話を足す。前の話の味方とマップを写す（続けて出る味方は Launch になる） */
  async addEpisode() {
    const prev = this.episodes.at(-1);
    const next = newBattle();
    next.width = prev.width;
    next.height = prev.height;
    next.mapSource = prev.mapSource;
    next.mapText = prev.mapText;
    next.spots = prev.spots;
    next.allies = prev.allies.map(u => ({ ...u }));
    this.episodes.push(next);
    await this.showEpisode(this.episodes.length - 1);
    this.status(t('quick.episodeAdded', this.episodes.length), 'info');
  }

  async removeEpisode() {
    const index = this.episodes.indexOf(this.battle);
    if (this.episodes.length < 2 || !confirm(t('quick.episodeRemoveConfirm', index + 1))) {
      return;
    }
    const title = this.title;
    this.episodes.splice(index, 1);
    this.title = title;
    await this.showEpisode(Math.max(0, index - 1));
  }

  // ---- ユニット ----

  addUnit(side) {
    const list = this.unitList(side);
    const party = side.endsWith('allies') ? 'allies' : 'enemies';
    const place = this.freeCell(party);
    list.push({ unit: '', pilot: '', level: party === 'allies' ? 10 : 5, rank: 0, bossRank: -1, x: place.x, y: place.y });
    this.selected = { side, index: list.length - 1 };
    this.saveDraft();
    this.render();
  }

  /** 場面の置く位置のうち空いているところ。無ければ、味方は左から、敵は右から空いているマス */
  freeCell(side) {
    const used = new Set(this.allUnits().map(x => `${x.unit.x},${x.unit.y}`));
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

  /** 題名（キャンペーンの題名。1 つ目の話に持つ） */
  get title() {
    return this.episodes[0].title ?? '';
  }

  set title(value) {
    this.episodes[0].title = value;
  }

  battleJson(b = this.battle) {
    return JSON.stringify(this.episodeObject(b));
  }

  episodeObject(b) {
    return {
      title: this.title || t('quick.defaultTitle'),
      dataTitles: this.titles.map(x => x.folder),
      width: b.width,
      height: b.height,
      map: b.mapSource ? (b.mapText ?? '') : '',
      allies: b.allies,
      enemies: b.enemies,
      opening: b.opening,
      victory: b.victory,
      defeat: b.defeat,
      goal: b.goal,
      loss: b.loss,
      events: b.events,
    };
  }

  /** 話が 2 つ以上なら、キャンペーンの JSON（7474/SRC#1691） */
  campaignJson() {
    return JSON.stringify({
      title: this.title || t('quick.defaultTitle'),
      dataTitles: this.titles.map(x => x.folder),
      episodes: this.episodes.map(b => this.episodeObject(b)),
    });
  }

  /** zip の中身。エラーがあれば { errors } */
  async build() {
    const result = this.episodes.length > 1 ? await buildQuickCampaign(this.campaignJson()) : await buildQuickBattle(this.battleJson());
    if (result.errors.length) {
      return { errors: result.errors };
    }
    const files = Object.entries(result.files).map(([path, data]) => ({ path, data }));
    for (const title of this.titles) {
      for (const f of title.files) {
        files.push({ path: `Data/${title.folder}/${f.name}`, data: f.bytes });
      }
    }
    // zip の名前は題名（中身は srcsharp new のひな形と同じ 第1話.eve など。7474/SRC#1691）
    return { errors: [], name: result.name, zip: makeZip(files) };
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
    const url = new URL(await webPlayUrl(name));
    if (this.episodes.length > 1) {
      // 話がいくつもあれば、第1話から始める
      url.searchParams.set('eve', '第1話.eve');
    }
    if (win) {
      win.location.href = url.toString();
    } else {
      location.href = url.toString();
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
      localStorage.setItem(DRAFT_KEY, JSON.stringify({
        battle: this.battle,
        episodes: this.episodes,
        current: this.episodes.indexOf(this.battle),
        titles: this.titles.filter(x => !x.system).map(x => x.key),
      }));
    } catch {
      // 残せなくても続ける
    }
    this.syncResetButtons();
  }

  hasContent() {
    const b = this.battle;
    return Boolean(
      this.titles.length
      || this.title
      || this.episodes.length > 1
      || b.mapSource
      || b.mapText !== undefined
      || b.spots
      || b.width !== 20
      || b.height !== 15
      || b.allies.length
      || b.enemies.length
      || b.opening.length
      || b.victory.length
      || b.defeat.length
      || b.events?.length
      || (b.goal && b.goal.kind !== 'annihilate')
      || (b.loss && b.loss.kind !== 'annihilate')
    );
  }

  syncResetButtons() {
    const disabled = !this.hasContent();
    this.root.querySelectorAll('.q-reset').forEach(button => { button.disabled = disabled; });
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
    if (Array.isArray(draft.episodes) && draft.episodes.length) {
      this.episodes = draft.episodes.map(e => Object.assign(newBattle(), e));
      this.battle = this.episodes[Math.max(0, Math.min(draft.current ?? 0, this.episodes.length - 1))];
    } else {
      Object.assign(this.battle, draft.battle);
    }
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

  clearDraft() {
    try {
      localStorage.removeItem(DRAFT_KEY);
    } catch {
      // 消せなくても続ける
    }
  }

  reset() {
    if (!confirm(t('quick.resetConfirm'))) {
      return;
    }
    this.titles = [];
    this.battle = newBattle();
    this.episodes = [this.battle];
    this.mapTiles = null;
    this.selected = null;
    this.clearDraft();
    this.render();
    this.status(t('quick.resetDone'), 'info');
  }

  status(text, kind = 'info') {
    const box = this.statusBox;
    clearTimeout(this.statusTimer);
    box.className = `quick-status ${text ? kind : ''}`;
    box.hidden = !text;
    box.innerHTML = text ? `<span>${esc(text)}</span><button type="button" aria-label="${esc(t('quick.close'))}">×</button>` : '';
    box.querySelector('button')?.addEventListener('click', () => this.status(''));
    // 知らせ（読み込み中・保存した）は少しして消す。エラーは閉じるまで残す
    if (text && kind === 'info' && !/…$/.test(text)) {
      this.statusTimer = setTimeout(() => this.status(''), 8000);
    }
  }

  // ---- 画面 ----

  render() {
    const b = this.battle;
    const units = this.units();
    const pilots = this.pilots();
    const speakers = [...new Set(this.allUnits().map(x => x.unit.pilot).filter(Boolean))];
    const enemyPilots = [...new Set(this.allUnits().filter(x => x.side === 'enemies').map(x => x.unit.pilot).filter(Boolean))];
    const allyPilots = [...new Set(this.allUnits().filter(x => x.side === 'allies').map(x => x.unit.pilot).filter(Boolean))];
    const pilotOptions = (list, value, empty) => `${empty !== undefined ? `<option value="" ${value ? '' : 'selected'}>${esc(empty)}</option>` : ''}`
      + list.map(p => `<option ${p === value ? 'selected' : ''}>${esc(p)}</option>`).join('')
      + (value && !list.includes(value) ? `<option selected>${esc(value)}</option>` : '');
    const maps = this.host.maps();
    const scenes = this.scenes ?? [];
    if (!this.scenes) {
      this.loadScenes().then(() => this.render());
    }
    const chosen = this.titles.filter(x => !x.system);
    const carried = this.carriedPilots();
    const galleryOpen = this.galleryOpen ?? false;
    const currentScene = scenes.find(sc => `scene:${sc.base}#${sc.id}` === b.mapSource);
    const sideTable = side => this.unitList(side).length ? `
      <table class="q-table">
        <thead><tr><th>${esc(t('quick.unit'))}</th><th>${esc(t('quick.pilot'))}</th><th>${esc(t('quick.level'))}</th><th title="${esc(t('quick.rankHint'))}">${esc(t('quick.rank'))}</th><th title="${esc(t('quick.bossRankHint'))}">${esc(t('quick.bossRank'))}</th><th>${esc(t('quick.position'))}</th><th></th></tr></thead>
        <tbody>${this.unitList(side).map((u, i) => `
          <tr data-side="${side}" data-index="${i}" class="${this.selected?.side === side && this.selected.index === i ? 'selected' : ''}">
            <td><input class="q-unit" list="q-units" value="${esc(u.unit)}" placeholder="${esc(t('quick.unit'))}" aria-label="${esc(t('quick.unit'))}"></td>
            <td><input class="q-pilot" list="q-pilots" value="${esc(u.pilot)}" placeholder="${esc(t('quick.pilot'))}" aria-label="${esc(t('quick.pilot'))}"></td>
            <td><input class="q-level" type="number" min="1" max="99" value="${u.level}" aria-label="${esc(t('quick.level'))}"></td>
            <td><input class="q-rank" type="number" min="0" max="20" value="${u.rank ?? 0}" aria-label="${esc(t('quick.rank'))}" title="${esc(t('quick.rankHint'))}"></td>
            <td><select class="q-boss" aria-label="${esc(t('quick.bossRank'))}" title="${esc(t('quick.bossRankHint'))}">
              <option value="-1">${esc(t('quick.notBoss'))}</option>
              ${[0, 1, 2, 3, 4, 5].map(n => `<option value="${n}" ${(u.bossRank ?? -1) === n ? 'selected' : ''}>${n}</option>`).join('')}
            </select></td>
            <td class="q-pos" title="${esc(t('quick.positionHint'))}">${u.x},${u.y}</td>
            <td><button type="button" class="q-remove icon" aria-label="${esc(t('quick.remove'))}" title="${esc(t('quick.remove'))}">×</button></td>
          </tr>`).join('')}</tbody>
      </table>` : '';
    const talkRows = scene => this.lineList(scene).map((line, i) => `
      <div class="q-line" data-scene="${scene}" data-index="${i}">
        <select class="q-speaker" aria-label="${esc(t('quick.speaker'))}">
          <option value="" ${line.speaker ? '' : 'selected'}>${esc(t('quick.narration'))}</option>
          ${speakers.map(s => `<option ${s === line.speaker ? 'selected' : ''}>${esc(s)}</option>`).join('')}
          ${line.speaker && !speakers.includes(line.speaker) ? `<option selected>${esc(line.speaker)}</option>` : ''}
        </select>
        <textarea class="q-text" rows="2" aria-label="${esc(t('quick.text'))}" placeholder="${esc(t('quick.text'))}">${esc(line.text)}</textarea>
        <button type="button" class="q-remove-line icon" aria-label="${esc(t('quick.remove'))}" title="${esc(t('quick.remove'))}">×</button>
      </div>`).join('');
    const triggerFields = (e, i) => {
      switch (e.trigger) {
        case 'turn':
          return `<label class="q-field">${esc(t('quick.ev.turn'))} <input class="q-ev-turn" type="number" min="1" max="99" value="${e.turn}"></label>
            <label class="q-field">${esc(t('quick.ev.phase'))} <select class="q-ev-phase">
              <option value="敵" ${e.phase === '敵' ? 'selected' : ''}>${esc(t('quick.enemies'))}</option>
              <option value="味方" ${e.phase === '味方' ? 'selected' : ''}>${esc(t('quick.allies'))}</option></select></label>`;
        case 'attack':
          return `<label class="q-field">${esc(t('quick.ev.unit'))} <select class="q-ev-unit">${pilotOptions(speakers, e.unit, '')}</select></label>
            <label class="q-field">${esc(t('quick.ev.target'))} <select class="q-ev-target">${pilotOptions(speakers, e.target, '')}</select></label>`;
        case 'damaged':
          return `<label class="q-field">${esc(t('quick.ev.unit'))} <select class="q-ev-unit">${pilotOptions(speakers, e.unit, '')}</select></label>
            <label class="q-field">${esc(t('quick.ev.ratio'))} <input class="q-ev-ratio" type="number" min="1" max="99" value="${e.ratio}">%</label>`;
        default:
          return `<label class="q-field">${esc(t('quick.ev.unit'))} <select class="q-ev-unit">${pilotOptions(speakers, e.unit, '')}</select></label>`;
      }
    };
    const eventCard = (e, i) => `
      <fieldset class="q-event" data-event="${i}">
        <legend>${esc(t('quick.ev.title', i + 1))}</legend>
        <div class="q-fields">
          <label class="q-field">${esc(t('quick.ev.trigger'))} <select class="q-ev-trigger">
            ${['turn', 'attack', 'damaged', 'destroyed'].map(k => `<option value="${k}" ${e.trigger === k ? 'selected' : ''}>${esc(t(`quick.ev.trigger.${k}`))}</option>`).join('')}
          </select></label>
          ${triggerFields(e, i)}
          ${e.trigger === 'destroyed' || e.trigger === 'turn' ? '' : `<label class="q-field q-check"><input type="checkbox" class="q-ev-retreat" ${e.retreat ? 'checked' : ''}> ${esc(t('quick.ev.retreat'))}</label>`}
          <button type="button" class="q-remove-event danger">${esc(t('quick.ev.remove'))}</button>
        </div>
        <h4>${esc(t('quick.ev.talk'))}</h4>
        ${talkRows(`ev${i}`)}
        <button type="button" class="q-add-line" data-scene="ev${i}">＋ ${esc(t('quick.addLine'))}</button>
        <h4>${esc(t('quick.ev.reinforce'))}</h4>
        ${e.allies.length ? `<div class="q-side allies">${sideTable(`ev${i}.allies`)}</div>` : ''}
        ${e.enemies.length ? `<div class="q-side enemies">${sideTable(`ev${i}.enemies`)}</div>` : ''}
        <div class="q-actions">
          <button type="button" class="q-add" data-side="ev${i}.allies">＋ ${esc(t('quick.addAlly'))}</button>
          <button type="button" class="q-add" data-side="ev${i}.enemies">＋ ${esc(t('quick.addEnemy'))}</button>
        </div>
      </fieldset>`;
    const goal = b.goal;
    const loss = b.loss;
    const conditions = `
      <div class="q-fields">
        <label class="q-field">${esc(t('quick.goal'))} <select class="q-goal-kind">
          ${['annihilate', 'boss', 'survive', 'reach'].map(k => `<option value="${k}" ${goal.kind === k ? 'selected' : ''}>${esc(t(`quick.goal.${k}`))}</option>`).join('')}
        </select></label>
        ${goal.kind === 'boss' ? `<label class="q-field">${esc(t('quick.goal.target'))} <select class="q-goal-target">${pilotOptions(enemyPilots, goal.target, '')}</select></label>` : ''}
        ${goal.kind === 'survive' ? `<label class="q-field">${esc(t('quick.goal.turns'))} <input class="q-goal-turns" type="number" min="1" max="99" value="${goal.turns}"></label>` : ''}
        ${goal.kind === 'reach' ? `<label class="q-field">${esc(t('quick.goal.who'))} <select class="q-goal-target">${pilotOptions(allyPilots, goal.target, t('quick.allies'))}</select></label>
          <button type="button" class="q-goal-pick" aria-pressed="${this.picking === 'goal'}">${esc(t('quick.goal.pick'))}</button>
          <span class="q-goal-cell">${goal.x && goal.y ? `(${goal.x},${goal.y})` : esc(t('quick.goal.noCell'))}</span>` : ''}
      </div>
      <div class="q-fields">
        <label class="q-field">${esc(t('quick.loss'))} <select class="q-loss-kind">
          ${['annihilate', 'unit', 'turns'].map(k => `<option value="${k}" ${loss.kind === k ? 'selected' : ''}>${esc(t(`quick.loss.${k}`))}</option>`).join('')}
        </select></label>
        ${loss.kind === 'unit' ? `<label class="q-field">${esc(t('quick.loss.target'))} <select class="q-loss-target">${pilotOptions(allyPilots, loss.target, '')}</select></label>` : ''}
        ${loss.kind === 'turns' ? `<label class="q-field">${esc(t('quick.loss.turnsLabel'))} <input class="q-loss-turns" type="number" min="1" max="99" value="${loss.turns}"></label>` : ''}
      </div>`;
    const sceneCard = sc => {
      const value = `scene:${sc.base}#${sc.id}`;
      return `<li><button type="button" class="q-scene-card" data-value="${esc(value)}" aria-pressed="${value === b.mapSource}">
        <canvas class="q-thumb" data-value="${esc(value)}" width="96" height="72" aria-hidden="true"></canvas>
        <span class="q-scene-text"><strong>${esc(sc.title ?? sc.id)}</strong>
        <span class="q-scene-desc">${esc(sc.description ?? '')}</span>
        ${sc.tags?.length ? `<span class="q-tags">${sc.tags.map(x => `<span>${esc(x)}</span>`).join('')}</span>` : ''}</span>
      </button></li>`;
    };

    const resetDisabled = this.hasContent() ? '' : 'disabled';
    this.root.innerHTML = `
      <p class="hint q-intro">${esc(t('quick.intro'))}</p>
      <div class="q-toolbar">
        <button type="button" class="q-reset danger" title="${esc(t('quick.resetHint'))}" ${resetDisabled}>${esc(t('quick.reset'))}</button>
      </div>
      <nav class="q-episodes" aria-label="${esc(t('quick.episodes'))}">
        ${this.episodes.map((e, i) => `<button type="button" class="q-episode" data-index="${i}" aria-current="${e === b ? 'page' : 'false'}">${esc(t('quick.episode', i + 1))}</button>`).join('')}
        <button type="button" class="q-add-episode">＋ ${esc(t('quick.addEpisode'))}</button>
        ${this.episodes.length > 1 ? `<button type="button" class="q-remove-episode danger">${esc(t('quick.removeEpisode'))}</button>` : ''}
      </nav>
      ${this.episodes.length > 1 ? `<p class="hint q-episode-hint">${esc(t('quick.episodesHint'))}${carried.length ? ' ' + esc(t('quick.carried', carried.join('、'))) : ''}</p>` : ''}
      <details class="q-gallery" ${galleryOpen ? 'open' : ''}>
        <summary>${esc(t('quick.gallery'))}${scenes.length ? ` <span class="hint">${esc(t('quick.galleryCount', scenes.length))}</span>` : ''}</summary>
        <p class="hint">${esc(t('quick.galleryHint'))}</p>
        ${this.scenes ? (scenes.length ? `<ul class="q-scenes">${scenes.map(sceneCard).join('')}</ul>` : `<p class="hint">${esc(t('quick.galleryEmpty'))}</p>`)
          : `<p class="hint">${esc(t('quick.galleryLoading'))}</p>`}
      </details>
      <section class="q-step">
        <h2><span class="q-num">1</span>${esc(t('quick.stepTitles'))}</h2>
        <p class="hint">${esc(t('quick.titlesHint'))}</p>
        <input type="search" class="q-search" placeholder="${esc(t('quick.searchTitles'))}" aria-label="${esc(t('quick.searchTitles'))}">
        <ul class="q-results"></ul>
        <ul class="q-chosen">${chosen.map(x => `
          <li data-key="${esc(x.key)}">${esc(x.title)} <span class="hint">${esc(t('quick.counts', x.units.length, x.pilots.length))}</span>
          <button type="button" class="q-remove-title icon" aria-label="${esc(t('quick.remove'))}" title="${esc(t('quick.remove'))}">×</button></li>`).join('')}</ul>
        <datalist id="q-units">${units.map(u => `<option value="${esc(u.name)}">${esc(u.title)}${u.class ? ' / ' + esc(u.class) : ''}</option>`).join('')}</datalist>
        <datalist id="q-pilots">${pilots.map(p => `<option value="${esc(p.name)}">${esc(p.nickname || '')} ${esc(p.title)}</option>`).join('')}</datalist>
      </section>
      <section class="q-step">
        <h2><span class="q-num">2</span>${esc(t('quick.stepUnits'))}</h2>
        ${chosen.length ? '' : `<p class="hint q-need">${esc(t('quick.needTitles'))}</p>`}
        <div class="q-sides">
          <div class="q-side allies">
            <h3>${esc(t('quick.allies'))} <span class="hint">${b.allies.length}</span></h3>
            ${sideTable('allies')}
            <button type="button" class="q-add" data-side="allies">＋ ${esc(t('quick.addAlly'))}</button>
          </div>
          <div class="q-side enemies">
            <h3>${esc(t('quick.enemies'))} <span class="hint">${b.enemies.length}</span></h3>
            ${sideTable('enemies')}
            <button type="button" class="q-add" data-side="enemies">＋ ${esc(t('quick.addEnemy'))}</button>
          </div>
        </div>
      </section>
      <section class="q-step">
        <h2><span class="q-num">3</span>${esc(t('quick.stepMap'))}</h2>
        <div class="q-fields">
          <label class="q-field">${esc(t('quick.map'))} <select class="q-map">
            <option value="">${esc(t('quick.plainMap'))}</option>
            ${scenes.length ? `<optgroup label="${esc(t('quick.scenes'))}">${scenes.map(sc => {
              const value = `scene:${sc.base}#${sc.id}`;
              return `<option value="${esc(value)}" ${value === b.mapSource ? 'selected' : ''}>${esc(sc.title ?? sc.id)}</option>`;
            }).join('')}</optgroup>` : ''}
            ${maps.length ? `<optgroup label="${esc(t('quick.folderMaps'))}">${maps.map(m => `<option value="${esc('folder:' + m)}" ${'folder:' + m === b.mapSource ? 'selected' : ''}>${esc(m)}</option>`).join('')}</optgroup>` : ''}
            ${b.mapSource?.startsWith('file:') ? `<option value="${esc(b.mapSource)}" selected>${esc(b.mapSource.slice(5))}</option>` : ''}
          </select></label>
          <button type="button" class="q-map-file">${esc(t('quick.mapFile'))}</button>
          <input type="file" class="q-map-input" accept=".map" hidden>
          <span class="q-size" ${b.mapSource ? 'hidden' : ''}>
            <label class="q-field">${esc(t('map.width'))} <input class="q-width" type="number" min="${MIN_DRAG_SIZE}" max="50" value="${b.width}"></label>
            <label class="q-field">${esc(t('map.height'))} <input class="q-height" type="number" min="${MIN_DRAG_SIZE}" max="50" value="${b.height}"></label>
          </span>
        </div>
        ${currentScene?.description ? `<p class="q-scene-about">${esc(currentScene.description)}</p>` : ''}
        ${b.width < MIN_DRAG_SIZE || b.height < MIN_DRAG_SIZE ? `<p class="q-small-map hint">${esc(t('quick.smallMap', MIN_DRAG_SIZE))}</p>` : ''}
        <p class="hint">${esc(t('quick.placeHint'))}</p>
        <p class="q-legend hint"><span class="q-key allies"></span>${esc(t('quick.allies'))}<span class="q-key enemies"></span>${esc(t('quick.enemies'))}<span class="q-key spot"></span>${esc(t('quick.spot'))}<span class="q-key boss"></span>${esc(t('quick.boss'))}<span class="q-key reinforce"></span>${esc(t('quick.reinforceKey'))}<span class="q-key goal"></span>${esc(t('quick.goalKey'))}</p>
        <div class="q-board"><canvas class="q-canvas"></canvas></div>
      </section>
      <section class="q-step">
        <h2><span class="q-num">4</span>${esc(t('quick.stepTalk'))}</h2>
        <p class="hint">${esc(t('quick.talkHint'))}</p>
        ${['opening', 'victory', 'defeat'].map(scene => `
          <h3>${esc(t(`quick.scene.${scene}`))}</h3>
          ${talkRows(scene)}
          <button type="button" class="q-add-line" data-scene="${scene}">＋ ${esc(t('quick.addLine'))}</button>`).join('')}
      </section>
      <section class="q-step q-events">
        <h2><span class="q-num">5</span>${esc(t('quick.stepEvents'))}</h2>
        <p class="hint">${esc(t('quick.eventsHint'))}</p>
        ${conditions}
        ${b.events.map(eventCard).join('')}
        <div class="q-actions">
          <button type="button" class="q-add-event" data-kind="reinforce">＋ ${esc(t('quick.ev.addReinforce'))}</button>
          <button type="button" class="q-add-event" data-kind="talk">＋ ${esc(t('quick.ev.addTalk'))}</button>
          <button type="button" class="q-add-event" data-kind="retreat">＋ ${esc(t('quick.ev.addRetreat'))}</button>
        </div>
      </section>
      <section class="q-step">
        <h2><span class="q-num">6</span>${esc(t('quick.stepPlay'))}</h2>
        <div class="q-fields">
          <label class="q-field">${esc(t('quick.scenarioTitle'))} <input class="q-title" value="${esc(this.title)}" placeholder="${esc(t('quick.defaultTitle'))}"></label>
        </div>
        <div class="q-actions">
          <button type="button" class="q-play primary">▶ ${esc(t('quick.play'))}</button>
          <button type="button" class="q-save">${esc(t('quick.save'))}</button>
          <button type="button" class="q-reset danger" title="${esc(t('quick.resetHint'))}" ${resetDisabled}>${esc(t('quick.reset'))}</button>
        </div>
        <p class="hint q-play-hint">${esc(t('quick.playHint'))}</p>
      </section>`;
    this.root.append(this.statusBox);
    this.bind();
    this.drawBoard();
    this.drawThumbs();
    webIsSameOrigin().then(same => {
      const button = this.root.querySelector('.q-play');
      if (button && !same) {
        button.disabled = true;
        this.root.querySelector('.q-play-hint').textContent = t('quick.playOtherSite');
      }
    });
  }

  /** 場面の一覧の縮図（地形タイプの色）。マップは一度だけ読む */
  async drawThumbs() {
    if (!this.root.querySelector('.q-gallery')?.open) {
      return;
    }
    // 縮図のマップはまとめて読み、読めたものから描く
    await Promise.all([...this.root.querySelectorAll('.q-thumb')].map(async canvas => {
      const value = canvas.dataset.value;
      if (!this.thumbs.has(value)) {
        const scene = this.scenes.find(sc => `scene:${sc.base}#${sc.id}` === value);
        this.thumbs.set(value, (async () => {
          const response = await fetch(new URL(scene.map, scene.base));
          return response.ok ? this.host.parseMap(new Uint8Array(await response.arrayBuffer())) : null;
        })().catch(() => null));
      }
      const map = await this.thumbs.get(value);
      if (!map || map.error || !canvas.isConnected) {
        return;
      }
      const cell = Math.max(1, Math.floor(Math.min(96 / map.width, 72 / map.height)));
      canvas.width = map.width * cell;
      canvas.height = map.height * cell;
      const ctx = canvas.getContext('2d');
      this.drawTiles(ctx, map.tiles, map.width, map.height, cell);
      if (await this.loadTileImages(map.tiles) && canvas.isConnected) {
        this.drawTiles(ctx, map.tiles, map.width, map.height, cell);
      }
    }));
  }

  /** マスの画像を読んでおく（SRC と同じ順で探す。無ければ null を覚えて色で描く） */
  async loadTileImages(tiles) {
    if (!this.host.loadImage) {
      return false;
    }
    // 画像はまとめて（並行に）読む。ほかの縮図が読んでいる途中の画像も待ち、読めたら描き直す
    const terrains = new Map(this.host.terrains().map(x => [x.id, x]));
    this.imageLoads ??= new Map();
    const waits = new Map();
    for (let i = 0; i < tiles.length; i += 2) {
      const info = terrains.get(tiles[i]);
      const key = info && `${info.bitmap}:${tiles[i + 1]}`;
      if (!key || waits.has(key)) {
        continue;
      }
      if (!this.images.has(key) && !this.imageLoads.has(key)) {
        this.imageLoads.set(key, this.host.loadImage(info.bitmap, tiles[i + 1])
          .catch(() => null)
          .then(image => {
            this.images.set(key, image);
            this.imageLoads.delete(key);
            return image;
          }));
      }
      if (this.imageLoads.has(key)) {
        waits.set(key, this.imageLoads.get(key));
      }
    }
    const loaded = await Promise.all(waits.values());
    return loaded.some(Boolean);
  }

  drawTiles(ctx, tiles, width, height, cell) {
    const terrains = new Map(this.host.terrains().map(x => [x.id, x]));
    ctx.imageSmoothingEnabled = cell < 16;
    for (let y = 0; y < height; y++) {
      for (let x = 0; x < width; x++) {
        const id = tiles[(y * width + x) * 2];
        const info = terrains.get(id);
        const image = info && this.images.get(`${info.bitmap}:${tiles[(y * width + x) * 2 + 1]}`);
        if (image) {
          ctx.drawImage(image, x * cell, y * cell, cell, cell);
        } else {
          ctx.fillStyle = terrainColor(info);
          ctx.fillRect(x * cell, y * cell, cell, cell);
        }
      }
    }
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
    this.root.querySelectorAll('.q-table tbody tr').forEach(row => {
      const unit = this.unitList(row.dataset.side)[Number(row.dataset.index)];
      const select = () => {
        if (this.selected?.side !== row.dataset.side || this.selected?.index !== Number(row.dataset.index)) {
          this.selected = { side: row.dataset.side, index: Number(row.dataset.index) };
          this.root.querySelectorAll('.q-table tbody tr').forEach(r => r.classList.toggle('selected', r === row));
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
      row.querySelector('.q-rank').addEventListener('change', ev => {
        unit.rank = Math.max(0, Math.min(20, Math.floor(Number(ev.target.value)) || 0));
        ev.target.value = unit.rank;
        this.saveDraft();
      });
      row.querySelector('.q-boss').addEventListener('change', ev => {
        unit.bossRank = Number(ev.target.value);
        this.saveDraft();
        this.drawBoard();
      });
      row.querySelector('.q-remove').addEventListener('click', ev => {
        ev.stopPropagation();
        this.unitList(row.dataset.side).splice(Number(row.dataset.index), 1);
        this.selected = null;
        this.saveDraft();
        this.render();
      });
    });

    $('.q-title').addEventListener('input', ev => {
      this.title = ev.target.value;
      this.saveDraft();
    });
    const resize = () => {
      this.battle.width = Math.max(MIN_DRAG_SIZE, Math.min(50, Number($('.q-width').value) || 20));
      this.battle.height = Math.max(MIN_DRAG_SIZE, Math.min(50, Number($('.q-height').value) || 15));
      $('.q-width').value = this.battle.width;
      $('.q-height').value = this.battle.height;
      $('.q-small-map')?.remove();
      this.clampUnits();
      this.saveDraft();
      this.drawBoard();
    };
    $('.q-width').addEventListener('change', resize);
    $('.q-height').addEventListener('change', resize);
    $('.q-map').addEventListener('change', ev => this.chooseMap(ev.target.value));
    const gallery = $('.q-gallery');
    // 開いた・閉じたのは人が押したときだけ覚える（作り直したときの toggle は数えない）
    gallery.querySelector('summary').addEventListener('click', () => {
      this.galleryOpen = !gallery.open;
    });
    gallery.addEventListener('toggle', () => {
      if (gallery.open) {
        this.drawThumbs();
      }
    });
    this.root.querySelectorAll('.q-scene-card').forEach(card => card.addEventListener('click', async () => {
      this.galleryOpen = false;
      await this.chooseMap(card.dataset.value);
      if (this.battle.mapSource === card.dataset.value) {
        const scene = this.scenes.find(sc => `scene:${sc.base}#${sc.id}` === card.dataset.value);
        this.status(t('quick.applied', scene?.title ?? ''), 'info');
      }
    }));
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
      const list = this.lineList(scene);
      const last = list.at(-1);
      list.push({ speaker: last?.speaker ?? '', text: '' });
      this.saveDraft();
      this.render();
      this.root.querySelector(`.q-line[data-scene="${scene}"]:last-of-type .q-text`)?.focus();
    }));
    this.root.querySelectorAll('.q-line').forEach(div => {
      const line = this.lineList(div.dataset.scene)[Number(div.dataset.index)];
      div.querySelector('.q-speaker').addEventListener('change', ev => {
        line.speaker = ev.target.value;
        this.saveDraft();
      });
      div.querySelector('.q-text').addEventListener('input', ev => {
        line.text = ev.target.value;
        this.saveDraft();
      });
      div.querySelector('.q-remove-line').addEventListener('click', () => {
        this.lineList(div.dataset.scene).splice(Number(div.dataset.index), 1);
        this.saveDraft();
        this.render();
      });
    });

    this.bindEvents();
    this.root.querySelectorAll('.q-episode').forEach(button => button.addEventListener('click', () => this.showEpisode(Number(button.dataset.index))));
    $('.q-add-episode').addEventListener('click', () => this.addEpisode());
    $('.q-remove-episode')?.addEventListener('click', () => this.removeEpisode());
    $('.q-play').addEventListener('click', () => this.play().catch(e => this.status(String(e.message ?? e), 'error')));
    $('.q-save').addEventListener('click', () => this.save().catch(e => this.status(String(e.message ?? e), 'error')));
    this.root.querySelectorAll('.q-reset').forEach(button => button.addEventListener('click', () => this.reset()));
  }

  /** 出来事の段（7474/SRC#1689）: 勝利・敗北の条件と、出来事の一覧 */
  bindEvents() {
    const $ = s => this.root.querySelector(s);
    const b = this.battle;
    const changed = (rerender = false) => {
      this.saveDraft();
      if (rerender) {
        this.render();
      } else {
        this.drawBoard();
      }
    };
    const number = (value, fallback) => Math.max(1, Math.min(99, Math.floor(Number(value)) || fallback));
    $('.q-goal-kind').addEventListener('change', ev => {
      b.goal.kind = ev.target.value;
      this.picking = null;
      changed(true);
    });
    $('.q-goal-target')?.addEventListener('change', ev => {
      b.goal.target = ev.target.value;
      changed();
    });
    $('.q-goal-turns')?.addEventListener('change', ev => {
      b.goal.turns = number(ev.target.value, 5);
      changed();
    });
    $('.q-goal-pick')?.addEventListener('click', ev => {
      this.picking = this.picking === 'goal' ? null : 'goal';
      ev.target.setAttribute('aria-pressed', String(this.picking === 'goal'));
      if (this.picking) {
        this.status(t('quick.goal.pickHint'), 'info');
        this.root.querySelector('.q-board')?.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
      }
    });
    $('.q-loss-kind').addEventListener('change', ev => {
      b.loss.kind = ev.target.value;
      changed(true);
    });
    $('.q-loss-target')?.addEventListener('change', ev => {
      b.loss.target = ev.target.value;
      changed();
    });
    $('.q-loss-turns')?.addEventListener('change', ev => {
      b.loss.turns = number(ev.target.value, 10);
      changed();
    });
    this.root.querySelectorAll('.q-add-event').forEach(button => button.addEventListener('click', () => {
      const e = newEvent(button.dataset.kind, b);
      b.events.push(e);
      if (button.dataset.kind === 'reinforce') {
        this.addUnit(`ev${b.events.length - 1}.enemies`);
        return;
      }
      changed(true);
    }));
    this.root.querySelectorAll('.q-event').forEach(box => {
      const e = b.events[Number(box.dataset.event)];
      const on = (selector, handler) => box.querySelector(selector)?.addEventListener('change', handler);
      on('.q-ev-trigger', ev => {
        e.trigger = ev.target.value;
        if (e.trigger === 'destroyed' || e.trigger === 'turn') {
          e.retreat = false;
        }
        changed(true);
      });
      on('.q-ev-turn', ev => {
        e.turn = number(ev.target.value, 2);
        changed();
      });
      on('.q-ev-phase', ev => {
        e.phase = ev.target.value;
        changed();
      });
      on('.q-ev-unit', ev => {
        e.unit = ev.target.value;
        changed();
      });
      on('.q-ev-target', ev => {
        e.target = ev.target.value;
        changed();
      });
      on('.q-ev-ratio', ev => {
        e.ratio = number(ev.target.value, 50);
        changed();
      });
      on('.q-ev-retreat', ev => {
        e.retreat = ev.target.checked;
        changed();
      });
      box.querySelector('.q-remove-event').addEventListener('click', () => {
        b.events.splice(Number(box.dataset.event), 1);
        this.selected = null;
        changed(true);
      });
    });
  }

  /** マップを選ぶ（'' は平地、scene:・folder: は場面のサンプル・シナリオのフォルダのマップ） */
  async chooseMap(value) {
    try {
      if (!value) {
        this.usePlain();
      } else if (value.startsWith('scene:')) {
        this.status(t('loading'));
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
  }

  clampUnits() {
    for (const { unit: u } of this.allUnits()) {
      u.x = Math.min(u.x, this.battle.width);
      u.y = Math.min(u.y, this.battle.height);
    }
  }

  /** 置いたユニットのマスなら選び、空いたマスなら選んでいるユニットを動かす */
  clickCell(x, y) {
    if (this.picking === 'goal') {
      // 勝利の条件の着くマス
      this.battle.goal.x = x;
      this.battle.goal.y = y;
      this.picking = null;
      this.saveDraft();
      this.render();
      return;
    }
    // そのマスのユニットを選ぶ。ただし援軍を選んでいるときは、はじめのユニットのマスにも動かせる（SRC が近くの空いたマスに置く）
    const reinforcing = Boolean(this.selected?.side.startsWith('ev'));
    const hit = this.allUnits().find(h => h.unit.x === x && h.unit.y === y && (!reinforcing || h.event >= 0));
    if (hit) {
      this.selected = { side: hit.key, index: hit.index };
      this.root.querySelectorAll('.q-table tbody tr').forEach(r =>
        r.classList.toggle('selected', r.dataset.side === hit.key && Number(r.dataset.index) === hit.index));
      this.drawBoard();
      return;
    }
    const unit = this.selected && this.unitList(this.selected.side)?.[this.selected.index];
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
    // 平地は地形 0（平地）のマス。マップの画像は手元の汎用データのもの（遊ぶ SRC#Web では、そのサイトの汎用グラフィック集で出る）
    const tiles = this.mapTiles ?? new Array(width * height * 2).fill(0);
    this.drawTiles(ctx, tiles, width, height, CELL);
    this.loadTileImages(tiles).then(loaded => {
      if (loaded) {
        this.drawBoard();
      }
    });
    // 場面の置く位置
    for (const [side, color] of [['allies', 'rgba(47,111,214,0.9)'], ['enemies', 'rgba(208,57,62,0.9)']]) {
      ctx.strokeStyle = color;
      ctx.lineWidth = 2;
      ctx.setLineDash([3, 2]);
      for (const p of this.battle.spots?.[side] ?? []) {
        ctx.strokeRect((p.x - 1) * CELL + 4, (p.y - 1) * CELL + 4, CELL - 8, CELL - 8);
      }
      ctx.setLineDash([]);
    }
    ctx.strokeStyle = 'rgba(0,0,0,0.25)';
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
    // 勝利の条件の着くマス
    const goal = this.battle.goal;
    if (goal?.kind === 'reach' && goal.x && goal.y) {
      ctx.strokeStyle = '#ffd400';
      ctx.lineWidth = 3;
      ctx.setLineDash([4, 3]);
      ctx.strokeRect((goal.x - 1) * CELL + 2, (goal.y - 1) * CELL + 2, CELL - 4, CELL - 4);
      ctx.setLineDash([]);
      ctx.fillStyle = '#ffd400';
      ctx.fillText('★', (goal.x - 0.5) * CELL, (goal.y - 0.5) * CELL + 1);
    }
    // 援軍は、はじめのユニットの下に薄く出す（出来事の番号を付ける）
    const order = this.allUnits().sort((a, c) => (c.event >= 0) - (a.event >= 0));
    for (const { key: side, side: party, index: i, unit: u, event } of order) {
      {
        const left = (u.x - 1) * CELL;
        const top = (u.y - 1) * CELL;
        ctx.globalAlpha = event >= 0 ? 0.55 : 1;
        ctx.fillStyle = party === 'allies' ? '#2f6fd6' : '#d0393e';
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
        if ((u.bossRank ?? -1) >= 0) {
          // ボスは右上に金の印
          ctx.fillStyle = '#ffd400';
          ctx.beginPath();
          ctx.arc(left + CELL - 5, top + 5, 4, 0, Math.PI * 2);
          ctx.fill();
          ctx.strokeStyle = '#000000';
          ctx.lineWidth = 1;
          ctx.stroke();
        }
        if (event >= 0) {
          // 援軍は左上に出来事の番号
          ctx.globalAlpha = 1;
          ctx.fillStyle = '#000000';
          ctx.fillRect(left + 1, top + 1, 11, 11);
          ctx.fillStyle = '#ffffff';
          ctx.font = 'bold 9px system-ui, sans-serif';
          ctx.fillText(String(event + 1), left + 6.5, top + 7);
          ctx.font = `bold ${CELL * 0.42}px system-ui, sans-serif`;
        }
        ctx.globalAlpha = 1;
      }
    }
  }
}
