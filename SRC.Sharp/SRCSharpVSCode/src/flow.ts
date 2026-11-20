import { spawn } from 'child_process';
import * as vscode from 'vscode';
import { scenarioRoot } from './hotReload';
import { resolveServer, serverCommand } from './server';
import { webviewHtml } from './webview';

export interface FlowNode {
  id: string;
  path: string;
  fullPath?: string;
  title: string;
  kind: 'Stage' | 'Intermission' | 'SubStage' | 'Include' | 'LoadData' | 'GameClear' | 'Missing';
  exists: boolean;
  entry: boolean;
  line: number;
  labels: string[];
  joins?: string[];
  leaves?: string[];
}

export interface FlowEdge {
  from: string;
  to: string;
  kind: 'Continue' | 'NextStage' | 'Exec' | 'IntermissionCommand' | 'IntermissionReturn' | 'Load' | 'GameClear';
  file: string;
  fullFile?: string;
  line: number;
  label?: string;
  condition?: string;
  command: string;
  commandArg?: string;
  target?: string;
  resolvedTarget?: string;
  dynamic?: boolean;
  missing?: boolean;
}

export interface FlowWarning {
  file: string;
  fullFile?: string;
  line: number;
  target?: string;
  message: string;
}

export interface ScenarioFlowData {
  scenarioRoot: string;
  scenarioTitle: string;
  appPath?: string;
  nodes: FlowNode[];
  edges: FlowEdge[];
  warnings: FlowWarning[];
  mermaid: string;
}

export interface FlowStatus {
  scenarioRoot?: string;
  scenarioTitle?: string;
  nodes: number;
  edges: number;
  warnings: number;
  rendered: boolean;
  error?: string;
}

type WebviewMessage =
  | { type: 'ready' }
  | { type: 'refresh' }
  | { type: 'copyMermaid' }
  | { type: 'openMarkdown' }
  | { type: 'openFile'; path?: string; line?: number }
  | { type: 'rendered'; nodes: number; edges: number; warnings: number };

/**
 * 話のつながりの図パネル（7474/SRC#1652）。
 * srcsharp flow --format json の結果をインタラクティブな SVG フローチャートとして表示し、
 * ノード・エッジのクリックで該当の .eve ファイル・行へ飛べる。
 */
export class EpisodeFlowPanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private ready = false;
  private targetRoot: string | undefined;
  private data: ScenarioFlowData | undefined;
  private lastError: string | undefined;
  private rendered = false;
  private timer: NodeJS.Timeout | undefined;
  private readonly disposables: vscode.Disposable[] = [];

  constructor() {
    this.disposables.push(
      vscode.workspace.onDidSaveTextDocument((doc) => {
        if (!this.panel || !this.targetRoot) {
          return;
        }
        if (doc.languageId === 'src-eve' || doc.fileName.toLowerCase().endsWith('.eve')) {
          this.scheduleRefresh(300);
        }
      }),
    );
  }

  public getStatus(): FlowStatus {
    return {
      scenarioRoot: this.targetRoot,
      scenarioTitle: this.data?.scenarioTitle,
      nodes: this.data?.nodes.length ?? 0,
      edges: this.data?.edges.length ?? 0,
      warnings: this.data?.warnings.length ?? 0,
      rendered: this.rendered,
      ...(this.lastError ? { error: this.lastError } : {}),
    };
  }

  public async show(explicitPath?: string): Promise<void> {
    const resolved = explicitPath ?? this.detectScenarioRoot();
    if (!resolved) {
      void vscode.window.showErrorMessage('SRC#: 話のつながりを調べるシナリオのフォルダかイベントファイル（*.eve）を開いてください。');
      return;
    }
    this.targetRoot = resolved;
    this.rendered = false;
    this.lastError = undefined;
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.episodeFlow',
        'SRC# 話のつながり',
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# 話のつながり', STYLE, BODY, SCRIPT);
    } else {
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    await this.refresh();
  }

  /** テストや外部から JSON データを直接渡して表示する */
  public async showData(flowData: ScenarioFlowData): Promise<void> {
    this.targetRoot = flowData.scenarioRoot;
    this.data = flowData;
    this.lastError = undefined;
    this.rendered = false;
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.episodeFlow',
        `SRC# 話のつながり: ${flowData.scenarioTitle}`,
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# 話のつながり', STYLE, BODY, SCRIPT);
    } else {
      this.panel.title = `SRC# 話のつながり: ${flowData.scenarioTitle}`;
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    this.postState();
  }

  public async refresh(): Promise<void> {
    if (!this.targetRoot) {
      return;
    }
    const server = resolveServer();
    if (!server.found) {
      this.lastError = 'srcsharp が見つかりません。';
      this.postState();
      return;
    }
    const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
    const args = [this.targetRoot, '--format', 'json', ...(appPath ? ['--app-path', appPath] : [])];
    const cmd = serverCommand(server, 'flow', args);

    try {
      const jsonText = await new Promise<string>((resolve, reject) => {
        const child = spawn(cmd.command, cmd.args, { cwd: this.targetRoot, env: server.env });
        let stdout = '';
        let stderr = '';
        child.stdout.on('data', (d: Buffer) => { stdout += d.toString(); });
        child.stderr.on('data', (d: Buffer) => { stderr += d.toString(); });
        child.on('error', reject);
        child.on('close', (code) => {
          if (code === 0) {
            resolve(stdout);
          } else {
            reject(new Error(stderr.trim() || `srcsharp flow が終了コード ${code} で失敗しました`));
          }
        });
      });
      this.data = JSON.parse(jsonText) as ScenarioFlowData;
      this.lastError = undefined;
      if (this.panel && this.data.scenarioTitle) {
        this.panel.title = `SRC# 話のつながり: ${this.data.scenarioTitle}`;
      }
    } catch (e) {
      this.lastError = (e as Error)?.message ?? String(e);
    }
    this.postState();
  }

  private detectScenarioRoot(): string | undefined {
    const editor = vscode.window.activeTextEditor;
    if (editor && editor.document.uri.scheme === 'file') {
      return scenarioRoot(editor.document.uri.fsPath);
    }
    return vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
  }

  private scheduleRefresh(delay = 300): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.timer = setTimeout(() => void this.refresh(), delay);
  }

  private postState(): void {
    if (!this.panel || !this.ready) {
      return;
    }
    void this.panel.webview.postMessage({
      type: 'flow',
      data: this.data,
      error: this.lastError,
    });
  }

  private async receive(msg: WebviewMessage): Promise<void> {
    switch (msg.type) {
      case 'ready':
        this.ready = true;
        this.postState();
        break;
      case 'refresh':
        await this.refresh();
        break;
      case 'copyMermaid':
        if (this.data?.mermaid) {
          await vscode.env.clipboard.writeText(this.data.mermaid);
          void vscode.window.showInformationMessage('SRC#: 話のつながりの Mermaid 図をクリップボードにコピーしました。');
        }
        break;
      case 'openMarkdown':
        if (this.data) {
          const md = buildMarkdown(this.data);
          const doc = await vscode.workspace.openTextDocument({ language: 'markdown', content: md });
          await vscode.window.showTextDocument(doc, vscode.ViewColumn.Active);
        }
        break;
      case 'openFile':
        if (msg.path) {
          try {
            const doc = await vscode.workspace.openTextDocument(vscode.Uri.file(msg.path));
            const editor = await vscode.window.showTextDocument(doc, vscode.ViewColumn.One);
            const line = Math.max(0, (msg.line ?? 1) - 1);
            const pos = new vscode.Position(line, 0);
            editor.selection = new vscode.Selection(pos, pos);
            editor.revealRange(new vscode.Range(pos, pos), vscode.TextEditorRevealType.InCenterIfOutsideViewport);
          } catch {
            void vscode.window.showWarningMessage(`SRC#: ファイルを開けませんでした: ${msg.path}`);
          }
        }
        break;
      case 'rendered':
        this.rendered = true;
        break;
    }
  }

  public dispose(): void {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.panel?.dispose();
    for (const d of this.disposables) {
      d.dispose();
    }
  }
}

function buildMarkdown(data: ScenarioFlowData): string {
  const lines = [
    `# 話のつながり（${data.scenarioTitle || 'シナリオ'}）`,
    '',
    '```mermaid',
    data.mermaid.trimEnd(),
    '```',
    '',
  ];
  if (data.warnings.length > 0) {
    lines.push('## 見つからない先（警告）', '');
    for (const w of data.warnings) {
      lines.push(`- \`${w.file}:${w.line}\`: ${w.message}`);
    }
    lines.push('');
  }
  return lines.join('\n');
}

const STYLE = `
body { margin: 0; padding: 12px 16px; display: flex; flex-direction: column; gap: 12px; }
.toolbar { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; border-bottom: 1px solid var(--vscode-panel-border); padding-bottom: 10px; }
.title-area h2 { margin: 0; font-size: 1.15em; }
.title-area .stats { font-size: 0.9em; color: var(--vscode-descriptionForeground); margin-top: 2px; }
.actions { display: flex; gap: 6px; flex-wrap: wrap; }
.legend { display: flex; flex-wrap: wrap; gap: 12px; font-size: 0.85em; color: var(--vscode-descriptionForeground); }
.legend-item { display: inline-flex; align-items: center; gap: 5px; }
.swatch { width: 12px; height: 12px; border-radius: 3px; border: 1.5px solid; display: inline-block; }
.swatch.stage { background: var(--vscode-editorWidget-background); border-color: var(--vscode-textLink-foreground); }
.swatch.intermission { background: var(--vscode-editorWidget-background); border-color: var(--vscode-charts-purple, #b180d7); border-radius: 99px; }
.swatch.load { background: var(--vscode-editorWidget-background); border-color: var(--vscode-charts-blue, #3794ff); }
.swatch.clear { background: var(--vscode-editorWidget-background); border-color: var(--vscode-charts-green, #89d185); border-radius: 99px; }
.swatch.missing { background: var(--vscode-editorWidget-background); border-color: var(--vscode-editorError-foreground); border-style: dashed; }
.warnings { border: 1px solid var(--vscode-editorWarning-foreground); background: color-mix(in srgb, var(--vscode-editorWarning-foreground) 10%, transparent); border-radius: 4px; padding: 8px 12px; }
.warnings h3 { margin: 0 0 6px; font-size: 0.95em; color: var(--vscode-editorWarning-foreground); }
.warning-item { cursor: pointer; padding: 2px 0; text-decoration: underline; font-size: 0.9em; }
.warning-item:hover { color: var(--vscode-textLink-activeForeground); }
.canvas-wrap { border: 1px solid var(--vscode-panel-border); border-radius: 6px; overflow: auto; background: var(--vscode-editor-background); min-height: 240px; padding: 12px; }
svg.flow-svg { display: block; min-width: 100%; }
.node-group { cursor: pointer; }
.node-box { fill: var(--vscode-editorWidget-background); stroke: var(--vscode-textLink-foreground); stroke-width: 1.8; }
.node-group.entry .node-box { stroke-width: 2.6; }
.node-group.intermission .node-box { stroke: var(--vscode-charts-purple, #b180d7); }
.node-group.load .node-box { stroke: var(--vscode-charts-blue, #3794ff); }
.node-group.substage .node-box, .node-group.include .node-box { stroke: var(--vscode-descriptionForeground); stroke-dasharray: 4 2; }
.node-group.clear .node-box { stroke: var(--vscode-charts-green, #89d185); stroke-width: 2.2; }
.node-group.missing .node-box { stroke: var(--vscode-editorError-foreground); stroke-dasharray: 5 4; }
.node-group:hover .node-box { filter: brightness(1.15); stroke-width: 2.8; }
.node-title { fill: var(--vscode-foreground); font-size: 12.5px; font-weight: 600; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
.node-sub { fill: var(--vscode-descriptionForeground); font-size: 10.5px; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
.node-cast { fill: var(--vscode-charts-green, #89d185); font-size: 10.5px; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
.edge-group { cursor: pointer; }
.edge-path { fill: none; stroke: var(--vscode-foreground); stroke-opacity: 0.65; stroke-width: 1.7; }
.edge-group.exec .edge-path { stroke-width: 2.5; stroke: var(--vscode-textLink-foreground); }
.edge-group.intermission .edge-path { stroke-dasharray: 5 4; stroke: var(--vscode-charts-purple, #b180d7); }
.edge-group.load .edge-path { stroke-dasharray: 3 3; stroke: var(--vscode-charts-blue, #3794ff); }
.edge-group.clear .edge-path { stroke: var(--vscode-charts-green, #89d185); }
.edge-group.missing .edge-path { stroke: var(--vscode-editorError-foreground); stroke-dasharray: 5 3; }
.edge-group:hover .edge-path { stroke-opacity: 1; stroke-width: 2.8; }
.edge-label-bg { fill: var(--vscode-editor-background); stroke: var(--vscode-panel-border); stroke-width: 1; rx: 4; opacity: 0.92; }
.edge-label { fill: var(--vscode-foreground); font-size: 10.5px; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
table.edge-table { width: 100%; border-collapse: collapse; font-size: 0.9em; }
table.edge-table th, table.edge-table td { border-bottom: 1px solid var(--vscode-panel-border); padding: 5px 8px; text-align: left; }
table.edge-table th { color: var(--vscode-descriptionForeground); font-weight: 600; }
table.edge-table tr.clickable { cursor: pointer; }
table.edge-table tr.clickable:hover { background: var(--vscode-list-hoverBackground); }
.badge { display: inline-block; padding: 1px 6px; border-radius: 3px; font-size: 0.85em; border: 1px solid var(--vscode-panel-border); }
.empty { padding: 24px; text-align: center; color: var(--vscode-descriptionForeground); }
`;

const BODY = `
<div class="toolbar">
  <div class="title-area">
    <h2 id="title">話のつながり</h2>
    <div id="stats" class="stats">読み込み中…</div>
  </div>
  <div class="actions">
    <button id="btn-refresh" class="secondary">再読み込み</button>
    <button id="btn-copy" class="secondary">Mermaid をコピー</button>
    <button id="btn-md" class="secondary">Markdown で開く</button>
  </div>
</div>
<div class="legend">
  <span class="legend-item"><span class="swatch stage"></span>話（太枠は入口）</span>
  <span class="legend-item"><span class="swatch intermission"></span>インターミッション</span>
  <span class="legend-item"><span class="swatch load"></span>Load データ</span>
  <span class="legend-item"><span class="swatch clear"></span>GameClear</span>
  <span class="legend-item"><span class="swatch missing"></span>見つからない先</span>
</div>
<div id="error" class="error" style="display:none"></div>
<div id="warnings" class="warnings" style="display:none"></div>
<div id="canvas-wrap" class="canvas-wrap"></div>
<div id="table-wrap"></div>
`;

const SCRIPT = `
(function () {
  const titleEl = document.getElementById('title');
  const statsEl = document.getElementById('stats');
  const errorEl = document.getElementById('error');
  const warningsEl = document.getElementById('warnings');
  const canvasWrap = document.getElementById('canvas-wrap');
  const tableWrap = document.getElementById('table-wrap');

  document.getElementById('btn-refresh').addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
  document.getElementById('btn-copy').addEventListener('click', () => vscode.postMessage({ type: 'copyMermaid' }));
  document.getElementById('btn-md').addEventListener('click', () => vscode.postMessage({ type: 'openMarkdown' }));

  function svgEl(tag, attrs, text) {
    const el = document.createElementNS('http://www.w3.org/2000/svg', tag);
    if (attrs) {
      for (const k of Object.keys(attrs)) {
        el.setAttribute(k, String(attrs[k]));
      }
    }
    if (text !== undefined) {
      el.textContent = text;
    }
    return el;
  }

  function truncate(s, max) {
    if (!s) return '';
    return s.length > max ? s.slice(0, max - 1) + '…' : s;
  }

  function formatEdgeLabel(edge) {
    const parts = [];
    if (edge.kind === 'IntermissionCommand') {
      parts.push(edge.commandArg ? 'メニュー: ' + edge.commandArg : 'インターミッション');
    } else if (edge.kind === 'IntermissionReturn') {
      parts.push('戻る');
    } else if (edge.kind === 'Load') {
      parts.push('Load');
    } else if (edge.kind === 'Exec') {
      parts.push(edge.commandArg ? 'Exec (' + edge.commandArg + ')' : 'Exec');
    } else if (edge.kind === 'GameClear') {
      parts.push('GameClear');
    }
    if (edge.condition) {
      parts.push(edge.condition);
    } else if ((edge.kind === 'Continue' || edge.kind === 'NextStage') && edge.label && edge.label !== '全滅 敵') {
      parts.push(edge.label);
    }
    return parts.join(' / ');
  }

  function edgeClass(kind, missing) {
    if (missing) return 'missing';
    switch (kind) {
      case 'Exec': return 'exec';
      case 'IntermissionCommand':
      case 'IntermissionReturn': return 'intermission';
      case 'Load': return 'load';
      case 'GameClear': return 'clear';
      default: return 'continue';
    }
  }

  function nodeClass(node) {
    const cls = [];
    if (node.entry) cls.push('entry');
    switch (node.kind) {
      case 'Intermission': cls.push('intermission'); break;
      case 'LoadData': cls.push('load'); break;
      case 'SubStage': cls.push('substage'); break;
      case 'Include': cls.push('include'); break;
      case 'GameClear': cls.push('clear'); break;
      case 'Missing': cls.push('missing'); break;
      default: cls.push('stage'); break;
    }
    return cls.join(' ');
  }

  function layoutAndRender(data) {
    canvasWrap.innerHTML = '';
    const nodes = data.nodes || [];
    const edges = data.edges || [];
    if (nodes.length === 0) {
      canvasWrap.innerHTML = '<div class="empty">表示できるイベントファイルがありません。</div>';
      return;
    }

    const byId = new Map(nodes.map((n) => [n.id, n]));
    const isAux = (n) => n.kind === 'Intermission' || n.kind === 'LoadData';
    const mainNodes = nodes.filter((n) => !isAux(n));
    const auxNodes = nodes.filter((n) => isAux(n));

    // メインフローのトポロジカル階層を決定
    const mainIds = new Set(mainNodes.map((n) => n.id));
    const outMain = new Map(mainNodes.map((n) => [n.id, []]));
    const inDeg = new Map(mainNodes.map((n) => [n.id, 0]));
    for (const e of edges) {
      if (e.from !== e.to && mainIds.has(e.from) && mainIds.has(e.to)) {
        outMain.get(e.from).push(e.to);
        inDeg.set(e.to, (inDeg.get(e.to) || 0) + 1);
      }
    }

    const layer = new Map();
    const queue = [];
    for (const n of mainNodes) {
      if ((inDeg.get(n.id) || 0) === 0) {
        layer.set(n.id, 0);
        queue.push(n.id);
      }
    }
    const visited = new Set();
    let head = 0;
    while (head < queue.length) {
      const u = queue[head++];
      visited.add(u);
      const nextLayer = (layer.get(u) || 0) + 1;
      for (const v of outMain.get(u) || []) {
        if ((layer.get(v) ?? -1) < nextLayer) {
          layer.set(v, nextLayer);
        }
        inDeg.set(v, (inDeg.get(v) || 0) - 1);
        if ((inDeg.get(v) || 0) <= 0 && !visited.has(v)) {
          queue.push(v);
        }
      }
    }
    for (const n of mainNodes) {
      if (!layer.has(n.id)) layer.set(n.id, 0);
    }

    const layers = new Map();
    let maxLayer = 0;
    for (const n of mainNodes) {
      const l = layer.get(n.id) || 0;
      if (l > maxLayer) maxLayer = l;
      if (!layers.has(l)) layers.set(l, []);
      layers.get(l).push(n);
    }

    const NODE_W = 200;
    // 加入・離脱（7474/SRC#1692）を 3 行目に出すときは高くする
    const castOf = (n) => [...(n.joins || []).map((x) => '＋' + x), ...(n.leaves || []).map((x) => '－' + x)].join(' ');
    const NODE_H = nodes.some((n) => castOf(n)) ? 70 : 52;
    const GAP_X = 48;
    const GAP_Y = 64;
    const PAD = 32;

    let maxCols = 1;
    for (const list of layers.values()) {
      if (list.length > maxCols) maxCols = list.length;
    }
    const mainAreaW = Math.max(NODE_W, maxCols * NODE_W + (maxCols - 1) * GAP_X);

    const pos = new Map();
    for (let l = 0; l <= maxLayer; l++) {
      const list = layers.get(l) || [];
      const rowW = list.length * NODE_W + Math.max(0, list.length - 1) * GAP_X;
      const startX = PAD + (mainAreaW - rowW) / 2;
      const y = PAD + l * (NODE_H + GAP_Y);
      list.forEach((n, idx) => {
        pos.set(n.id, { x: startX + idx * (NODE_W + GAP_X), y, w: NODE_W, h: NODE_H });
      });
    }

    // 補助ノード（インターミッション・Load）を右レーンに配置
    const AUX_W = 190;
    const AUX_H = 48;
    if (auxNodes.length > 0) {
      const auxX = PAD + mainAreaW + 76;
      const auxWanted = auxNodes.map((n) => {
        const srcs = edges.filter((e) => e.to === n.id && pos.has(e.from)).map((e) => pos.get(e.from).y);
        const avgY = srcs.length > 0 ? srcs.reduce((a, b) => a + b, 0) / srcs.length : PAD;
        return { n, avgY };
      });
      auxWanted.sort((a, b) => a.avgY - b.avgY || a.n.path.localeCompare(b.n.path));
      let curY = PAD;
      for (const item of auxWanted) {
        const y = Math.max(curY, item.avgY);
        pos.set(item.n.id, { x: auxX, y, w: AUX_W, h: AUX_H });
        curY = y + AUX_H + 22;
      }
    }

    let totalW = PAD * 2 + mainAreaW + (auxNodes.length > 0 ? 76 + AUX_W : 0);
    let totalH = PAD * 2 + (maxLayer + 1) * NODE_H + maxLayer * GAP_Y;
    for (const p of pos.values()) {
      totalW = Math.max(totalW, p.x + p.w + PAD);
      totalH = Math.max(totalH, p.y + p.h + PAD);
    }

    const svg = svgEl('svg', {
      class: 'flow-svg',
      width: totalW,
      height: totalH,
      viewBox: '0 0 ' + totalW + ' ' + totalH,
    });

    const defs = svgEl('defs');
    const marker = svgEl('marker', {
      id: 'arrow',
      viewBox: '0 0 10 10',
      refX: 9,
      refY: 5,
      markerWidth: 6,
      markerHeight: 6,
      orient: 'auto-start-reverse',
    });
    marker.appendChild(svgEl('path', { d: 'M 0 1 L 10 5 L 0 9 z', fill: 'currentColor' }));
    defs.appendChild(marker);
    svg.appendChild(defs);

    // エッジを先に描画
    for (const edge of edges) {
      const p1 = pos.get(edge.from);
      const p2 = pos.get(edge.to);
      if (!p1 || !p2) continue;

      const g = svgEl('g', { class: 'edge-group ' + edgeClass(edge.kind, edge.missing) });
      let d;
      let labelX;
      let labelY;

      if (edge.from === edge.to) {
        // IntermissionReturn 等の自己ループ
        const rx = p1.x + p1.w;
        const ry = p1.y + p1.h / 2;
        d = 'M ' + rx + ' ' + (ry - 10) + ' C ' + (rx + 40) + ' ' + (ry - 24) + ', ' + (rx + 40) + ' ' + (ry + 24) + ', ' + rx + ' ' + (ry + 10);
        labelX = rx + 42;
        labelY = ry;
      } else if (isAux(byId.get(edge.to))) {
        const x1 = p1.x + p1.w;
        const y1 = p1.y + p1.h / 2;
        const x2 = p2.x;
        const y2 = p2.y + p2.h / 2;
        const mx = (x1 + x2) / 2;
        d = 'M ' + x1 + ' ' + y1 + ' C ' + mx + ' ' + y1 + ', ' + mx + ' ' + y2 + ', ' + x2 + ' ' + y2;
        labelX = mx;
        labelY = (y1 + y2) / 2;
      } else {
        const x1 = p1.x + p1.w / 2;
        const y1 = p1.y + p1.h;
        const x2 = p2.x + p2.w / 2;
        const y2 = p2.y;
        const dy = Math.max(24, Math.abs(y2 - y1) * 0.45);
        d = 'M ' + x1 + ' ' + y1 + ' C ' + x1 + ' ' + (y1 + dy) + ', ' + x2 + ' ' + (y2 - dy) + ', ' + x2 + ' ' + y2;
        labelX = (x1 + x2) / 2;
        labelY = (y1 + y2) / 2;
      }

      const pathEl = svgEl('path', { class: 'edge-path', d, 'marker-end': 'url(#arrow)' });
      if (edge.fullFile) {
        pathEl.appendChild(svgEl('title', null, edge.file + ':' + edge.line + ' (' + edge.command + ')'));
        g.addEventListener('click', () => {
          vscode.postMessage({ type: 'openFile', path: edge.fullFile, line: edge.line });
        });
      }
      g.appendChild(pathEl);

      const labelText = truncate(formatEdgeLabel(edge), 24);
      if (labelText) {
        const w = Math.max(44, labelText.length * 8.5 + 14);
        g.appendChild(svgEl('rect', { class: 'edge-label-bg', x: labelX - w / 2, y: labelY - 10, width: w, height: 20 }));
        g.appendChild(svgEl('text', { class: 'edge-label', x: labelX, y: labelY }, labelText));
      }
      svg.appendChild(g);
    }

    // ノードを描画
    for (const node of nodes) {
      const p = pos.get(node.id);
      if (!p) continue;
      const g = svgEl('g', { class: 'node-group ' + nodeClass(node) });
      const radius = node.kind === 'Intermission' || node.kind === 'GameClear' ? 22 : 6;
      const rect = svgEl('rect', {
        class: 'node-box',
        x: p.x,
        y: p.y,
        width: p.w,
        height: p.h,
        rx: radius,
        ry: radius,
      });
      const cast = castOf(node);
      rect.appendChild(svgEl('title', null, (node.fullPath || node.path)
        + ((node.joins || []).length ? '\\n加入: ' + node.joins.join('、') : '')
        + ((node.leaves || []).length ? '\\n離脱: ' + node.leaves.join('、') : '')));
      g.appendChild(rect);

      const mainTitle = truncate(node.kind === 'GameClear' ? 'クリア (GameClear)' : (node.kind === 'LoadData' ? 'Load: ' + node.title : node.title), 22);
      const subTitle = node.kind === 'Intermission'
        ? 'インターミッション'
        : node.kind === 'Missing'
          ? '見つかりません'
          : node.kind === 'GameClear'
            ? ''
            : truncate(node.path, 26);

      if (cast) {
        g.appendChild(svgEl('text', { class: 'node-title', x: p.x + p.w / 2, y: p.y + p.h * 0.27 }, mainTitle));
        g.appendChild(svgEl('text', { class: 'node-sub', x: p.x + p.w / 2, y: p.y + p.h * 0.52 }, subTitle));
        g.appendChild(svgEl('text', { class: 'node-cast', x: p.x + p.w / 2, y: p.y + p.h * 0.8 }, truncate(cast, 24)));
      } else {
        g.appendChild(svgEl('text', { class: 'node-title', x: p.x + p.w / 2, y: p.y + (subTitle ? p.h * 0.38 : p.h * 0.5) }, mainTitle));
        if (subTitle) {
          g.appendChild(svgEl('text', { class: 'node-sub', x: p.x + p.w / 2, y: p.y + p.h * 0.7 }, subTitle));
        }
      }

      if (node.fullPath) {
        g.addEventListener('click', () => {
          vscode.postMessage({ type: 'openFile', path: node.fullPath, line: node.line || 1 });
        });
      }
      svg.appendChild(g);
    }

    canvasWrap.appendChild(svg);
  }

  function renderTable(data) {
    tableWrap.innerHTML = '';
    const edges = data.edges || [];
    if (edges.length === 0) return;

    const byId = new Map((data.nodes || []).map((n) => [n.id, n]));
    const table = document.createElement('table');
    table.className = 'edge-table';
    table.innerHTML = '<thead><tr><th>元</th><th>行</th><th>コマンド</th><th>条件</th><th>先</th></tr></thead>';
    const tbody = document.createElement('tbody');
    for (const e of edges) {
      const tr = document.createElement('tr');
      if (e.fullFile) {
        tr.className = 'clickable';
        tr.addEventListener('click', () => vscode.postMessage({ type: 'openFile', path: e.fullFile, line: e.line }));
      }
      const fromNode = byId.get(e.from);
      const toNode = byId.get(e.to);
      const cond = [e.label, e.condition].filter(Boolean).join(' / ');
      const toName = e.kind === 'IntermissionReturn'
        ? 'インターミッションへ戻る'
        : (toNode ? (toNode.kind === 'GameClear' ? 'GameClear' : toNode.path) : (e.target || ''));
      const cells = [
        fromNode ? fromNode.path : e.file,
        String(e.line),
        e.command + (e.commandArg ? ' (' + e.commandArg + ')' : ''),
        cond,
        toName,
      ];
      for (const c of cells) {
        const td = document.createElement('td');
        td.textContent = c;
        tr.appendChild(td);
      }
      tbody.appendChild(tr);
    }
    table.appendChild(tbody);
    tableWrap.appendChild(table);
  }

  window.addEventListener('message', (event) => {
    const msg = event.data;
    if (!msg || msg.type !== 'flow') return;
    if (msg.error) {
      errorEl.textContent = msg.error;
      errorEl.style.display = 'block';
    } else {
      errorEl.style.display = 'none';
    }
    const data = msg.data;
    if (!data) {
      statsEl.textContent = '表示できませんでした';
      return;
    }
    const nodes = data.nodes || [];
    const edges = data.edges || [];
    const warnings = data.warnings || [];
    titleEl.textContent = '話のつながり: ' + (data.scenarioTitle || '');
    statsEl.textContent = nodes.length + ' ノード・' + edges.length + ' 遷移・' + warnings.length + ' 警告';

    if (warnings.length > 0) {
      warningsEl.innerHTML = '<h3>見つからない先（警告）</h3>';
      for (const w of warnings) {
        const div = document.createElement('div');
        div.className = 'warning-item';
        div.textContent = w.file + ':' + w.line + ' — ' + w.message;
        if (w.fullFile) {
          div.addEventListener('click', () => vscode.postMessage({ type: 'openFile', path: w.fullFile, line: w.line }));
        }
        warningsEl.appendChild(div);
      }
      warningsEl.style.display = 'block';
    } else {
      warningsEl.style.display = 'none';
    }

    layoutAndRender(data);
    renderTable(data);
    vscode.postMessage({ type: 'rendered', nodes: nodes.length, edges: edges.length, warnings: warnings.length });
  });

  vscode.postMessage({ type: 'ready' });
})();
`;
