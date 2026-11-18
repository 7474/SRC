import { spawn } from 'child_process';
import * as path from 'path';
import * as vscode from 'vscode';
import type { Controller } from './extension';
import { resolveServer, serverCommand } from './server';
import { webviewHtml } from './webview';

export interface EventFlowNode {
  id: string;
  name: string;
  kind: 'Entry' | 'Trigger' | 'Command' | 'Subroutine' | 'RequireFile' | 'System' | 'Dynamic' | 'Missing';
  triggerType?: string;
  file: string;
  fullFile?: string;
  line: number;
  exists: boolean;
  included?: boolean;
  required?: boolean;
  incoming?: number;
  outgoing?: number;
}

export interface EventFlowEdge {
  from: string;
  to: string;
  kind: 'Call' | 'Goto' | 'Require';
  file: string;
  fullFile?: string;
  line: number;
  label?: string;
  condition?: string;
  command: string;
  target?: string;
  args?: string;
  dynamic?: boolean;
  missing?: boolean;
}

export interface EventFlowWarning {
  file: string;
  fullFile?: string;
  line: number;
  target?: string;
  message: string;
}

export interface EventFlowGraphData {
  file: string;
  fullFile?: string;
  scenarioRoot?: string;
  appPath?: string;
  nodes: EventFlowNode[];
  edges: EventFlowEdge[];
  warnings: EventFlowWarning[];
  mermaid: string;
}

export interface EventFlowStatus {
  file?: string;
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
 * 1 つのイベントファイル（と include・Require 先）の流れの図パネル（7474/SRC#1655）。
 * LSP（srcsharp/eventFlow）で未保存の変更も含めて解析結果を取得し（LSP 停止時は CLI へフォールバック）、
 * 発火イベント・ラベル・Call・Goto・Require・If/Switch 条件を SVG で表示する。
 */
export class EventFlowPanel implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private ready = false;
  private targetFile: string | undefined;
  private data: EventFlowGraphData | undefined;
  private lastError: string | undefined;
  private rendered = false;
  private timer: NodeJS.Timeout | undefined;
  private readonly disposables: vscode.Disposable[] = [];

  constructor(private readonly controller: Controller) {
    this.disposables.push(
      vscode.workspace.onDidSaveTextDocument((doc) => {
        if (!this.panel || !this.targetFile) {
          return;
        }
        if (doc.languageId === 'src-eve' || doc.fileName.toLowerCase().endsWith('.eve')) {
          this.scheduleRefresh(250);
        }
      }),
      vscode.workspace.onDidChangeTextDocument((e) => {
        if (!this.panel || !this.targetFile) {
          return;
        }
        if (e.document.languageId === 'src-eve' || e.document.fileName.toLowerCase().endsWith('.eve')) {
          this.scheduleRefresh(500);
        }
      }),
    );
  }

  public getStatus(): EventFlowStatus {
    return {
      file: this.targetFile,
      nodes: this.data?.nodes.length ?? 0,
      edges: this.data?.edges.length ?? 0,
      warnings: this.data?.warnings.length ?? 0,
      rendered: this.rendered,
      ...(this.lastError ? { error: this.lastError } : {}),
    };
  }

  public async show(explicitFile?: string): Promise<void> {
    const resolved = explicitFile ?? this.detectEventFile();
    if (!resolved) {
      void vscode.window.showErrorMessage('SRC#: イベントの流れを調べるイベントファイル（*.eve）を開いてください。');
      return;
    }
    this.targetFile = path.resolve(resolved);
    this.rendered = false;
    this.lastError = undefined;
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.eventFlow',
        `SRC# イベントの流れ: ${path.basename(this.targetFile)}`,
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# イベントの流れ', STYLE, BODY, SCRIPT);
    } else {
      this.panel.title = `SRC# イベントの流れ: ${path.basename(this.targetFile)}`;
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    await this.refresh();
  }

  /** テストや外部から JSON データを直接渡して表示する */
  public async showData(flowData: EventFlowGraphData): Promise<void> {
    this.targetFile = flowData.fullFile || flowData.file;
    this.data = flowData;
    this.lastError = undefined;
    this.rendered = false;
    const title = flowData.file ? `SRC# イベントの流れ: ${path.basename(flowData.file)}` : 'SRC# イベントの流れ';
    if (!this.panel) {
      this.panel = vscode.window.createWebviewPanel(
        'srcsharp.eventFlow',
        title,
        vscode.ViewColumn.Beside,
        { enableScripts: true, retainContextWhenHidden: true },
      );
      this.panel.onDidDispose(() => {
        this.panel = undefined;
        this.ready = false;
        this.rendered = false;
      });
      this.panel.webview.onDidReceiveMessage((msg: WebviewMessage) => void this.receive(msg));
      this.panel.webview.html = webviewHtml(this.panel.webview, 'SRC# イベントの流れ', STYLE, BODY, SCRIPT);
    } else {
      this.panel.title = title;
      this.panel.reveal(vscode.ViewColumn.Beside, false);
    }
    this.postState();
  }

  public async refresh(): Promise<void> {
    if (!this.targetFile) {
      return;
    }
    try {
      let result: EventFlowGraphData | undefined;
      if (this.controller.clientState === 'running') {
        try {
          result = await this.controller.sendRequest<EventFlowGraphData>('srcsharp/eventFlow', {
            textDocument: { uri: vscode.Uri.file(this.targetFile).toString() },
          });
        } catch {
          result = undefined;
        }
      }
      if (!result) {
        result = await this.runCli(this.targetFile);
      }
      this.data = result;
      this.lastError = undefined;
      if (this.panel && result.file) {
        this.panel.title = `SRC# イベントの流れ: ${path.basename(result.file)}`;
      }
    } catch (e) {
      this.lastError = (e as Error)?.message ?? String(e);
    }
    this.postState();
  }

  private async runCli(targetFile: string): Promise<EventFlowGraphData> {
    const server = resolveServer();
    if (!server.found) {
      throw new Error('srcsharp が見つかりません。');
    }
    const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath');
    const args = [targetFile, '--format', 'json', ...(appPath ? ['--app-path', appPath] : [])];
    const cmd = serverCommand(server, 'event-flow', args);

    const jsonText = await new Promise<string>((resolve, reject) => {
      const child = spawn(cmd.command, cmd.args, { cwd: path.dirname(targetFile), env: server.env });
      let stdout = '';
      let stderr = '';
      child.stdout.on('data', (d: Buffer) => { stdout += d.toString(); });
      child.stderr.on('data', (d: Buffer) => { stderr += d.toString(); });
      child.on('error', reject);
      child.on('close', (code) => {
        if (code === 0) {
          resolve(stdout);
        } else {
          reject(new Error(stderr.trim() || `srcsharp event-flow が終了コード ${code} で失敗しました`));
        }
      });
    });
    return JSON.parse(jsonText) as EventFlowGraphData;
  }

  private detectEventFile(): string | undefined {
    const editor = vscode.window.activeTextEditor;
    if (editor && editor.document.uri.scheme === 'file'
      && (editor.document.languageId === 'src-eve' || editor.document.fileName.toLowerCase().endsWith('.eve'))) {
      return editor.document.uri.fsPath;
    }
    return undefined;
  }

  private scheduleRefresh(delay = 350): void {
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
      type: 'eventFlow',
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
          void vscode.window.showInformationMessage('SRC#: イベントの流れの Mermaid 図をクリップボードにコピーしました。');
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

function buildMarkdown(data: EventFlowGraphData): string {
  const lines = [
    `# イベントの流れ（${data.file || 'イベント'}）`,
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
body { margin: 0; padding: 12px 16px; display: flex; flex-direction: column; gap: 10px; }
.toolbar { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; border-bottom: 1px solid var(--vscode-panel-border); padding-bottom: 10px; }
.title-area h2 { margin: 0; font-size: 1.15em; }
.title-area .stats { font-size: 0.9em; color: var(--vscode-descriptionForeground); margin-top: 2px; }
.actions { display: flex; gap: 6px; flex-wrap: wrap; }
.controls { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 8px; font-size: 0.85em; }
.filters { display: flex; flex-wrap: wrap; gap: 10px; }
.filters label { display: inline-flex; align-items: center; gap: 4px; cursor: pointer; }
.legend { display: flex; flex-wrap: wrap; gap: 12px; color: var(--vscode-descriptionForeground); }
.legend-item { display: inline-flex; align-items: center; gap: 5px; }
.swatch { width: 12px; height: 12px; border-radius: 3px; border: 1.5px solid; display: inline-block; background: var(--vscode-editorWidget-background); }
.swatch.entry { border-color: var(--vscode-charts-green, #2da44e); border-radius: 99px; border-width: 2.2px; }
.swatch.trigger { border-color: var(--vscode-charts-yellow, #d29922); border-width: 2px; }
.swatch.command { border-color: var(--vscode-charts-purple, #8957e5); border-width: 2px; }
.swatch.subroutine { border-color: var(--vscode-textLink-foreground); }
.swatch.require { border-color: var(--vscode-charts-blue, #3794ff); }
.swatch.system { border-color: var(--vscode-descriptionForeground); border-style: dashed; border-radius: 99px; }
.swatch.missing { border-color: var(--vscode-editorError-foreground); border-style: dashed; }
.warnings { border: 1px solid var(--vscode-editorWarning-foreground); background: color-mix(in srgb, var(--vscode-editorWarning-foreground) 10%, transparent); border-radius: 4px; padding: 8px 12px; }
.warnings h3 { margin: 0 0 6px; font-size: 0.95em; color: var(--vscode-editorWarning-foreground); }
.warning-item { cursor: pointer; padding: 2px 0; text-decoration: underline; font-size: 0.9em; }
.warning-item:hover { color: var(--vscode-textLink-activeForeground); }
.canvas-wrap { border: 1px solid var(--vscode-panel-border); border-radius: 6px; overflow: auto; background: var(--vscode-editor-background); min-height: 220px; padding: 12px; }
svg.flow-svg { display: block; min-width: 100%; }
.node-group { cursor: pointer; }
.node-box { fill: var(--vscode-editorWidget-background); stroke: var(--vscode-textLink-foreground); stroke-width: 1.8; }
.node-group.entry .node-box { stroke: var(--vscode-charts-green, #2da44e); stroke-width: 2.5; }
.node-group.trigger .node-box { stroke: var(--vscode-charts-yellow, #d29922); stroke-width: 2.2; }
.node-group.command .node-box { stroke: var(--vscode-charts-purple, #8957e5); stroke-width: 2.2; }
.node-group.require .node-box { stroke: var(--vscode-charts-blue, #3794ff); }
.node-group.system .node-box, .node-group.dynamic .node-box { stroke: var(--vscode-descriptionForeground); stroke-dasharray: 4 3; }
.node-group.missing .node-box { stroke: var(--vscode-editorError-foreground); stroke-dasharray: 5 4; }
.node-group:hover .node-box { filter: brightness(1.15); stroke-width: 2.8; }
.node-title { fill: var(--vscode-foreground); font-size: 12.5px; font-weight: 600; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
.node-sub { fill: var(--vscode-descriptionForeground); font-size: 10.5px; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
.edge-group { cursor: pointer; }
.edge-path { fill: none; stroke: var(--vscode-foreground); stroke-opacity: 0.65; stroke-width: 1.7; }
.edge-group.goto .edge-path { stroke-width: 2.4; stroke: var(--vscode-charts-orange, #d18616); }
.edge-group.require .edge-path { stroke-dasharray: 4 4; stroke: var(--vscode-charts-blue, #3794ff); }
.edge-group.missing .edge-path { stroke: var(--vscode-editorError-foreground); stroke-dasharray: 5 3; }
.edge-group:hover .edge-path { stroke-opacity: 1; stroke-width: 2.8; }
.edge-label-bg { fill: var(--vscode-editor-background); stroke: var(--vscode-panel-border); stroke-width: 1; rx: 4; opacity: 0.92; }
.edge-label { fill: var(--vscode-foreground); font-size: 10.5px; text-anchor: middle; dominant-baseline: middle; pointer-events: none; }
table.edge-table { width: 100%; border-collapse: collapse; font-size: 0.9em; }
table.edge-table th, table.edge-table td { border-bottom: 1px solid var(--vscode-panel-border); padding: 5px 8px; text-align: left; }
table.edge-table th { color: var(--vscode-descriptionForeground); font-weight: 600; }
table.edge-table tr.clickable { cursor: pointer; }
table.edge-table tr.clickable:hover { background: var(--vscode-list-hoverBackground); }
.empty { padding: 24px; text-align: center; color: var(--vscode-descriptionForeground); }
`;

const BODY = `
<div class="toolbar">
  <div class="title-area">
    <h2 id="title">イベントの流れ</h2>
    <div id="stats" class="stats">読み込み中…</div>
  </div>
  <div class="actions">
    <button id="btn-refresh" class="secondary">再読み込み</button>
    <button id="btn-copy" class="secondary">Mermaid をコピー</button>
    <button id="btn-md" class="secondary">Markdown で開く</button>
  </div>
</div>
<div class="controls">
  <div class="filters">
    <label><input type="checkbox" id="flt-call" checked> Call</label>
    <label><input type="checkbox" id="flt-goto" checked> Goto</label>
    <label><input type="checkbox" id="flt-require" checked> Require</label>
    <label><input type="checkbox" id="flt-system" checked> 共通サブルーチン</label>
  </div>
  <div class="legend">
    <span class="legend-item"><span class="swatch entry"></span>入口</span>
    <span class="legend-item"><span class="swatch trigger"></span>発火</span>
    <span class="legend-item"><span class="swatch command"></span>コマンド</span>
    <span class="legend-item"><span class="swatch subroutine"></span>サブルーチン</span>
    <span class="legend-item"><span class="swatch require"></span>Require</span>
    <span class="legend-item"><span class="swatch system"></span>共通</span>
    <span class="legend-item"><span class="swatch missing"></span>見つからない先</span>
  </div>
</div>
<div id="error" class="error" hidden></div>
<div id="warnings" class="warnings" hidden></div>
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
  const fltCall = document.getElementById('flt-call');
  const fltGoto = document.getElementById('flt-goto');
  const fltRequire = document.getElementById('flt-require');
  const fltSystem = document.getElementById('flt-system');

  let currentData = null;

  document.getElementById('btn-refresh').addEventListener('click', () => vscode.postMessage({ type: 'refresh' }));
  document.getElementById('btn-copy').addEventListener('click', () => vscode.postMessage({ type: 'copyMermaid' }));
  document.getElementById('btn-md').addEventListener('click', () => vscode.postMessage({ type: 'openMarkdown' }));
  for (const chk of [fltCall, fltGoto, fltRequire, fltSystem]) {
    chk.addEventListener('change', () => {
      if (currentData) renderAll(currentData);
    });
  }

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

  function nodeClass(node) {
    switch (node.kind) {
      case 'Entry': return 'entry';
      case 'Trigger': return 'trigger';
      case 'Command': return 'command';
      case 'RequireFile': return 'require';
      case 'System': return 'system';
      case 'Dynamic': return 'dynamic';
      case 'Missing': return 'missing';
      default: return 'subroutine';
    }
  }

  function edgeClass(edge) {
    if (edge.missing) return 'missing';
    if (edge.kind === 'Goto') return 'goto';
    if (edge.kind === 'Require') return 'require';
    return 'call';
  }

  function nodeSubtitle(node) {
    if (node.kind === 'System') return '共通サブルーチン';
    if (node.kind === 'Missing') return '見つかりません';
    if (node.kind === 'Dynamic') return '式で決まる先';
    if (node.kind === 'RequireFile') return node.file || 'Require';
    if (node.kind === 'Trigger' || node.kind === 'Command') {
      return (node.triggerType || node.kind) + (node.line ? ' (' + node.line + '行)' : '');
    }
    const prefix = node.included ? (node.file + ':') : '';
    return prefix + (node.line ? node.line + '行目' : '');
  }

  function formatEdgeLabel(edge) {
    const parts = [];
    if (edge.kind === 'Goto') parts.push('Goto');
    else if (edge.kind === 'Require') parts.push('Require');
    if (edge.args) parts.push(edge.args);
    if (edge.condition) parts.push(edge.condition);
    return parts.join(' / ');
  }

  function filteredGraph(data) {
    const showCall = fltCall.checked;
    const showGoto = fltGoto.checked;
    const showRequire = fltRequire.checked;
    const showSystem = fltSystem.checked;

    const allNodes = data.nodes || [];
    const nodeSet = new Set(
      allNodes
        .filter((n) => showSystem || n.kind !== 'System')
        .map((n) => n.id),
    );
    const edges = (data.edges || []).filter((e) => {
      if (!nodeSet.has(e.from) || !nodeSet.has(e.to)) return false;
      if (e.kind === 'Call' && !showCall) return false;
      if (e.kind === 'Goto' && !showGoto) return false;
      if (e.kind === 'Require' && !showRequire) return false;
      return true;
    });
    const nodes = allNodes.filter((n) => nodeSet.has(n.id));
    return { nodes, edges };
  }

  function layoutAndRender(nodes, edges) {
    canvasWrap.innerHTML = '';
    if (nodes.length === 0) {
      canvasWrap.innerHTML = '<div class="empty">表示できるイベントラベルや呼び出しがありません。</div>';
      return;
    }

    const byId = new Map(nodes.map((n) => [n.id, n]));
    const isAux = (n) => n.kind === 'RequireFile' || n.kind === 'System' || n.kind === 'Dynamic' || n.kind === 'Missing';
    const mainNodes = nodes.filter((n) => !isAux(n));
    const auxNodes = nodes.filter((n) => isAux(n));

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

    const NODE_W = 210;
    const NODE_H = 52;
    const GAP_X = 44;
    const GAP_Y = 62;
    const PAD = 32;

    let maxCols = 1;
    for (const list of layers.values()) {
      if (list.length > maxCols) maxCols = list.length;
    }
    const mainAreaW = mainNodes.length > 0
      ? Math.max(NODE_W, maxCols * NODE_W + (maxCols - 1) * GAP_X)
      : 0;

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

    const AUX_W = 200;
    const AUX_H = 48;
    if (auxNodes.length > 0) {
      const auxX = mainAreaW > 0 ? PAD + mainAreaW + 76 : PAD;
      const auxWanted = auxNodes.map((n) => {
        const srcs = edges.filter((e) => e.to === n.id && pos.has(e.from)).map((e) => pos.get(e.from).y);
        const avgY = srcs.length > 0 ? srcs.reduce((a, b) => a + b, 0) / srcs.length : PAD;
        return { n, avgY };
      });
      auxWanted.sort((a, b) => a.avgY - b.avgY || (a.n.name || '').localeCompare(b.n.name || ''));
      let curY = PAD;
      for (const item of auxWanted) {
        const y = Math.max(curY, item.avgY);
        pos.set(item.n.id, { x: auxX, y, w: AUX_W, h: AUX_H });
        curY = y + AUX_H + 20;
      }
    }

    let totalW = PAD * 2 + mainAreaW + (auxNodes.length > 0 ? (mainAreaW > 0 ? 76 : 0) + AUX_W : 0);
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

    for (const edge of edges) {
      const p1 = pos.get(edge.from);
      const p2 = pos.get(edge.to);
      if (!p1 || !p2) continue;

      const g = svgEl('g', { class: 'edge-group ' + edgeClass(edge) });
      let d;
      let labelX;
      let labelY;

      if (edge.from === edge.to) {
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
      pathEl.appendChild(svgEl('title', null, edge.file + ':' + edge.line + ' ' + edge.command + (edge.condition ? ' [' + edge.condition + ']' : '')));
      if (edge.fullFile) {
        g.addEventListener('click', () => vscode.postMessage({ type: 'openFile', path: edge.fullFile, line: edge.line }));
      }
      g.appendChild(pathEl);

      const labelText = truncate(formatEdgeLabel(edge), 26);
      if (labelText) {
        const w = Math.max(48, labelText.length * 8.5 + 14);
        g.appendChild(svgEl('rect', { class: 'edge-label-bg', x: labelX - w / 2, y: labelY - 10, width: w, height: 20 }));
        g.appendChild(svgEl('text', { class: 'edge-label', x: labelX, y: labelY }, labelText));
      }
      svg.appendChild(g);
    }

    for (const node of nodes) {
      const p = pos.get(node.id);
      if (!p) continue;
      const g = svgEl('g', { class: 'node-group ' + nodeClass(node) });
      const radius = node.kind === 'Entry' || node.kind === 'System' ? 22 : 6;
      const rect = svgEl('rect', {
        class: 'node-box',
        x: p.x,
        y: p.y,
        width: p.w,
        height: p.h,
        rx: radius,
        ry: radius,
      });
      rect.appendChild(svgEl('title', null, (node.fullFile || node.file || '') + (node.line ? ':' + node.line : '')));
      g.appendChild(rect);

      const mainTitle = truncate(node.name, 24);
      const subTitle = truncate(nodeSubtitle(node), 28);
      g.appendChild(svgEl('text', { class: 'node-title', x: p.x + p.w / 2, y: p.y + (subTitle ? p.h * 0.38 : p.h * 0.5) }, mainTitle));
      if (subTitle) {
        g.appendChild(svgEl('text', { class: 'node-sub', x: p.x + p.w / 2, y: p.y + p.h * 0.72 }, subTitle));
      }

      if (node.fullFile) {
        g.addEventListener('click', () => vscode.postMessage({ type: 'openFile', path: node.fullFile, line: node.line || 1 }));
      }
      svg.appendChild(g);
    }

    canvasWrap.appendChild(svg);
  }

  function renderTable(nodes, edges) {
    tableWrap.innerHTML = '';
    if (edges.length === 0) return;

    const byId = new Map(nodes.map((n) => [n.id, n]));
    const table = document.createElement('table');
    table.className = 'edge-table';
    table.innerHTML = '<thead><tr><th>元のラベル</th><th>行</th><th>コマンド</th><th>条件</th><th>先</th></tr></thead>';
    const tbody = document.createElement('tbody');
    for (const e of edges) {
      const tr = document.createElement('tr');
      if (e.fullFile) {
        tr.className = 'clickable';
        tr.addEventListener('click', () => vscode.postMessage({ type: 'openFile', path: e.fullFile, line: e.line }));
      }
      const fromNode = byId.get(e.from);
      const toNode = byId.get(e.to);
      const cmdText = e.command + (e.args ? ' (' + e.args + ')' : '');
      const toName = toNode ? toNode.name : (e.target || '');
      const cells = [
        fromNode ? fromNode.name : (e.label || ''),
        String(e.line),
        cmdText,
        e.condition || '',
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

  function renderAll(data) {
    const nodes = data.nodes || [];
    const edges = data.edges || [];
    const warnings = data.warnings || [];
    titleEl.textContent = 'イベントの流れ: ' + (data.file || '');
    statsEl.textContent = nodes.length + ' ノード・' + edges.length + ' 呼び出し／遷移・' + warnings.length + ' 警告';

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
      warningsEl.hidden = false;
    } else {
      warningsEl.hidden = true;
    }

    const filtered = filteredGraph(data);
    layoutAndRender(filtered.nodes, filtered.edges);
    renderTable(filtered.nodes, filtered.edges);
    vscode.postMessage({ type: 'rendered', nodes: nodes.length, edges: edges.length, warnings: warnings.length });
  }

  window.addEventListener('message', (event) => {
    const msg = event.data;
    if (!msg || msg.type !== 'eventFlow') return;
    if (msg.error) {
      errorEl.textContent = msg.error;
      errorEl.hidden = false;
    } else {
      errorEl.hidden = true;
    }
    const data = msg.data;
    if (!data) {
      statsEl.textContent = '表示できませんでした';
      return;
    }
    currentData = data;
    renderAll(data);
  });

  vscode.postMessage({ type: 'ready' });
})();
`;
