// SRC#Studio のマップ作成ツールの画面（7474/SRC#1641）。ファイルの読み書きは使う側（index.html の studio.js・VS Code の vscode.js）が行う。
//
// const editor = new MapEditor(element, {
//   loadImage: async (bitmap, no) => ImageBitmap | null,   // 地形の画像
//   imageNumbers: async bitmap => number[],                 // 手元にある画像の番号（パレット）
//   serialize: async map => string,                          // マップをテキストにする（studio-core.js の serializeMap）
//   autoConnect: async map => ({ tiles, layer }),            // 地形の境目・変種を自動でつなぐ（任意）
//   onEdit: text => {},                                      // 編集のたび（一筆ごと）に、書き出したテキスト
//   undo: 'own' | { undo: () => {}, redo: () => {} },        // 元に戻す（既定は自前。VS Code は文書の元に戻すを使う）
// });
// editor.setTerrains(terrains); editor.setMap(map);   // map は studio-core.js の parseMap の結果
import { t } from './i18n.js';

const TILE = 32;
const NO_LAYER = 10000;
export const CLASS_COLORS = { '陸': '#8db360', '屋内': '#b49a74', '月面': '#b9b9b9', '水': '#4f86c6', '深水': '#2c5a92', '空': '#bcd8f0', '宇宙': '#1b1b3a' };
export const PARTY_COLORS = { '味方': '#2f6fd6', '敵': '#d0393e', '中立': '#d99b00', 'ＮＰＣ': '#2d9d5c' };
const TOOLS = ['pen', 'rect', 'fill', 'pick', 'copy', 'paste'];

const TOOL_KEYS = { pen: 'B', rect: 'R', fill: 'G', pick: 'I', copy: 'C', paste: 'V' };

export class MapEditor {
  constructor(element, options) {
    this.options = options;
    this.terrains = [];
    this.terrainById = new Map();
    this.map = null;
    this.tool = 'pen';
    this.target = 'tiles';
    this.autoConnect = false;
    this.showPlacements = true;
    this.placements = [];
    this.selectedPlacement = null;
    this.moveRangeCells = new Set();
    this.highlightCell = null;
    this.dragPlacement = null;
    /** 人形劇（7474/SRC#1694）: { file, line, steps, index } か null */
    this.puppet = null;
    this.puppetBlocks = [];
    this.dragPuppet = null;
    this.current = { terrain: 0, bitmap: 0 };
    this.zoom = 1;
    this.grid = true;
    this.history = [];
    this.future = [];
    this.images = new Map();
    this.unitImages = new Map();
    this.clipboard = null;
    this.element = element;
    this.build();
  }

  // ---- 画面を作る ----

  build() {
    const e = this.element;
    e.classList.add('map-editor');
    e.innerHTML = `
      <div class="me-toolbar" role="toolbar">
        <span class="me-group me-tools"></span>
        <span class="me-group me-target-group">
          <label class="me-seg"><input type="radio" name="me-target" value="tiles" checked><span>${t('map.targetTiles')}</span></label>
          <label class="me-seg"><input type="radio" name="me-target" value="layer"><span>${t('map.targetLayer')}</span></label>
          <label class="me-seg"><input type="radio" name="me-target" value="placement"><span>${t('map.targetPlacement')}</span></label>
          <button type="button" class="me-erase" title="${t('map.eraseLayerHint')}">${t('map.eraseLayer')}</button>
        </span>
        <span class="me-group">
          <label class="me-check" title="${t('map.autoConnectHint')}"><input type="checkbox" class="me-auto-connect"> ${t('map.autoConnect')}</label>
          <button type="button" class="me-reconnect" title="${t('map.reconnectHint')}">${t('map.reconnect')}</button>
        </span>
        <span class="me-group">
          <button type="button" class="me-undo" title="Ctrl+Z">${t('map.undo')}</button>
          <button type="button" class="me-redo" title="Ctrl+Y">${t('map.redo')}</button>
        </span>
        <span class="me-group">
          <button type="button" class="me-zoom-out" aria-label="${t('map.zoomOut')}" title="${t('map.zoomOut')} (-)">−</button>
          <button type="button" class="me-zoom-label" title="${t('map.zoomReset')} (0)">100%</button>
          <button type="button" class="me-zoom-in" aria-label="${t('map.zoomIn')}" title="${t('map.zoomIn')} (+)">＋</button>
          <label class="me-check"><input type="checkbox" class="me-grid" checked> ${t('map.grid')}</label>
          <label class="me-check"><input type="checkbox" class="me-show-placements" checked> ${t('map.showPlacements')}</label>
        </span>
        <span class="me-group">
          <button type="button" class="me-resize">${t('map.resize')}</button>
        </span>
      </div>
      <div class="me-body">
        <div class="me-canvas-wrap"><canvas class="me-canvas"></canvas></div>
        <div class="me-palette">
          <div class="me-terrain-pane">
            <div class="me-current"><canvas width="${TILE}" height="${TILE}"></canvas><div class="me-current-info"><span></span><small class="me-current-meta"></small></div></div>
            <select class="me-class"></select>
            <input type="search" class="me-search" placeholder="${t('map.search')}">
            <ul class="me-terrains" role="listbox"></ul>
            <div class="me-bitmaps"></div>
          </div>
          <div class="me-placement-pane" hidden>
            <div class="me-puppet" hidden>
              <label class="me-puppet-head">${t('map.puppet.title')}
                <select class="me-puppet-block"><option value="">${t('map.puppet.none')}</option></select></label>
              <div class="me-puppet-body" hidden>
                <div class="me-puppet-nav">
                  <button type="button" class="me-puppet-prev" aria-label="${t('map.puppet.prev')}" title="${t('map.puppet.prev')} (←)">◀</button>
                  <input type="range" class="me-puppet-range" min="0" max="0" value="0" aria-label="${t('map.puppet.step')}">
                  <button type="button" class="me-puppet-next" aria-label="${t('map.puppet.next')}" title="${t('map.puppet.next')} (→)">▶</button>
                  <span class="me-puppet-pos"></span>
                  <button type="button" class="me-puppet-play" hidden>${t('map.puppet.play')}</button>
                </div>
                <div class="me-puppet-step" aria-live="polite"></div>
                <ul class="me-puppet-warnings"></ul>
                <p class="hint">${t('map.puppet.hint')}</p>
              </div>
            </div>
            <p class="hint me-static">${t('map.placements.hint')}</p>
            <div class="me-legend">
              <span><i class="me-party-dot" style="background:${PARTY_COLORS['味方']}"></i>${t('map.party.ally')}</span>
              <span><i class="me-party-dot" style="background:${PARTY_COLORS['敵']}"></i>${t('map.party.enemy')}</span>
              <span><i class="me-party-dot" style="background:${PARTY_COLORS['中立']}"></i>${t('map.party.neutral')}</span>
              <span><i class="me-party-dot" style="background:${PARTY_COLORS['ＮＰＣ']}"></i>${t('map.party.npc')}</span>
              <span><i class="me-party-dot me-dot-event"></i>${t('map.legend.byEvent')}</span>
              <span><i class="me-party-dot me-dot-goal"></i>${t('map.legend.goal')}</span>
            </div>
            <input type="search" class="me-pl-search me-static" placeholder="${t('map.placements.search')}" aria-label="${t('map.placements.search')}">
            <ul class="me-placements me-static" role="listbox"></ul>
            <p class="hint me-placements-empty" hidden>${t('map.placements.empty')}</p>
            <div class="me-placement-detail" hidden></div>
          </div>
        </div>
      </div>
      <div class="me-status-bar">
        <div class="me-status" aria-live="polite"></div>
        <div class="me-map-size"></div>
      </div>
      <dialog class="me-resize-dialog">
        <form method="dialog">
          <p>${t('map.resizeHint')}</p>
          <label>${t('map.width')} <input type="number" name="width" min="1" max="50" required></label>
          <label>${t('map.height')} <input type="number" name="height" min="1" max="50" required></label>
          <menu><button value="cancel" formnovalidate>${t('cancel')}</button><button value="ok">${t('ok')}</button></menu>
        </form>
      </dialog>`;
    const tools = e.querySelector('.me-tools');
    for (const tool of TOOLS) {
      const b = document.createElement('button');
      b.type = 'button';
      b.dataset.tool = tool;
      b.textContent = t(`map.tool.${tool}`);
      b.title = `${t(`map.toolHint.${tool}`)} (${TOOL_KEYS[tool]})`;
      b.addEventListener('click', () => this.setTool(tool));
      tools.append(b);
    }
    this.canvas = e.querySelector('.me-canvas');
    this.ctx = this.canvas.getContext('2d');
    this.status = e.querySelector('.me-status');
    e.querySelectorAll('input[name="me-target"]').forEach(r => r.addEventListener('change', () => this.setTarget(r.value)));
    e.querySelector('.me-erase').addEventListener('click', () => this.setCurrent(NO_LAYER, NO_LAYER, 'layer'));
    const ac = e.querySelector('.me-auto-connect');
    ac.disabled = !this.options.autoConnect;
    ac.addEventListener('change', ev => { this.autoConnect = ev.target.checked; });
    e.querySelector('.me-reconnect').addEventListener('click', () => this.reconnect());
    e.querySelector('.me-undo').addEventListener('click', () => this.undo());
    e.querySelector('.me-redo').addEventListener('click', () => this.redo());
    e.querySelector('.me-zoom-in').addEventListener('click', () => this.setZoom(this.zoom * 2));
    e.querySelector('.me-zoom-out').addEventListener('click', () => this.setZoom(this.zoom / 2));
    e.querySelector('.me-zoom-label').addEventListener('click', () => this.setZoom(1));
    e.querySelector('.me-grid').addEventListener('change', ev => { this.grid = ev.target.checked; this.render(); });
    e.querySelector('.me-show-placements').addEventListener('change', ev => { this.showPlacements = ev.target.checked; this.render(); });
    e.querySelector('.me-resize').addEventListener('click', () => this.openResize());
    e.querySelector('.me-puppet-block').addEventListener('change', ev => this.selectPuppet(ev.target.value));
    e.querySelector('.me-puppet-range').addEventListener('input', ev => this.setPuppetIndex(Number(ev.target.value)));
    e.querySelector('.me-puppet-prev').addEventListener('click', () => this.setPuppetIndex((this.puppet?.index ?? 0) - 1));
    e.querySelector('.me-puppet-next').addEventListener('click', () => this.setPuppetIndex((this.puppet?.index ?? 0) + 1));
    e.querySelector('.me-puppet-play').addEventListener('click', () => this.puppet && this.options.timeline?.play?.(this.puppet.file, this.puppet.line));
    e.querySelector('.me-class').addEventListener('change', () => this.renderTerrainList());
    e.querySelector('.me-search').addEventListener('input', () => this.renderTerrainList());
    e.querySelector('.me-pl-search').addEventListener('input', () => this.renderPlacementList());
    e.querySelector('.me-canvas-wrap').addEventListener('wheel', ev => {
      if (ev.ctrlKey || ev.metaKey) {
        ev.preventDefault();
        this.setZoom(ev.deltaY < 0 ? this.zoom * 2 : this.zoom / 2);
      }
    }, { passive: false });
    this.canvas.addEventListener('contextmenu', ev => ev.preventDefault());
    this.canvas.addEventListener('pointerdown', ev => this.pointerDown(ev));
    this.canvas.addEventListener('pointermove', ev => this.pointerMove(ev));
    this.canvas.addEventListener('pointerup', ev => this.pointerUp(ev));
    this.canvas.addEventListener('pointerleave', () => { this.hover = null; this.render(); });
    e.tabIndex = 0;
    e.addEventListener('keydown', ev => this.keyDown(ev));
    this.setTool('pen');
    this.setTarget('tiles');
  }

  setTarget(target) {
    this.target = target;
    const radio = this.element.querySelector(`input[name="me-target"][value="${target}"]`);
    if (radio) {
      radio.checked = true;
    }
    const isPlace = target === 'placement';
    this.element.querySelector('.me-terrain-pane').hidden = isPlace;
    this.element.querySelector('.me-placement-pane').hidden = !isPlace;
    this.element.querySelectorAll('.me-tools button').forEach(b => { b.disabled = isPlace; });
    this.element.querySelector('.me-erase').disabled = isPlace;
    this.element.querySelector('.me-reconnect').disabled = isPlace || !this.options.autoConnect;
    this.canvas.style.cursor = isPlace ? 'grab' : 'crosshair';
    this.render();
  }

  setTool(tool) {
    this.tool = tool;
    this.selection = null;
    this.element.querySelectorAll('.me-tools button').forEach(b => b.setAttribute('aria-pressed', String(b.dataset.tool === tool)));
    this.render();
  }

  setZoom(zoom) {
    this.zoom = Math.min(2, Math.max(0.25, zoom));
    this.element.querySelector('.me-zoom-label').textContent = `${Math.round(this.zoom * 100)}%`;
    this.render();
  }

  // ---- 地形とパレット ----

  setTerrains(terrains) {
    this.terrains = terrains;
    this.terrainById = new Map(terrains.map(x => [x.id, x]));
    const classes = [...new Set(terrains.map(x => x.class))];
    const select = this.element.querySelector('.me-class');
    select.innerHTML = `<option value="">${t('map.allClasses')}</option>` + classes.map(c => `<option>${escapeHtml(c)}</option>`).join('');
    this.renderTerrainList();
    if (terrains.length > 0 && !this.terrainById.has(this.current.terrain)) {
      this.setCurrent(terrains[0].id, 0);
    } else {
      this.renderCurrent();
    }
    this.render();
  }

  renderTerrainList() {
    const cls = this.element.querySelector('.me-class').value;
    const q = this.element.querySelector('.me-search').value.trim();
    const list = this.element.querySelector('.me-terrains');
    list.innerHTML = '';
    for (const terrain of this.terrains) {
      if ((cls && terrain.class !== cls) || (q && !`${terrain.id} ${terrain.name} ${terrain.bitmap}`.includes(q))) {
        continue;
      }
      const li = document.createElement('li');
      li.role = 'option';
      li.dataset.id = terrain.id;
      const swatch = document.createElement('i');
      swatch.className = 'me-terrain-dot';
      swatch.setAttribute('aria-hidden', 'true');
      swatch.style.background = CLASS_COLORS[terrain.class] ?? '#999';
      li.append(swatch, `${terrain.id} ${terrain.name}`);
      li.title = `${terrain.class}・${t('map.moveCost')} ${terrain.moveCost}・${t('map.hit')} ${terrain.hitMod}・${t('map.damage')} ${terrain.damageMod}${terrain.features.length ? '・' + terrain.features.join(' ') : ''}`;
      li.setAttribute('aria-selected', String(terrain.id === this.current.terrain));
      li.addEventListener('click', () => this.setCurrent(terrain.id, null));
      list.append(li);
    }
  }

  /** 描く地形を選ぶ。bitmap が null なら、その地形で手元にある最初の画像の番号 */
  async setCurrent(terrain, bitmap, target) {
    if (target) {
      this.setTarget(target);
    }
    this.current = { terrain, bitmap: bitmap ?? 0 };
    this.element.querySelectorAll('.me-terrains li').forEach(li => li.setAttribute('aria-selected', String(Number(li.dataset.id) === terrain)));
    const info = this.terrainById.get(terrain);
    const numbers = info ? await this.options.imageNumbers(info.bitmap) : [];
    if (bitmap == null && numbers.length > 0) {
      this.current.bitmap = numbers[0];
    }
    const box = this.element.querySelector('.me-bitmaps');
    box.innerHTML = '';
    for (const no of numbers) {
      const c = document.createElement('canvas');
      c.width = c.height = TILE;
      c.title = `${info.bitmap} ${no}`;
      c.dataset.no = no;
      c.setAttribute('aria-selected', String(no === this.current.bitmap));
      c.addEventListener('click', () => {
        this.current.bitmap = no;
        box.querySelectorAll('canvas').forEach(x => x.setAttribute('aria-selected', String(Number(x.dataset.no) === no)));
        this.renderCurrent();
      });
      box.append(c);
      this.drawTile(c.getContext('2d'), 0, 0, TILE, { terrain, bitmap: no });
    }
    this.renderCurrent();
  }

  renderCurrent() {
    const c = this.element.querySelector('.me-current canvas');
    const ctx = c.getContext('2d');
    ctx.clearRect(0, 0, TILE, TILE);
    const { terrain, bitmap } = this.current;
    const meta = this.element.querySelector('.me-current-meta');
    if (terrain === NO_LAYER) {
      this.element.querySelector('.me-current span').textContent = t('map.eraseLayer');
      if (meta) {
        meta.textContent = '';
      }
      return;
    }
    this.drawTile(ctx, 0, 0, TILE, this.current);
    const info = this.terrainById.get(terrain);
    this.element.querySelector('.me-current span').textContent = `${terrain} ${info?.name ?? t('map.undefined')}（${bitmap}）`;
    if (meta) {
      meta.textContent = info ? `${info.class} · ${t('map.moveCost')}${info.moveCost} · ${t('map.hit')}${info.hitMod >= 0 ? '+' : ''}${info.hitMod} · ${t('map.damage')}${info.damageMod >= 0 ? '+' : ''}${info.damageMod}` : '';
    }
  }

  // ---- 配置（7474/SRC#1653） ----

  /** このマップに対する配置一覧を入れる */
  setPlacements(items) {
    this.placements = Array.isArray(items) ? items : [];
    this.refreshPuppetBlocks();
    const prevId = this.selectedPlacement?.id;
    const match = prevId ? this.placements.find(p => p.id === prevId) : null;
    this.selectedPlacement = match ?? null;
    this.renderPlacementList();
    this.updateMoveRange();
    this.render();
  }

  /** イベント行や座標を指定して配置タブを開き、そのマスを画面の中央付近に出す */
  async focusPlacement(focus) {
    this.setTarget('placement');
    let match = null;
    if (focus?.file && focus?.line) {
      const norm = p => String(p).replace(/\\/g, '/').replace(/^\.\//, '').toLowerCase();
      const targetFile = norm(focus.file);
      match = this.placements.find(p => norm(p.file) === targetFile && p.line === focus.line) ?? null;
    }
    if (match) {
      this.highlightCell = null;
      await this.selectPlacement(match);
      this.scrollToCell(match.x, match.y);
    } else if (focus?.x && focus?.y) {
      this.highlightCell = { x: focus.x, y: focus.y, label: focus.command ?? '' };
      await this.selectPlacement(null, false);
      this.scrollToCell(focus.x, focus.y);
      this.status.textContent = `(${focus.x},${focus.y}) ${focus.command ?? ''} ${focus.unit ?? ''}`.trim();
    }
  }

  scrollToCell(x, y) {
    if (!this.map) {
      return;
    }
    const wrap = this.element.querySelector('.me-canvas-wrap');
    const size = TILE * this.zoom;
    wrap.scrollLeft = Math.max(0, (x - 0.5) * size - wrap.clientWidth / 2);
    wrap.scrollTop = Math.max(0, (y - 0.5) * size - wrap.clientHeight / 2);
  }

  async selectPlacement(item, clearHighlight = true) {
    if (clearHighlight) {
      this.highlightCell = null;
    }
    this.selectedPlacement = item ?? null;
    this.element.querySelectorAll('.me-placements li').forEach(li => {
      li.setAttribute('aria-selected', String(li.dataset.id === (item?.id ?? '')));
    });
    this.renderPlacementDetail();
    await this.updateMoveRange();
    this.render();
  }

  async updateMoveRange() {
    this.moveRangeCells.clear();
    const item = this.selectedPlacement;
    if (!item || !(item.speed > 0) || !this.options.computeMoveRange || !this.map) {
      return;
    }
    const targetId = item.id;
    try {
      const cells = await this.options.computeMoveRange(this.map, item);
      if (this.selectedPlacement?.id !== targetId) {
        return;
      }
      for (const c of cells) {
        this.moveRangeCells.add(`${c.x},${c.y}`);
      }
      this.render();
    } catch {
      // 移動範囲の目安は計算できなくても操作できる
    }
  }

  placementAt(x, y) {
    const hits = this.placements.filter(p => p.x === x && p.y === y);
    if (hits.length === 0) {
      return null;
    }
    if (this.selectedPlacement) {
      const cur = hits.find(p => p.id === this.selectedPlacement.id);
      if (cur) {
        return cur;
      }
    }
    return hits[hits.length - 1];
  }

  async movePlacement(item, newX, newY) {
    if (!item?.editable || !this.map) {
      return;
    }
    const x = Math.max(1, Math.min(this.map.width, Math.round(newX)));
    const y = Math.max(1, Math.min(this.map.height, Math.round(newY)));
    if (x === item.x && y === item.y) {
      return;
    }
    item.x = x;
    item.y = y;
    this.renderPlacementList();
    this.renderPlacementDetail();
    await this.updateMoveRange();
    this.render();
    await this.options.onEditPlacement?.(item, x, y);
  }

  renderPlacementList() {
    const list = this.element.querySelector('.me-placements');
    const empty = this.element.querySelector('.me-placements-empty');
    const search = this.element.querySelector('.me-pl-search');
    const q = (search?.value ?? '').trim().toLowerCase();
    list.innerHTML = '';
    empty.hidden = this.placements.length > 0;
    if (search) {
      search.hidden = this.placements.length === 0;
    }
    for (const item of this.placements) {
      if (q) {
        const hay = `${item.unit ?? ''} ${item.pilot ?? ''} ${item.party ?? ''} ${item.label ?? ''} ${item.command ?? ''} ${item.file ?? ''}`.toLowerCase();
        if (!hay.includes(q)) {
          continue;
        }
      }
      const li = document.createElement('li');
      li.role = 'option';
      li.dataset.id = item.id;
      li.setAttribute('aria-selected', String(item.id === this.selectedPlacement?.id));
      const color = PARTY_COLORS[item.party] ?? '#6e7681';
      const goal = item.command === '進入';
      const title = goal ? `${item.command} ${item.unit}` : item.unit || item.pilot || item.command;
      // 出来事で出るユニットは、どの出来事か（ラベル）を添える
      const sub = (item.pilot && item.pilot !== item.unit ? `${item.pilot} · ` : '') + (item.byEvent && !goal && item.label ? `${item.label} · ` : '');
      const dot = goal ? '<i class="me-party-dot me-dot-goal"></i>'
        : `<i class="me-party-dot${item.byEvent ? ' me-dot-event' : ''}" style="${item.byEvent ? `color:${color}` : `background:${color}`}"></i>`;
      li.innerHTML = `${dot}<span class="me-pl-main" title="${escapeHtml(`${item.file}:${item.line} ${item.command}`)}">${escapeHtml(title)}</span><span class="me-pl-meta">${escapeHtml(sub)}(${item.x},${item.y})</span>`;
      li.addEventListener('click', () => {
        this.selectPlacement(item);
        this.scrollToCell(item.x, item.y);
      });
      list.append(li);
    }
    this.renderPlacementDetail();
  }

  renderPlacementDetail() {
    const box = this.element.querySelector('.me-placement-detail');
    const item = this.selectedPlacement;
    if (!item) {
      box.hidden = true;
      box.innerHTML = '';
      return;
    }
    box.hidden = false;
    const partyLabel = item.party || t('map.party.other');
    const who = [item.unit, item.pilot && item.pilot !== item.unit ? `(${item.pilot})` : ''].filter(Boolean).join(' ');
    const moveInfo = item.speed > 0 ? `<div class="hint">${escapeHtml(t('map.placements.moveRange', item.speed))} ${escapeHtml(item.transportation || '')}</div>` : '';
    const ro = item.editable ? '' : `<div class="hint">${escapeHtml(t('map.placements.readonly'))}</div>`;
    box.innerHTML = `
      <div><strong>${escapeHtml(who || item.command)}</strong> · ${escapeHtml(partyLabel)}</div>
      <div class="hint">${escapeHtml(`${item.file}:${item.line}`)}${item.label ? ` · ${escapeHtml(item.label)}` : ''} · ${escapeHtml(item.command)}</div>
      <div class="me-pl-coords">
        <label>${t('map.placements.x')} <input type="number" class="me-pl-x" min="1" max="${this.map?.width ?? 50}" value="${item.x}" ${item.editable ? '' : 'disabled'}></label>
        <label>${t('map.placements.y')} <input type="number" class="me-pl-y" min="1" max="${this.map?.height ?? 50}" value="${item.y}" ${item.editable ? '' : 'disabled'}></label>
      </div>
      ${moveInfo}${ro}`;
    if (item.editable) {
      const xIn = box.querySelector('.me-pl-x');
      const yIn = box.querySelector('.me-pl-y');
      const apply = () => this.movePlacement(item, Number(xIn.value), Number(yIn.value));
      xIn.addEventListener('change', apply);
      yIn.addEventListener('change', apply);
    }
  }

  // ---- マップ ----

  /** マップを入れる（読み直したとき・VS Code で文書が変わったとき）。keepHistory なら元に戻すの履歴を残す */
  setMap(map, keepHistory = false) {
    this.map = { ...map, tiles: Array.from(map.tiles), layer: map.layer ? Array.from(map.layer) : null };
    if (!keepHistory) {
      this.history = [];
      this.future = [];
    }
    this.render();
    this.updateButtons();
  }

  cell(source, x, y) {
    const i = (y * this.map.width + x) * 2;
    return { terrain: source[i], bitmap: source[i + 1] };
  }

  async render() {
    if (!this.map) {
      return;
    }
    const size = TILE * this.zoom;
    const { width, height } = this.map;
    const sizeLabel = this.element.querySelector('.me-map-size');
    if (sizeLabel) {
      sizeLabel.textContent = t('map.sizeInfo', width, height);
    }
    this.canvas.width = width * size;
    this.canvas.height = height * size;
    const ctx = this.ctx;
    ctx.imageSmoothingEnabled = false;
    for (let y = 0; y < height; y++) {
      for (let x = 0; x < width; x++) {
        this.drawTile(ctx, x * size, y * size, size, this.cell(this.map.tiles, x, y));
        if (this.map.layer) {
          const l = this.cell(this.map.layer, x, y);
          if (l.terrain !== NO_LAYER) {
            this.drawTile(ctx, x * size, y * size, size, l, this.target === 'tiles' ? 0.6 : 1);
          }
        }
      }
    }
    if (this.grid) {
      ctx.strokeStyle = 'rgba(0,0,0,0.25)';
      ctx.lineWidth = 1;
      ctx.beginPath();
      for (let x = 0; x <= width; x++) {
        ctx.moveTo(x * size + 0.5, 0);
        ctx.lineTo(x * size + 0.5, height * size);
      }
      for (let y = 0; y <= height; y++) {
        ctx.moveTo(0, y * size + 0.5);
        ctx.lineTo(width * size, y * size + 0.5);
      }
      ctx.stroke();
    }
    if (this.puppet && this.target === 'placement') {
      this.drawPuppet(ctx, size);
    } else if (this.target === 'placement' || this.showPlacements) {
      this.drawPlacements(ctx, size);
    }
    const box = this.selectionBox();
    if (box) {
      ctx.strokeStyle = '#e8b400';
      ctx.lineWidth = 2;
      ctx.strokeRect(box.x0 * size + 1, box.y0 * size + 1, (box.x1 - box.x0 + 1) * size - 2, (box.y1 - box.y0 + 1) * size - 2);
    }
    if (this.hover) {
      ctx.strokeStyle = '#fff';
      ctx.lineWidth = 2;
      const w = this.target !== 'placement' && this.tool === 'paste' && this.clipboard ? this.clipboard.width : 1;
      const h = this.target !== 'placement' && this.tool === 'paste' && this.clipboard ? this.clipboard.height : 1;
      ctx.strokeRect(this.hover.x * size + 1, this.hover.y * size + 1, w * size - 2, h * size - 2);
    }
  }

  // ---- 人形劇（7474/SRC#1694）: 場面を一手ずつ見て、ユニットを動かして Move を書く ----

  /** このマップのイベントの塊を選べるようにする（動き・会話のある塊だけ） */
  async refreshPuppetBlocks() {
    const box = this.element.querySelector('.me-puppet');
    if (!this.options.timeline) {
      box.hidden = true;
      return;
    }
    const blocks = (await this.options.timeline.blocks()).filter(b => b.moves > 0 || b.talks > 0);
    this.puppetBlocks = blocks;
    box.hidden = false;
    this.element.querySelector('.me-puppet-play').hidden = !this.options.timeline.play;
    const select = this.element.querySelector('.me-puppet-block');
    const current = this.puppet ? `${this.puppet.file}#${this.puppet.line}` : '';
    select.innerHTML = `<option value="">${escapeHtml(t('map.puppet.none'))}</option>` + blocks.map(b =>
      `<option value="${escapeHtml(`${b.file}#${b.line}`)}">${escapeHtml(`${b.file}: ${b.label}`)} — ${escapeHtml(t('map.puppet.blockInfo', b.moves, b.talks))}</option>`).join('');
    if (current && blocks.some(b => `${b.file}#${b.line}` === current)) {
      select.value = current;
      await this.reloadPuppet();
    } else if (current) {
      await this.selectPuppet('');
    }
  }

  async selectPuppet(value) {
    if (!value) {
      this.puppet = null;
    } else {
      const i = value.lastIndexOf('#');
      this.puppet = { file: value.slice(0, i), line: Number(value.slice(i + 1)), steps: [], index: 0 };
      await this.reloadPuppet();
    }
    // 人形劇の間は、静的な配置の一覧を隠す（マップには一手ごとの状態を出す）
    this.element.querySelector('.me-placement-pane').classList.toggle('me-puppet-active', Boolean(this.puppet));
    this.renderPuppetPanel();
    this.render();
  }

  /** 一手ずつの状態を作り直す。focusLine があれば、その行の手を選ぶ */
  async reloadPuppet(focusLine) {
    if (!this.puppet) {
      return;
    }
    const steps = await this.options.timeline.build(this.puppet.file, this.puppet.line);
    this.puppet.steps = steps;
    if (focusLine) {
      const i = steps.findIndex(s => s.line === focusLine);
      if (i >= 0) {
        this.puppet.index = i;
      }
    }
    this.puppet.index = Math.max(0, Math.min(this.puppet.index, steps.length - 1));
    this.renderPuppetPanel();
    this.render();
  }

  setPuppetIndex(i) {
    if (!this.puppet || this.puppet.steps.length === 0) {
      return;
    }
    this.puppet.index = Math.max(0, Math.min(i, this.puppet.steps.length - 1));
    this.renderPuppetPanel();
    this.revealPuppetStep();
    this.render();
  }

  /** 今の手で見るべきマス（動いた先・Center・話す人）が画面の外なら、そこまでスクロールする */
  revealPuppetStep() {
    const step = this.puppet?.steps[this.puppet.index];
    if (!step) {
      return;
    }
    const speaker = step.kind === 'talk' ? step.units.find(u => u.key === step.speaker || u.pilot === step.speaker || u.unit === step.speaker) : null;
    const cell = step.toX ? [step.toX, step.toY] : step.centerX ? [step.centerX, step.centerY] : speaker ? [speaker.x, speaker.y] : null;
    const wrap = this.element.querySelector('.me-canvas-wrap');
    if (!cell || !wrap) {
      return;
    }
    const size = TILE * this.zoom;
    const [cx, cy] = [(cell[0] - 0.5) * size, (cell[1] - 0.5) * size];
    if (cx < wrap.scrollLeft || cx > wrap.scrollLeft + wrap.clientWidth || cy < wrap.scrollTop || cy > wrap.scrollTop + wrap.clientHeight) {
      this.scrollToCell(cell[0], cell[1]);
    }
  }

  renderPuppetPanel() {
    const body = this.element.querySelector('.me-puppet-body');
    if (!this.puppet) {
      body.hidden = true;
      return;
    }
    body.hidden = false;
    const { steps, index } = this.puppet;
    const range = this.element.querySelector('.me-puppet-range');
    range.max = String(Math.max(0, steps.length - 1));
    range.value = String(index);
    this.element.querySelector('.me-puppet-pos').textContent = steps.length ? `${index + 1} / ${steps.length}` : '0 / 0';
    const step = steps[index];
    const view = this.element.querySelector('.me-puppet-step');
    if (!step) {
      view.innerHTML = `<p class="hint">${escapeHtml(t('map.puppet.empty'))}</p>`;
    } else if (step.kind === 'talk') {
      view.innerHTML = `<div class="me-puppet-line">${escapeHtml(t('map.puppet.line', step.line))}</div>`
        + `<div class="me-puppet-talk"><b>${escapeHtml(step.speaker || t('map.puppet.narration'))}</b>${escapeHtml(step.message ?? '')}</div>`;
    } else {
      view.innerHTML = `<div class="me-puppet-line">${escapeHtml(t('map.puppet.line', step.line))}</div><code class="me-puppet-cmd">${escapeHtml(step.text)}</code>`;
    }
    this.element.querySelector('.me-puppet-warnings').innerHTML = (step?.warnings ?? []).map(w => `<li>${escapeHtml(w)}</li>`).join('');
  }

  drawPuppet(ctx, size) {
    const step = this.puppet.steps[this.puppet.index];
    if (!step) {
      return;
    }
    const center = (x, y) => [(x - 0.5) * size, (y - 0.5) * size];
    // この手で動いたユニットの元の位置（薄く）と通り道
    if (step.kind === 'move' && step.fromX && step.toX) {
      const moved = step.units.find(u => u.key === step.target);
      if (moved) {
        this.drawUnitToken(ctx, ...center(step.fromX, step.fromY), size, moved, 0.3, false);
      }
      ctx.save();
      ctx.strokeStyle = '#ffd400';
      ctx.lineWidth = 3;
      ctx.setLineDash(step.path ? [] : [6, 4]);
      ctx.beginPath();
      const points = step.path ?? [[step.fromX, step.fromY], [step.toX, step.toY]];
      points.forEach(([x, y], i) => (i === 0 ? ctx.moveTo(...center(x, y)) : ctx.lineTo(...center(x, y))));
      ctx.stroke();
      ctx.restore();
    }
    // 退いたユニットの元の位置
    if (step.kind === 'escape' && step.fromX) {
      ctx.save();
      ctx.strokeStyle = '#d0393e';
      ctx.lineWidth = 3;
      const [cx, cy] = center(step.fromX, step.fromY);
      ctx.beginPath();
      ctx.moveTo(cx - size / 3, cy - size / 3);
      ctx.lineTo(cx + size / 3, cy + size / 3);
      ctx.moveTo(cx + size / 3, cy - size / 3);
      ctx.lineTo(cx - size / 3, cy + size / 3);
      ctx.stroke();
      ctx.restore();
    }
    for (const u of step.units) {
      const dragging = this.dragPuppet?.unit.key === u.key;
      if (dragging) {
        this.drawUnitToken(ctx, ...center(u.x, u.y), size, u, 0.35, false);
        this.drawUnitToken(ctx, ...center(this.dragPuppet.curX, this.dragPuppet.curY), size, u, 1, true);
      } else {
        const speaking = step.kind === 'talk' && (u.key === step.speaker || u.pilot === step.speaker || u.unit === step.speaker);
        this.drawUnitToken(ctx, ...center(u.x, u.y), size, u, 1, u.key === step.target || speaking);
      }
    }
    if (step.centerX) {
      ctx.strokeStyle = '#ffd400';
      ctx.lineWidth = 3;
      ctx.strokeRect((step.centerX - 1) * size + 1.5, (step.centerY - 1) * size + 1.5, size - 3, size - 3);
    }
  }

  /** ドラッグしたユニットを動かす。今の手が同じユニットの Move なら行き先を書き換え、そうでなければ今の手の後に Move を差し込む */
  async movePuppetUnit(unit, x, y) {
    const step = this.puppet?.steps[this.puppet.index];
    if (!step) {
      return;
    }
    let focusLine;
    if (step.kind === 'move' && step.target === unit.key) {
      await this.options.timeline.editMove(this.puppet.file, step.line, x, y);
      focusLine = step.line;
    } else {
      const result = await this.options.timeline.insertMove(this.puppet.file, step.line, unit.key, x, y);
      focusLine = result?.line;
    }
    await this.reloadPuppet(focusLine);
  }

  drawPlacements(ctx, size) {
    const dim = this.target !== 'placement' ? 0.85 : 1;
    // 移動範囲の目安（選択中ユニット）
    if (this.target === 'placement' && this.moveRangeCells.size > 0) {
      ctx.fillStyle = 'rgba(64, 156, 255, 0.25)';
      ctx.strokeStyle = 'rgba(64, 156, 255, 0.7)';
      ctx.lineWidth = 1;
      for (const key of this.moveRangeCells) {
        const [cx, cy] = key.split(',').map(Number);
        if (cx >= 1 && cy >= 1 && cx <= this.map.width && cy <= this.map.height) {
          ctx.fillRect((cx - 1) * size + 1, (cy - 1) * size + 1, size - 2, size - 2);
          ctx.strokeRect((cx - 1) * size + 1.5, (cy - 1) * size + 1.5, size - 3, size - 3);
        }
      }
    }
    // Move コマンドの移動線
    for (const item of this.placements) {
      if (item.fromX && item.fromY) {
        const isSel = item.id === this.selectedPlacement?.id;
        ctx.save();
        ctx.globalAlpha = dim * (isSel ? 0.95 : 0.65);
        ctx.strokeStyle = PARTY_COLORS[item.party] ?? '#fff';
        ctx.lineWidth = isSel ? 2.5 : 1.5;
        ctx.setLineDash([4, 3]);
        const x1 = (item.fromX - 0.5) * size;
        const y1 = (item.fromY - 0.5) * size;
        const x2 = (item.x - 0.5) * size;
        const y2 = (item.y - 0.5) * size;
        ctx.beginPath();
        ctx.moveTo(x1, y1);
        ctx.lineTo(x2, y2);
        ctx.stroke();
        ctx.restore();
      }
    }
    // ユニット配置アイコン
    for (const item of this.placements) {
      const isDragging = this.dragPlacement?.item.id === item.id;
      const drawX = isDragging ? this.dragPlacement.curX : item.x;
      const drawY = isDragging ? this.dragPlacement.curY : item.y;
      if (isDragging) {
        this.drawUnitToken(ctx, (item.x - 0.5) * size, (item.y - 0.5) * size, size, item, 0.35, false);
      }
      const selected = item.id === this.selectedPlacement?.id;
      this.drawUnitToken(ctx, (drawX - 0.5) * size, (drawY - 0.5) * size, size, item, dim, selected);
    }
    // 補助座標の強調表示（Center・Explode・進入 等）
    if (this.highlightCell) {
      const px = (this.highlightCell.x - 1) * size;
      const py = (this.highlightCell.y - 1) * size;
      ctx.strokeStyle = '#ffd400';
      ctx.lineWidth = 3;
      ctx.strokeRect(px + 1.5, py + 1.5, size - 3, size - 3);
    }
  }

  drawUnitToken(ctx, cx, cy, size, item, alpha, selected) {
    if (item.command === '進入') {
      this.drawGoal(ctx, cx, cy, size, item, alpha, selected);
      return;
    }
    const color = PARTY_COLORS[item.party] ?? '#6e7681';
    const pad = Math.max(2, size * 0.08);
    const left = cx - size / 2 + pad;
    const top = cy - size / 2 + pad;
    const w = size - pad * 2;
    ctx.save();
    ctx.globalAlpha = alpha;
    // 背景色と陣営の枠
    ctx.fillStyle = 'rgba(0,0,0,0.45)';
    ctx.fillRect(left, top, w, w);
    const img = this.getUnitImage(item.bitmap);
    if (img) {
      ctx.drawImage(img, left + 1, top + 1, w - 2, w - 2);
    } else {
      ctx.fillStyle = color;
      ctx.fillRect(left + 1, top + 1, w - 2, w - 2);
      if (size >= 16) {
        ctx.fillStyle = '#fff';
        ctx.font = `bold ${Math.max(8, Math.floor(w * 0.42))}px sans-serif`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const label = (item.unit || item.pilot || item.command || '?').slice(0, 2);
        ctx.fillText(label, cx, cy + 1);
      }
    }
    ctx.strokeStyle = selected ? '#ffd400' : color;
    ctx.lineWidth = selected ? 3 : 2;
    // 出来事で出るユニット（援軍など。7474/SRC#1692）は破線の枠
    if (item.command === 'Organize' || item.byEvent) {
      ctx.setLineDash([3, 2]);
    }
    ctx.strokeRect(left, top, w, w);
    ctx.restore();
  }

  /** 進入の目標のマス（7474/SRC#1692）: マス全体を黄色で塗り、旗を立てる */
  drawGoal(ctx, cx, cy, size, item, alpha, selected) {
    const left = cx - size / 2;
    const top = cy - size / 2;
    ctx.save();
    ctx.globalAlpha = alpha;
    ctx.fillStyle = 'rgba(255, 212, 0, 0.30)';
    ctx.fillRect(left + 1, top + 1, size - 2, size - 2);
    ctx.strokeStyle = selected ? '#ffffff' : '#ffd400';
    ctx.lineWidth = selected ? 3 : 2;
    ctx.setLineDash([5, 3]);
    ctx.strokeRect(left + 1.5, top + 1.5, size - 3, size - 3);
    ctx.setLineDash([]);
    if (size >= 12) {
      // 旗
      const poleX = cx - size * 0.18;
      ctx.strokeStyle = '#ffd400';
      ctx.lineWidth = Math.max(1.5, size * 0.06);
      ctx.beginPath();
      ctx.moveTo(poleX, cy + size * 0.32);
      ctx.lineTo(poleX, cy - size * 0.32);
      ctx.stroke();
      ctx.fillStyle = '#ffd400';
      ctx.beginPath();
      ctx.moveTo(poleX, cy - size * 0.32);
      ctx.lineTo(poleX + size * 0.4, cy - size * 0.18);
      ctx.lineTo(poleX, cy - size * 0.04);
      ctx.closePath();
      ctx.fill();
    }
    ctx.restore();
  }

  getUnitImage(bitmap) {
    if (!bitmap || bitmap === '-.bmp' || !this.options.loadUnitImage) {
      return null;
    }
    if (this.unitImages.has(bitmap)) {
      return this.unitImages.get(bitmap);
    }
    this.unitImages.set(bitmap, null);
    this.options.loadUnitImage(bitmap).then(img => {
      if (img) {
        this.unitImages.set(bitmap, img);
        this.scheduleRender();
      }
    }, () => {});
    return null;
  }

  /** 1 マスを描く。画像が無ければ地形タイプの色と番号 */
  drawTile(ctx, px, py, size, tile, alpha = 1) {
    const info = this.terrainById.get(tile.terrain);
    const key = `${info?.bitmap}|${tile.bitmap}`;
    const image = info ? this.images.get(key) : null;
    ctx.globalAlpha = alpha;
    if (image) {
      ctx.drawImage(image, px, py, size, size);
    } else {
      ctx.fillStyle = info ? (CLASS_COLORS[info.class] ?? '#999') : '#d33';
      ctx.fillRect(px, py, size, size);
      if (size >= 16) {
        ctx.fillStyle = '#000';
        ctx.font = `${Math.max(8, size / 3)}px sans-serif`;
        ctx.fillText(String(tile.terrain), px + 2, py + size / 2);
      }
      if (info && image === undefined) {
        this.requestImage(key, info.bitmap, tile.bitmap);
      }
    }
    ctx.globalAlpha = 1;
  }

  requestImage(key, bitmap, no) {
    this.images.set(key, null);
    this.options.loadImage(bitmap, no).then(img => {
      if (img) {
        this.images.set(key, img);
        this.scheduleRender();
      }
    }, () => {});
  }

  scheduleRender() {
    if (this.renderQueued) {
      return;
    }
    this.renderQueued = true;
    requestAnimationFrame(() => {
      this.renderQueued = false;
      this.render();
      this.renderCurrent();
      this.element.querySelectorAll('.me-bitmaps canvas').forEach(c => {
        const ctx = c.getContext('2d');
        ctx.clearRect(0, 0, TILE, TILE);
        this.drawTile(ctx, 0, 0, TILE, { terrain: this.current.terrain, bitmap: Number(c.dataset.no) });
      });
    });
  }

  // ---- 操作 ----

  pointAt(ev) {
    const rect = this.canvas.getBoundingClientRect();
    const size = TILE * this.zoom * (rect.width / this.canvas.width);
    const x = Math.floor((ev.clientX - rect.left) / size);
    const y = Math.floor((ev.clientY - rect.top) / size);
    return x >= 0 && y >= 0 && x < this.map.width && y < this.map.height ? { x, y } : null;
  }

  async pointerDown(ev) {
    if (!this.map) {
      return;
    }
    const p = this.pointAt(ev);
    if (!p) {
      return;
    }
    this.element.focus({ preventScroll: true });
    this.canvas.setPointerCapture(ev.pointerId);
    if (this.target === 'placement' && this.puppet) {
      const step = this.puppet.steps[this.puppet.index];
      const unit = step?.units.find(u => u.x === p.x + 1 && u.y === p.y + 1);
      if (unit && ev.button === 0) {
        this.dragPuppet = { unit, curX: unit.x, curY: unit.y };
        this.canvas.style.cursor = 'grabbing';
      }
      this.render();
      return;
    }
    if (this.target === 'placement') {
      const hit = this.placementAt(p.x + 1, p.y + 1);
      this.selectPlacement(hit);
      if (hit?.editable && ev.button === 0) {
        this.dragPlacement = { item: hit, startX: hit.x, startY: hit.y, curX: hit.x, curY: hit.y };
        this.canvas.style.cursor = 'grabbing';
      }
      this.render();
      return;
    }
    // 右クリックはスポイト（元の MapEdit と同じ）
    if (ev.button === 2 || this.tool === 'pick') {
      this.pick(p);
      return;
    }
    this.dragStart = p;
    this.dragging = true;
    switch (this.tool) {
      case 'pen':
        this.begin();
        this.paint(p);
        break;
      case 'fill':
        this.begin();
        this.flood(p);
        await this.commit();
        this.dragging = false;
        break;
      case 'paste':
        this.begin();
        this.paste(p);
        await this.commit();
        this.dragging = false;
        break;
      case 'rect':
      case 'copy':
        this.selection = { from: p, to: p };
        break;
    }
    this.render();
  }

  pointerMove(ev) {
    if (!this.map) {
      return;
    }
    const p = this.pointAt(ev);
    this.hover = p;
    if (this.dragPuppet && p) {
      this.dragPuppet.curX = p.x + 1;
      this.dragPuppet.curY = p.y + 1;
      const d = this.dragPuppet;
      this.status.textContent = t('map.placements.dragging', d.unit.key, d.unit.x, d.unit.y, d.curX, d.curY);
      this.render();
      return;
    }
    if (this.dragPlacement && p) {
      this.dragPlacement.curX = p.x + 1;
      this.dragPlacement.curY = p.y + 1;
      const d = this.dragPlacement;
      this.status.textContent = t('map.placements.dragging', d.item.unit || d.item.pilot || d.item.command, d.startX, d.startY, d.curX, d.curY);
      this.render();
      return;
    }
    if (p) {
      const tile = this.cell(this.map.tiles, p.x, p.y);
      const info = this.terrainById.get(tile.terrain);
      let text = `(${p.x + 1},${p.y + 1}) ${tile.terrain} ${info?.name ?? t('map.undefined')}（${tile.bitmap}）`;
      if (this.map.layer) {
        const l = this.cell(this.map.layer, p.x, p.y);
        if (l.terrain !== NO_LAYER) {
          text += ` / ${t('map.targetLayer')}: ${l.terrain} ${this.terrainById.get(l.terrain)?.name ?? t('map.undefined')}（${l.bitmap}）`;
        }
      }
      if (this.target === 'placement' || this.showPlacements) {
        const hits = this.placements.filter(pl => pl.x === p.x + 1 && pl.y === p.y + 1);
        if (hits.length > 0) {
          text += ' — ' + hits.map(pl => pl.unit || pl.pilot || pl.command).join(', ');
        }
      }
      this.status.textContent = text;
    }
    if (this.dragging && p) {
      if (this.tool === 'pen') {
        this.paint(p);
      } else if (this.selection) {
        this.selection.to = p;
      }
    }
    this.render();
  }

  async pointerUp() {
    if (this.dragPuppet) {
      const drag = this.dragPuppet;
      this.dragPuppet = null;
      this.canvas.style.cursor = 'grab';
      if (drag.curX !== drag.unit.x || drag.curY !== drag.unit.y) {
        await this.movePuppetUnit(drag.unit, drag.curX, drag.curY);
      } else {
        this.render();
      }
      return;
    }
    if (this.dragPlacement) {
      const drag = this.dragPlacement;
      this.dragPlacement = null;
      this.canvas.style.cursor = 'grab';
      if (drag.curX !== drag.startX || drag.curY !== drag.startY) {
        await this.movePlacement(drag.item, drag.curX, drag.curY);
      } else {
        this.render();
      }
      return;
    }
    if (!this.dragging) {
      return;
    }
    this.dragging = false;
    if (this.tool === 'pen') {
      await this.commit();
    } else if (this.tool === 'rect' && this.selection) {
      this.begin();
      const b = this.selectionBox();
      for (let y = b.y0; y <= b.y1; y++) {
        for (let x = b.x0; x <= b.x1; x++) {
          this.set(x, y);
        }
      }
      this.selection = null;
      await this.commit();
    } else if (this.tool === 'copy' && this.selection) {
      this.copy();
      this.setTool('paste');
    }
    this.render();
  }

  keyDown(ev) {
    const mod = ev.ctrlKey || ev.metaKey;
    if (mod && ev.key.toLowerCase() === 'z' && !ev.shiftKey) {
      ev.preventDefault();
      this.undo();
      return;
    }
    if (mod && (ev.key.toLowerCase() === 'y' || (ev.key.toLowerCase() === 'z' && ev.shiftKey))) {
      ev.preventDefault();
      this.redo();
      return;
    }
    if (mod || ev.altKey || /^(INPUT|SELECT|TEXTAREA)$/.test(ev.target?.tagName ?? '')) {
      return;
    }
    const key = ev.key.toLowerCase();
    if (this.target === 'placement' && this.puppet) {
      if (ev.key === 'ArrowLeft') {
        ev.preventDefault();
        this.setPuppetIndex(this.puppet.index - 1);
      } else if (ev.key === 'ArrowRight') {
        ev.preventDefault();
        this.setPuppetIndex(this.puppet.index + 1);
      }
      return;
    }
    if (this.target !== 'placement') {
      const toolByKey = { b: 'pen', p: 'pen', r: 'rect', g: 'fill', f: 'fill', i: 'pick', c: 'copy', v: 'paste' };
      if (toolByKey[key]) {
        ev.preventDefault();
        this.setTool(toolByKey[key]);
        return;
      }
    }
    if (ev.key === '+' || ev.key === '=') {
      ev.preventDefault();
      this.setZoom(this.zoom * 2);
    } else if (ev.key === '-') {
      ev.preventDefault();
      this.setZoom(this.zoom / 2);
    } else if (ev.key === '0') {
      ev.preventDefault();
      this.setZoom(1);
    }
  }

  selectionBox() {
    if (!this.selection) {
      return null;
    }
    const { from, to } = this.selection;
    return { x0: Math.min(from.x, to.x), y0: Math.min(from.y, to.y), x1: Math.max(from.x, to.x), y1: Math.max(from.y, to.y) };
  }

  /** 書く先（地形かレイヤー）。レイヤーに書くとき、まだ無ければ作る */
  source() {
    if (this.target === 'layer') {
      this.map.layer ??= Array.from({ length: this.map.width * this.map.height * 2 }, () => NO_LAYER);
      return this.map.layer;
    }
    return this.map.tiles;
  }

  set(x, y, tile = this.current) {
    // 地形に「レイヤーを消す」は書かない
    if (this.target === 'tiles' && tile.terrain === NO_LAYER) {
      return;
    }
    const s = this.source();
    const i = (y * this.map.width + x) * 2;
    s[i] = tile.terrain;
    s[i + 1] = tile.bitmap;
  }

  paint(p) {
    this.set(p.x, p.y);
  }

  flood(p) {
    const s = this.source();
    const start = this.cell(s, p.x, p.y);
    if (start.terrain === this.current.terrain && start.bitmap === this.current.bitmap) {
      return;
    }
    const stack = [p];
    const seen = new Set();
    while (stack.length) {
      const q = stack.pop();
      const k = q.y * this.map.width + q.x;
      if (seen.has(k) || q.x < 0 || q.y < 0 || q.x >= this.map.width || q.y >= this.map.height) {
        continue;
      }
      seen.add(k);
      const c = this.cell(s, q.x, q.y);
      if (c.terrain !== start.terrain || c.bitmap !== start.bitmap) {
        continue;
      }
      this.set(q.x, q.y);
      stack.push({ x: q.x + 1, y: q.y }, { x: q.x - 1, y: q.y }, { x: q.x, y: q.y + 1 }, { x: q.x, y: q.y - 1 });
    }
  }

  pick(p) {
    const s = this.target === 'layer' && this.map.layer ? this.map.layer : this.map.tiles;
    const c = this.cell(s, p.x, p.y);
    this.setCurrent(c.terrain, c.bitmap);
  }

  copy() {
    const b = this.selectionBox();
    const width = b.x1 - b.x0 + 1;
    const height = b.y1 - b.y0 + 1;
    const tiles = [];
    const layer = [];
    for (let y = b.y0; y <= b.y1; y++) {
      for (let x = b.x0; x <= b.x1; x++) {
        tiles.push(this.cell(this.map.tiles, x, y));
        layer.push(this.map.layer ? this.cell(this.map.layer, x, y) : { terrain: NO_LAYER, bitmap: NO_LAYER });
      }
    }
    this.clipboard = { width, height, tiles, layer };
    this.selection = null;
    this.status.textContent = t('map.copied', width, height);
  }

  /** 写した範囲を、地形とレイヤーごと貼る */
  paste(p) {
    if (!this.clipboard) {
      return;
    }
    const { width, height, tiles, layer } = this.clipboard;
    const hasLayer = layer.some(l => l.terrain !== NO_LAYER);
    if (hasLayer) {
      this.map.layer ??= Array.from({ length: this.map.width * this.map.height * 2 }, () => NO_LAYER);
    }
    for (let dy = 0; dy < height; dy++) {
      for (let dx = 0; dx < width; dx++) {
        const x = p.x + dx;
        const y = p.y + dy;
        if (x >= this.map.width || y >= this.map.height) {
          continue;
        }
        const i = (y * this.map.width + x) * 2;
        const tile = tiles[dy * width + dx];
        this.map.tiles[i] = tile.terrain;
        this.map.tiles[i + 1] = tile.bitmap;
        if (this.map.layer) {
          const l = layer[dy * width + dx];
          this.map.layer[i] = l.terrain;
          this.map.layer[i + 1] = l.bitmap;
        }
      }
    }
  }

  // ---- 元に戻す・書き出す ----

  snapshot() {
    return { width: this.map.width, height: this.map.height, tiles: [...this.map.tiles], layer: this.map.layer ? [...this.map.layer] : null };
  }

  begin() {
    this.before = this.snapshot();
  }

  /** 一筆の終わり。変わっていれば自動接続を掛け、履歴に積んで書き出して知らせる */
  async commit(applyAuto = true) {
    const before = this.before;
    this.before = null;
    if (!before) {
      return;
    }
    const mid = this.snapshot();
    if (JSON.stringify(before) === JSON.stringify(mid)) {
      return;
    }
    if (applyAuto && this.autoConnect && this.options.autoConnect) {
      const updated = await this.options.autoConnect(this.map);
      if (updated?.tiles) {
        this.map.tiles = Array.from(updated.tiles);
        this.map.layer = updated.layer ? Array.from(updated.layer) : null;
        this.render();
      }
    }
    const after = this.snapshot();
    if (JSON.stringify(before) === JSON.stringify(after)) {
      return;
    }
    if (this.options.undo === undefined || this.options.undo === 'own') {
      this.history.push(before);
      this.future = [];
      this.updateButtons();
    }
    await this.emit();
  }

  /** マップ全体の地形の境目と変種をつなぎ直す */
  async reconnect() {
    if (!this.map || !this.options.autoConnect) {
      return;
    }
    this.begin();
    const updated = await this.options.autoConnect(this.map);
    if (updated?.tiles) {
      this.map.tiles = Array.from(updated.tiles);
      this.map.layer = updated.layer ? Array.from(updated.layer) : null;
      this.render();
    }
    await this.commit(false);
  }

  async emit() {
    const text = await this.options.serialize(this.map);
    this.options.onEdit?.(text);
  }

  undo() {
    if (this.options.undo && this.options.undo !== 'own') {
      this.options.undo.undo();
      return;
    }
    const prev = this.history.pop();
    if (!prev) {
      return;
    }
    this.future.push(this.snapshot());
    Object.assign(this.map, prev);
    this.render();
    this.updateButtons();
    this.emit();
  }

  redo() {
    if (this.options.undo && this.options.undo !== 'own') {
      this.options.undo.redo();
      return;
    }
    const next = this.future.pop();
    if (!next) {
      return;
    }
    this.history.push(this.snapshot());
    Object.assign(this.map, next);
    this.render();
    this.updateButtons();
    this.emit();
  }

  updateButtons() {
    if (this.options.undo && this.options.undo !== 'own') {
      return;
    }
    this.element.querySelector('.me-undo').disabled = this.history.length === 0;
    this.element.querySelector('.me-redo').disabled = this.future.length === 0;
  }

  openResize() {
    if (!this.map) {
      return;
    }
    const dialog = this.element.querySelector('.me-resize-dialog');
    const form = dialog.querySelector('form');
    form.width.value = this.map.width;
    form.height.value = this.map.height;
    dialog.onclose = () => {
      if (dialog.returnValue === 'ok') {
        void this.resize(Number(form.width.value), Number(form.height.value));
      }
    };
    dialog.showModal();
  }

  /** 大きさを変える。左上をそろえ、増えたマスは描く地形で埋める */
  async resize(width, height) {
    if (!(width >= 1 && height >= 1 && width <= 50 && height <= 50)) {
      return;
    }
    this.begin();
    const old = this.snapshot();
    const fill = this.current.terrain === NO_LAYER ? this.cell(old.tiles, 0, 0) : this.current;
    const tiles = [];
    const layer = old.layer ? [] : null;
    for (let y = 0; y < height; y++) {
      for (let x = 0; x < width; x++) {
        const inside = x < old.width && y < old.height;
        const tile = inside ? this.cell(old.tiles, x, y) : fill;
        tiles.push(tile.terrain, tile.bitmap);
        if (layer) {
          const l = inside ? this.cell(old.layer, x, y) : { terrain: NO_LAYER, bitmap: NO_LAYER };
          layer.push(l.terrain, l.bitmap);
        }
      }
    }
    Object.assign(this.map, { width, height, tiles, layer });
    await this.commit();
    this.render();
  }
}

/** 新しいマップ（全て fill の地形） */
export function newMap(width, height, fill) {
  const tiles = [];
  for (let i = 0; i < width * height; i++) {
    tiles.push(fill.terrain, fill.bitmap);
  }
  return { width, height, tiles, layer: null, template: 0, legacy: false };
}

function escapeHtml(s) {
  return String(s).replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c]);
}
