import * as vscode from 'vscode';
import { appPathCandidates, browseAppPath, isAppFolder } from './appPath';
import type { Controller } from './extension';
import { webviewHtml } from './webview';

/** 表示言語（SRCCore.UiLanguages.Supported と auto。docs/i18n.md） */
export const LANGUAGES: [string, string][] = [
  ['', '指定しない（既定は日本語）'],
  ['ja', '日本語'],
  ['en', 'English'],
  ['zh-Hans', '简体中文'],
  ['zh-Hant', '繁體中文'],
  ['ko', '한국어'],
  ['fr', 'Français'],
  ['de', 'Deutsch'],
  ['es', 'Español'],
  ['pt-BR', 'Português (Brasil)'],
  ['it', 'Italiano'],
  ['ru', 'Русский'],
  ['pl', 'Polski'],
  ['tr', 'Türkçe'],
  ['th', 'ไทย'],
  ['uk', 'Українська'],
  ['auto', 'OS の言語に従う'],
];

interface Values {
  serverPath: string;
  serverArgs: string[];
  appPath: string;
  language: string;
  screenAutoOpen: boolean;
}

type Message =
  | { type: 'ready' }
  | { type: 'save'; values: Values; target: 'workspace' | 'user' }
  | { type: 'browseServer' }
  | { type: 'browseAppPath' }
  | { type: 'install' }
  | { type: 'openJson' };

/** 設定画面（Webview）。srcsharp の状態を見ながら、設定をフォームで書く */
export class SettingsView implements vscode.Disposable {
  private panel: vscode.WebviewPanel | undefined;
  private readonly disposables: vscode.Disposable[] = [];

  constructor(private readonly controller: Controller) {
    // 状態だけ送る（編集中のフォームの値は消さない）
    this.disposables.push(controller.onDidChange(() => this.post({})));
  }

  show(): void {
    if (this.panel) {
      this.panel.reveal();
      return;
    }
    this.panel = vscode.window.createWebviewPanel('srcsharp.settings', 'SRC# 設定', vscode.ViewColumn.Active, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });
    this.panel.webview.html = this.html(this.panel.webview);
    this.panel.webview.onDidReceiveMessage((m: Message) => void this.receive(m));
    this.panel.onDidDispose(() => (this.panel = undefined));
  }

  private values(): Values {
    const config = vscode.workspace.getConfiguration('srcsharp');
    return {
      serverPath: config.get<string>('serverPath', 'srcsharp'),
      serverArgs: config.get<string[]>('serverArgs', []),
      appPath: config.get<string>('appPath', ''),
      language: config.get<string>('language', ''),
      screenAutoOpen: config.get<boolean>('screen.autoOpen', true),
    };
  }

  /** 状態と、fields が無ければ設定の値、あればフォームに入れる値を送る */
  private post(fields?: Partial<Values>): void {
    if (!this.panel) {
      return;
    }
    const c = this.controller;
    void this.panel.webview.postMessage({
      type: 'state',
      values: fields ? undefined : this.values(),
      fields,
      server: { found: c.server.found, command: c.server.command, source: c.server.source, version: c.version },
      candidates: appPathCandidates(),
      hasWorkspace: !!vscode.workspace.workspaceFolders?.length,
    });
  }

  private async receive(m: Message): Promise<void> {
    switch (m.type) {
      case 'ready':
        this.post();
        break;
      case 'save': {
        const target = m.target === 'workspace' ? vscode.ConfigurationTarget.Workspace : vscode.ConfigurationTarget.Global;
        const config = vscode.workspace.getConfiguration('srcsharp');
        const v = m.values;
        // 既定と同じ値は書かない（設定ファイルを汚さない）
        await config.update('serverPath', v.serverPath.trim() && v.serverPath.trim() !== 'srcsharp' ? v.serverPath.trim() : undefined, target);
        await config.update('serverArgs', v.serverArgs.length ? v.serverArgs : undefined, target);
        await config.update('appPath', v.appPath.trim() || undefined, target);
        await config.update('language', v.language || undefined, target);
        await config.update('screen.autoOpen', v.screenAutoOpen ? undefined : false, target);
        if (v.appPath.trim() && !isAppFolder(v.appPath.trim())) {
          vscode.window.showWarningMessage(`SRC 本体のフォルダ ${v.appPath} に Lib も Data/System もありません。`);
        }
        vscode.window.showInformationMessage(
          `SRC# の設定を${m.target === 'workspace' ? 'ワークスペース' : 'ユーザー'}の設定に保存しました。`,
        );
        break;
      }
      case 'browseServer': {
        const picked = await vscode.window.showOpenDialog({ canSelectFiles: true, canSelectMany: false, openLabel: 'srcsharp にする' });
        if (picked?.[0]) {
          this.post({ serverPath: picked[0].fsPath });
        }
        break;
      }
      case 'browseAppPath': {
        const dir = await browseAppPath();
        if (dir) {
          this.post({ appPath: dir });
        }
        break;
      }
      case 'install':
        await vscode.commands.executeCommand('srcsharp.installServer');
        break;
      case 'openJson':
        await vscode.commands.executeCommand('workbench.action.openSettings', '@ext:koudenpa.src-vscode');
        break;
    }
  }

  private html(webview: vscode.Webview): string {
    const languages = LANGUAGES.map(([v, l]) => `<option value="${v}">${l}${v ? ` (${v})` : ''}</option>`).join('');
    return webviewHtml(
      webview,
      'SRC# 設定',
      `
h1 { font-size: 1.4em; font-weight: 600; margin: 0 0 12px; }
h2 { font-size: 1.1em; font-weight: 600; margin: 20px 0 6px; }
section { max-width: 760px; }
.row { display: flex; gap: 6px; align-items: center; }
.row input[type=text], .row select { flex: 1; min-width: 0; }
textarea { width: 100%; min-height: 4em; }
.help { margin: 4px 0 0; }
.status { margin: 6px 0; }
.actions { margin-top: 24px; display: flex; gap: 8px; align-items: center; flex-wrap: wrap; }
label.check { display: flex; gap: 6px; align-items: center; }
`,
      `
<section>
<h1>SRC# 設定</h1>

<h2>srcsharp</h2>
<div class="status" id="serverStatus"></div>
<div class="row">
  <input type="text" id="serverPath" placeholder="srcsharp" aria-label="srcsharp の場所">
  <button class="secondary" id="browseServer">参照…</button>
  <button id="install">入れる／更新する</button>
</div>
<p class="help muted">既定（srcsharp）なら PATH と ~/.dotnet/tools から探します。リポジトリでビルドした dll を動かす場合は dotnet を指定し、下の引数に dll のパスを書きます。</p>

<h2>srcsharp の引数</h2>
<textarea id="serverArgs" placeholder="1 行に 1 つ（例: path/to/srcsharp.dll）" aria-label="srcsharp の引数"></textarea>
<p class="help muted">lsp・dap などのコマンドの前に付けます。</p>

<h2>SRC 本体のフォルダ</h2>
<div class="row">
  <input type="text" id="appPath" list="candidates" placeholder="空ならシナリオのフォルダの親から探す" aria-label="SRC 本体のフォルダ">
  <datalist id="candidates"></datalist>
  <button class="secondary" id="browseAppPath">参照…</button>
</div>
<p class="help muted" id="candidateHelp">Lib や共通の Data のあるフォルダ。システムのラベル（汎用戦闘アニメなど）をここから読みます。</p>

<h2>表示言語</h2>
<div class="row"><select id="language" aria-label="表示言語">${languages}</select></div>
<p class="help muted">lint・バランスレポート・自動プレイの出力の言語（環境変数 SRCSHARP_LANG）。</p>

<h2>画面</h2>
<label class="check"><input type="checkbox" id="screenAutoOpen"> デバッグを始めたら画面のビューを開く</label>

<div class="actions">
  <label>保存先
    <select id="target">
      <option value="workspace">ワークスペース</option>
      <option value="user">ユーザー</option>
    </select>
  </label>
  <button id="save">保存</button>
  <button class="secondary" id="openJson">VS Code の設定で開く</button>
</div>
<p class="help muted">保存すると Language Server を起動し直します。</p>
</section>
`,
      `
const $ = (id) => document.getElementById(id);
const sources = { setting: '設定の場所', path: 'PATH', 'dotnet-tools': '~/.dotnet/tools' };
function setValues(v) {
  if (v.serverPath !== undefined) $('serverPath').value = v.serverPath === 'srcsharp' ? '' : v.serverPath;
  if (v.serverArgs !== undefined) $('serverArgs').value = v.serverArgs.join('\\n');
  if (v.appPath !== undefined) $('appPath').value = v.appPath;
  if (v.language !== undefined) $('language').value = v.language;
  if (v.screenAutoOpen !== undefined) $('screenAutoOpen').checked = v.screenAutoOpen;
}
window.addEventListener('message', (e) => {
  const m = e.data;
  if (m.type !== 'state') return;
  if (m.values) setValues(m.values);
  if (m.fields) setValues(m.fields);
  const s = m.server;
  const status = $('serverStatus');
  status.textContent = '';
  const mark = document.createElement('span');
  if (s.found) {
    mark.className = 'ok';
    mark.textContent = '✓ ' + (s.version ? 'srcsharp ' + s.version : 'srcsharp（版は不明。0.5.1 以前）');
    status.append(mark, ' ', Object.assign(document.createElement('span'), { className: 'muted', textContent: s.command + '（' + (sources[s.source] || '') + '）' }));
    $('install').textContent = '更新する';
  } else {
    mark.className = 'error';
    mark.textContent = '✗ srcsharp が見つかりません。「入れる」で dotnet tool として入れます（.NET 10 SDK が要ります）。';
    status.append(mark);
    $('install').textContent = '入れる';
  }
  const list = $('candidates');
  list.textContent = '';
  for (const c of m.candidates) list.append(Object.assign(document.createElement('option'), { value: c }));
  $('candidateHelp').textContent = 'Lib や共通の Data のあるフォルダ。システムのラベル（汎用戦闘アニメなど）をここから読みます。'
    + (m.candidates.length ? ' ワークスペースで見つかったもの: ' + m.candidates.join('、') : '');
  if (!m.hasWorkspace) {
    $('target').value = 'user';
    $('target').disabled = true;
  }
});
$('browseServer').addEventListener('click', () => vscode.postMessage({ type: 'browseServer' }));
$('browseAppPath').addEventListener('click', () => vscode.postMessage({ type: 'browseAppPath' }));
$('install').addEventListener('click', () => vscode.postMessage({ type: 'install' }));
$('openJson').addEventListener('click', () => vscode.postMessage({ type: 'openJson' }));
$('save').addEventListener('click', () => vscode.postMessage({
  type: 'save',
  target: $('target').value,
  values: {
    serverPath: $('serverPath').value,
    serverArgs: $('serverArgs').value.split(/\\r?\\n/).map((x) => x.trim()).filter((x) => x),
    appPath: $('appPath').value,
    language: $('language').value,
    screenAutoOpen: $('screenAutoOpen').checked,
  },
}));
vscode.postMessage({ type: 'ready' });
`,
    );
  }

  dispose(): void {
    this.panel?.dispose();
    this.disposables.forEach((d) => d.dispose());
  }
}
