// 拡張のテスト（#1364）。npm test で VS Code を落としてきて、test/fixture を開いた VS Code の中で test/*.test.js を動かす。
// 先に npm run compile（または package）で out/extension.js を作っておく。Linux の CI では xvfb-run の下で動かす
import { defineConfig } from '@vscode/test-cli';

export default defineConfig({
  files: 'test/**/*.test.js',
  version: 'stable',
  workspaceFolder: 'test/fixture',
  mocha: { ui: 'bdd', timeout: 60 * 1000 },
  // ほかの拡張と、ユーザーの設定に影響されないようにする
  launchArgs: ['--disable-extensions'],
});
