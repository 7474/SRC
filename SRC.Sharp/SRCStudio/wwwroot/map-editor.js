// SRC#Studio のマップ作成ツールの画面（7474/SRC#1641）。ファイルの読み書きは使う側（index.html の studio.js・VS Code の vscode.js）が行う。
//
// const editor = new MapEditor(element, {
//   loadImage: async (bitmap, no) => ImageBitmap | null,   // 地形の画像
//   imageNumbers: async bitmap => number[],                 // 手元にある画像の番号（パレット）
//   serialize: async map => string,                          // マップをテキストにする（studio-core.js の serializeMap）
//   onEdit: text => {},                                      // 編集のたび（一筆ごと）に、書き出したテキスト
//   undo: 'own' | { undo: () => {}, redo: () => {} },        // 元に戻す（既定は自前。VS Code は文書の元に戻すを使う）
// });
// editor.setTerrains(terrains); editor.setMap(map);   // map は studio-core.js の parseMap の結果
import { t } from './i18n.js';

const TILE = 32;
const NO_LAYER = 10000;
const CLASS_COLORS = { '陸': '#8db360', '屋内': '#b49a74', '月面': '#b9b9b9', '水': '#4f86c6', '深水': '#2c5a92', '空': '#bcd8f0', '宇宙': '#1b1b3a' };
const TOOLS = ['pen', 'rect', 'fill', 'pick', 'copy', 'paste'];

export class MapEditor {
  constructor(element, options) {
    this.options = options;
    this.terrains = [];
    this.terrainById = new Map();
    this.map = null;
    this.tool = 'pen';
    this.target = 'tiles';
    this.current = { terrain: 0, bitmap: 0 };
    this.zoom = 1;
    this.grid = true;
    this.history = [];
    this.future = [];
    this.images = new Map();
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
        <span class="me-group">
          <label><input type="radio" name="me-target" value="tiles" checked> ${t('map.targetTiles')}</label>
          <label><input type="radio" name="me-target" value="layer"> ${t('map.targetLayer')}</label>
          <button type="button" class="me-erase" title="${t('map.eraseLayerHint')}">${t('map.eraseLayer')}</button>
        </span>
        <span class="me-group">
          <button type="button" class="me-undo" title="Ctrl+Z">${t('map.undo')}</button>
          <button type="button" class="me-redo" title="Ctrl+Y">${t('map.redo')}</button>
        </span>
        <span class="me-group">
          <button type="button" class="me-zoom-out" aria-label="${t('map.zoomOut')}">−</button>
          <span class="me-zoom-label">100%</span>
          <button type="button" class="me-zoom-in" aria-label="${t('map.zoomIn')}">＋</button>
          <label><input type="checkbox" class="me-grid" checked> ${t('map.grid')}</label>
        </span>
        <span class="me-group">
          <button type="button" class="me-resize">${t('map.resize')}</button>
        </span>
      </div>
      <div class="me-body">
        <div class="me-canvas-wrap"><canvas class="me-canvas"></canvas></div>
        <div class="me-palette">
          <div class="me-current"><canvas width="${TILE}" height="${TILE}"></canvas><span></span></div>
          <select class="me-class"></select>
          <input type="search" class="me-search" placeholder="${t('map.search')}">
          <ul class="me-terrains" role="listbox"></ul>
          <div class="me-bitmaps"></div>
        </div>
      </div>
      <div class="me-status" aria-live="polite"></div>
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
      b.title = t(`map.toolHint.${tool}`);
      b.addEventListener('click', () => this.setTool(tool));
      tools.append(b);
    }
    this.canvas = e.querySelector('.me-canvas');
    this.ctx = this.canvas.getContext('2d');
    this.status = e.querySelector('.me-status');
    e.querySelectorAll('input[name="me-target"]').forEach(r => r.addEventListener('change', () => { this.target = r.value; this.render(); }));
    e.querySelector('.me-erase').addEventListener('click', () => this.setCurrent(NO_LAYER, NO_LAYER, 'layer'));
    e.querySelector('.me-undo').addEventListener('click', () => this.undo());
    e.querySelector('.me-redo').addEventListener('click', () => this.redo());
    e.querySelector('.me-zoom-in').addEventListener('click', () => this.setZoom(this.zoom * 2));
    e.querySelector('.me-zoom-out').addEventListener('click', () => this.setZoom(this.zoom / 2));
    e.querySelector('.me-grid').addEventListener('change', ev => { this.grid = ev.target.checked; this.render(); });
    e.querySelector('.me-resize').addEventListener('click', () => this.openResize());
    e.querySelector('.me-class').addEventListener('change', () => this.renderTerrainList());
    e.querySelector('.me-search').addEventListener('input', () => this.renderTerrainList());
    this.canvas.addEventListener('contextmenu', ev => ev.preventDefault());
    this.canvas.addEventListener('pointerdown', ev => this.pointerDown(ev));
    this.canvas.addEventListener('pointermove', ev => this.pointerMove(ev));
    this.canvas.addEventListener('pointerup', ev => this.pointerUp(ev));
    this.canvas.addEventListener('pointerleave', () => { this.hover = null; this.render(); });
    e.tabIndex = 0;
    e.addEventListener('keydown', ev => this.keyDown(ev));
    this.setTool('pen');
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
      li.textContent = `${terrain.id} ${terrain.name}`;
      li.title = `${terrain.class}・${t('map.moveCost')} ${terrain.moveCost}・${t('map.hit')} ${terrain.hitMod}・${t('map.damage')} ${terrain.damageMod}${terrain.features.length ? '・' + terrain.features.join(' ') : ''}`;
      li.setAttribute('aria-selected', String(terrain.id === this.current.terrain));
      li.addEventListener('click', () => this.setCurrent(terrain.id, null));
      list.append(li);
    }
  }

  /** 描く地形を選ぶ。bitmap が null なら、その地形で手元にある最初の画像の番号 */
  async setCurrent(terrain, bitmap, target) {
    if (target) {
      this.target = target;
      this.element.querySelector(`input[name="me-target"][value="${target}"]`).checked = true;
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
    if (terrain === NO_LAYER) {
      this.element.querySelector('.me-current span').textContent = t('map.eraseLayer');
      return;
    }
    this.drawTile(ctx, 0, 0, TILE, this.current);
    const info = this.terrainById.get(terrain);
    this.element.querySelector('.me-current span').textContent = `${terrain} ${info?.name ?? t('map.undefined')}（${bitmap}）`;
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
    const box = this.selectionBox();
    if (box) {
      ctx.strokeStyle = '#e8b400';
      ctx.lineWidth = 2;
      ctx.strokeRect(box.x0 * size + 1, box.y0 * size + 1, (box.x1 - box.x0 + 1) * size - 2, (box.y1 - box.y0 + 1) * size - 2);
    }
    if (this.hover) {
      ctx.strokeStyle = '#fff';
      ctx.lineWidth = 2;
      const w = this.tool === 'paste' && this.clipboard ? this.clipboard.width : 1;
      const h = this.tool === 'paste' && this.clipboard ? this.clipboard.height : 1;
      ctx.strokeRect(this.hover.x * size + 1, this.hover.y * size + 1, w * size - 2, h * size - 2);
    }
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

  pointerDown(ev) {
    if (!this.map) {
      return;
    }
    const p = this.pointAt(ev);
    if (!p) {
      return;
    }
    this.element.focus({ preventScroll: true });
    this.canvas.setPointerCapture(ev.pointerId);
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
        this.commit();
        this.dragging = false;
        break;
      case 'paste':
        this.begin();
        this.paste(p);
        this.commit();
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

  pointerUp() {
    if (!this.dragging) {
      return;
    }
    this.dragging = false;
    if (this.tool === 'pen') {
      this.commit();
    } else if (this.tool === 'rect' && this.selection) {
      this.begin();
      const b = this.selectionBox();
      for (let y = b.y0; y <= b.y1; y++) {
        for (let x = b.x0; x <= b.x1; x++) {
          this.set(x, y);
        }
      }
      this.selection = null;
      this.commit();
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
    } else if (mod && (ev.key.toLowerCase() === 'y' || (ev.key.toLowerCase() === 'z' && ev.shiftKey))) {
      ev.preventDefault();
      this.redo();
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

  /** 一筆の終わり。変わっていれば履歴に積み、書き出して知らせる */
  async commit() {
    const before = this.before;
    this.before = null;
    if (!before) {
      return;
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
        this.resize(Number(form.width.value), Number(form.height.value));
      }
    };
    dialog.showModal();
  }

  /** 大きさを変える。左上をそろえ、増えたマスは描く地形で埋める */
  resize(width, height) {
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
    this.commit();
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
