// @ts-check
const { defineConfig } = require('@playwright/test');

const port = 5280;
// GitHub Pages と同じくサブパス（例: /SRC/web/）に置いて確かめる場合は SRCWEB_BASE で指定する（SRCWEB_ROOT はそのサイトのルート）
const base = process.env.SRCWEB_BASE ?? '/';
// 配信したサイト（例: Static Web Apps のプレビュー環境）を確かめるときは DEPLOYED_URL を渡す。手元のサーバーは立てない（tests/deployed.spec.js）
const deployed = process.env.DEPLOYED_URL;

module.exports = defineConfig({
  testDir: './tests',
  // .NET の WebAssembly の読み込みとシナリオの実行に時間がかかる
  timeout: 5 * 60 * 1000,
  expect: { timeout: 60 * 1000 },
  // 並列の数は PW_WORKERS（既定 1）。2 以上ならファイルの中のテストも並べる（CI は 2。docs/testing.md）
  fullyParallel: Number(process.env.PW_WORKERS ?? 1) > 1,
  workers: Number(process.env.PW_WORKERS ?? 1),
  // CI では失敗したテストを 1 度だけやり直す。やり直して通ったものは flaky として結果と注釈に残る（docs/testing.md）
  retries: process.env.CI ? 1 : 0,
  reporter: [
    ['list'],
    ['html', { open: 'never', outputFolder: 'playwright-report' }],
    // CI では結果を JSON にも残し、やり直して通ったテストをジョブのサマリに出す（.github/scripts/playwright-summary.mjs）
    ...(process.env.CI ? [['github'], ['json', { outputFile: 'test-results/results.json' }]] : []),
  ],
  use: {
    baseURL: deployed ? deployed.replace(/\/?$/, '/') : `http://127.0.0.1:${port}${base}`,
    viewport: { width: 800, height: 800 },
    screenshot: 'on',
    trace: 'retain-on-failure',
  },
  projects: [{ name: 'chromium', use: { browserName: 'chromium' } }],
  webServer: deployed ? undefined : {
    command: `node serve.mjs ${process.env.SRCWEB_ROOT ?? '../SRCSharpWeb/bin/web/wwwroot'} ${port}`,
    url: `http://127.0.0.1:${port}${base}`,
    reuseExistingServer: !process.env.CI,
  },
});
