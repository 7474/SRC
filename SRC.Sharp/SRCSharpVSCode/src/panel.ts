import * as vscode from 'vscode';
import { appPathInferable } from './appPath';
import type { Controller } from './extension';

type Node = { kind: 'group'; label: string; children: () => vscode.TreeItem[] } | { kind: 'item'; item: vscode.TreeItem };

function item(label: string, icon: string, command: string, description?: string, tooltip?: string): vscode.TreeItem {
  const t = new vscode.TreeItem(label, vscode.TreeItemCollapsibleState.None);
  t.iconPath = new vscode.ThemeIcon(icon);
  t.command = { command, title: label };
  t.description = description;
  t.tooltip = tooltip;
  return t;
}

/**
 * 操作パネル（アクティビティバーの SRC#）。状態（srcsharp・Language Server・SRC 本体のフォルダ）と操作を並べる。
 * srcsharp が見つからない時は空にし、viewsWelcome の導入の案内を出す
 */
export class PanelProvider implements vscode.TreeDataProvider<Node> {
  private readonly changed = new vscode.EventEmitter<void>();
  readonly onDidChangeTreeData = this.changed.event;

  constructor(private readonly controller: Controller) {
    controller.onDidChange(() => this.changed.fire());
  }

  getTreeItem(node: Node): vscode.TreeItem {
    if (node.kind === 'item') {
      return node.item;
    }
    return new vscode.TreeItem(node.label, vscode.TreeItemCollapsibleState.Expanded);
  }

  getChildren(node?: Node): Node[] {
    if (node) {
      return node.kind === 'group' ? node.children().map((i) => ({ kind: 'item', item: i })) : [];
    }
    if (!this.controller.server.found) {
      return [];
    }
    return [
      { kind: 'group', label: '状態', children: () => this.status() },
      { kind: 'group', label: '操作', children: () => this.actions() },
    ];
  }

  private status(): vscode.TreeItem[] {
    const c = this.controller;
    const server = item(
      'srcsharp',
      'check',
      'srcsharp.openSettings',
      c.version ?? '版は不明（0.5.1 以前）',
      `${c.server.command}\n${{ setting: '設定の serverPath', path: 'PATH', 'dotnet-tools': '~/.dotnet/tools', missing: '' }[c.server.source]}から見つけました`,
    );
    const lsp = item(
      'Language Server',
      { running: 'pass', starting: 'loading~spin', stopped: 'circle-slash', error: 'error' }[c.clientState],
      'srcsharp.restartServer',
      { running: '動作中', starting: '起動中', stopped: '停止', error: '起動できませんでした' }[c.clientState],
      'クリックで再起動',
    );
    const appPath = vscode.workspace.getConfiguration('srcsharp').get<string>('appPath', '');
    const app = item(
      'SRC 本体',
      appPath || appPathInferable() ? 'folder' : 'warning',
      'srcsharp.pickAppPath',
      appPath || (appPathInferable() ? 'シナリオのフォルダの親から探す' : '未設定'),
      appPath ? `${appPath}\nクリックで変更` : 'Lib や共通の Data のあるフォルダ。クリックで選ぶ',
    );
    return [server, lsp, app];
  }

  private actions(): vscode.TreeItem[] {
    return [
      item('開いているイベントを実行（画面付き）', 'play', 'srcsharp.runFile'),
      item('開いているイベントをデバッグ（最初で止める）', 'debug-alt', 'srcsharp.debugFile'),
      item('画面を表示', 'device-desktop', 'srcsharp.showScreen'),
      item('開いているイベントを自動プレイ（出力のみ）', 'run-all', 'srcsharp.play'),
      item('ワークスペースを lint', 'checklist', 'srcsharp.lint'),
      item('バランスレポートを作る', 'graph', 'srcsharp.balance'),
      item('設定', 'gear', 'srcsharp.openSettings'),
      item('srcsharp を入れる／更新する', 'cloud-download', 'srcsharp.installServer'),
    ];
  }
}
