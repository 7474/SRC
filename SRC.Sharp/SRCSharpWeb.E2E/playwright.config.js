// @ts-check
const { defineConfig } = require('@playwright/test');

const port = 5280;
// GitHub Pages と同じくサブパス（例: /SRC/web/）に置いて確かめる場合は SRCWEB_BASE で指定する（SRCWEB_ROOT はそのサイトのルート）
const base = process.env.SRCWEB_BASE ?? '/';

module.exports = defineConfig({
  testDir: './tests',
  // .NET の WebAssembly の読み込みとシナリオの実行に時間がかかる
  timeout: 5 * 60 * 1000,
  expect: { timeout: 60 * 1000 },
  fullyParallel: false,
  workers: 1,
  retries: 0,
  reporter: [['list'], ['html', { open: 'never', outputFolder: 'playwright-report' }]],
  use: {
    baseURL: `http://127.0.0.1:${port}${base}`,
    viewport: { width: 800, height: 800 },
    screenshot: 'on',
    trace: 'retain-on-failure',
  },
  projects: [{ name: 'chromium', use: { browserName: 'chromium' } }],
  webServer: {
    command: `node serve.mjs ${process.env.SRCWEB_ROOT ?? '../SRCSharpWeb/bin/web/wwwroot'} ${port}`,
    url: `http://127.0.0.1:${port}${base}`,
    reuseExistingServer: !process.env.CI,
  },
});
